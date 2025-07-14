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
        var builder = new DecisionTreeBuilder(m_parameters.Criterion);
          var tree = builder.BuildTree(data, featureColumns, targetColumn);

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
    /// Прогнозирует класс для одного объекта.
    /// </summary>
    private string Predict(DiscriminantTreeNode node, Dictionary<string, object> sample)
      {
        while (!node.End)
        {
          double value = Convert.ToDouble(sample[node.FeatureName]);
          node = value <= node.Znach ? node.Left : node.Right;
        }
        return node.ClassName;
      }
    }
  }
 
