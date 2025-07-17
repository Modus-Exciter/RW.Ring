using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Notung;
using Schicksal.Basic;


namespace Schicksal.Discriminant
{
  /// <summary>
  /// Строит дерево решений и оценивает его точность
  /// </summary>
  public class DiscriminantProcessor : RunBase
    {
      private readonly DiscriminantParameters m_parameters;
      public DiscriminantResult Result { get; private set; }

      public DiscriminantProcessor(DiscriminantParameters parameters)
      {
      m_parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
      }
    /// <summary>
    /// Запуск анализа
    /// </summary>
    public override void Run()
      {
        try
        {
          var dataTable = m_parameters.Table;
          var filter = m_parameters.Filter;
          var targetColumn = m_parameters.Response;
          var featureColumns = m_parameters.Predictors.ToList();
        // Фильтруем строки по фильтру
        var filteredRows = string.IsNullOrEmpty(filter)
            ? dataTable.Select()
            : dataTable.Select(filter);

        var data = new List<Dictionary<string, object>>();
        // Создаем список словарей для удобной работы с данными
        foreach (DataRow row in filteredRows)
          {
            var item = new Dictionary<string, object>();
            foreach (string col in featureColumns.Concat(new[] { targetColumn }))
            {
              item[col] = row[col];
            }
            data.Add(item);
          }

          this.ReportProgress(25);
        // Строим дерево
        // Строим дерево
        var builder = new DecisionTreeBuilder(); // <-- убираем параметр

        var tree = builder.BuildTree(
            data,
            featureColumns,
            targetColumn,
            m_parameters.Criterion, // <-- передаём критерий
            maxDepth: 5); // <-- необязательно, но можно указать

          this.ReportProgress(75);

          // Оценка точности
          double accuracy = this.EvaluateTree(tree, data, targetColumn);
        // Подсчитываем распределение классов
        var classDistribution = data
              .GroupBy(d => d[targetColumn].ToString())
              .ToDictionary(g => g.Key, g => g.Count());
        
        // Сохраняем результат
        this.Result = new DiscriminantResult
          {
            DecisionTree = tree,
            Accuracy = accuracy,
            Classandelement = classDistribution
          };

          this.ReportProgress(100);
        }
        catch (Exception ex)
        {
          throw new Exception($"Ошибка дискриминантного анализа: {ex.Message}");
        }
      }
    /// <summary>
    /// Оценивает точность дерева
    /// </summary>
    private double EvaluateTree(DiscriminantTreeNode tree, List<Dictionary<string, object>> data, string targetColumn)
      {
        int correct = 0;
        foreach (var item in data)
        {
          string predicted = this.Predict(tree, item);
          string actual = item[targetColumn].ToString();
          if (predicted == actual)
            correct++;
        }
        return (double)correct / data.Count;
      }
    /// <summary>
    /// Прогнозирует класс для одного объекта
    /// </summary>
    private string Predict(DiscriminantTreeNode node, Dictionary<string, object> sample)
    {
      while (!node.End)
      {
        var featureValue = sample[node.FeatureName];

        if (featureValue == null)
          return "Unknown";

        if (node.SplitType == SplitType.Numeric)
        {
          if (TryGetNumericValue(featureValue, out double val))
          {
            node = val <= node.Znach ? node.Left : node.Right;
          }
          else
          {
            return "Unknown"; // не удалось привести к числу
          }
        }
        else if (node.SplitType == SplitType.Categorical)
        {
          string strVal = featureValue.ToString();
          if (node.Categories.TryGetValue(strVal, out var next))
          {
            node = next;
          }
          else if (node.Categories.TryGetValue("Unknown", out var fallback))
          {
            node = fallback;
          }
          else
          {
            return "Unknown";
          }
        }
        else
        {
          return "Unknown"; // неизвестный тип
        }
      }

      return node.ClassName;
    }
    /// <summary>
    /// Пробует привести значение к числу (double), обрабатывая null и ошибки приведения
    /// </summary>
    private bool TryGetNumericValue(object value, out double result)
    {
      if (value == null || value is DBNull)
      {
        result = double.NaN;
        return false;
      }

      switch (value)
      {
        case string s when double.TryParse(s, out var dbl):
          result = dbl;
          return true;
        case IConvertible convertible:
          try
          {
            result = convertible.ToDouble(null);
            return true;
          }
          catch
          {
            result = double.NaN;
            return false;
          }
        default:
          result = double.NaN;
          return false;
      }
    }

  }
}
 
