using System;
using System.Collections.Generic;
using System.Linq;

namespace Schicksal.Discriminant
{
  /// <summary>
  /// Класс, строящий дерево решений на основе заданного критерия (энтропия или Джини)
  /// </summary>
  public class DecisionTreeBuilder
  {
    private readonly DiscriminantParameters.SplitCriterion m_criterion;
    /// <param name="criterion">Критерий разбиения: Entropy или Gini</param>
    public DecisionTreeBuilder(DiscriminantParameters.SplitCriterion criterion)
    {
      m_criterion = criterion;
    }
    /// <summary>
    /// Строит дерево решений 
    /// </summary>
    public DiscriminantTreeNode BuildTree(List<Dictionary<string, object>> data, List<string> features, string targetColumn)
    {
      if (data.Count == 0) return null;
      // Получаем уникальные классы в текущей подвыборке
      var uniqueClasses = data.Select(d => d[targetColumn].ToString()).Distinct().ToList();
      // Если все элементы относятся к одному классу — создаем лист
      if (uniqueClasses.Count == 1)
      {
        return new DiscriminantTreeNode { ClassName = uniqueClasses[0] };
      }
      // Если нет факторов — выбираем наиболее частый класс как ответ
      if (features.Count == 0)
      {
        var majorityClass = data.GroupBy(d => d[targetColumn])
                                .OrderByDescending(g => g.Count())
                                .First().Key.ToString();
        return new DiscriminantTreeNode { ClassName = majorityClass };
      }

      // Выбираем лучший фактор для разделения
      var bestFeature = this.SelectBestSplit(data, features, targetColumn);
      var node = new DiscriminantTreeNode { FeatureName = bestFeature };

      // Находим порог — медиану значений по выбранному фактору
      var featureValues = data.Select(d => Convert.ToDouble(d[bestFeature])).ToList();
      double median = featureValues.OrderBy(x => x).ElementAt(featureValues.Count / 2);
      node.Znach = median;

      // Разделяем данные на левую и правую части
      var leftData = data.Where(d => Convert.ToDouble(d[bestFeature]) <= median).ToList();
      var rightData = data.Where(d => Convert.ToDouble(d[bestFeature]) > median).ToList();

      // Убираем использованный фактор из списка доступных
      var remainingFeatures = new List<string>(features);
      remainingFeatures.Remove(bestFeature);

      // Рекурсивно строим левое и правое поддеревья
      node.Left = this.BuildTree(leftData, remainingFeatures, targetColumn);
      node.Right = this.BuildTree(rightData, remainingFeatures, targetColumn);

      return node;
    }

    /// <summary>
    /// Выбирает лучший фактор для разделения на основе сравнения энтропии
    /// </summary>
    private string SelectBestSplit(List<Dictionary<string, object>> data, List<string> features, string targetColumn)
    {
      // Для каждого фактора сравниваем энтропии
      return features.OrderByDescending(f => this.GetInformationGain(data, f, targetColumn)).First();
    }

    /// <summary>
    /// Сравнивает энтропии
    /// Чем выше — тем лучше фактор делит данные
    /// </summary>
    private double GetInformationGain(List<Dictionary<string, object>> data, string feature, string targetColumn)
    {
      // Энтропия до разделения
      var entropyBefore = this.GetEntropy(data, targetColumn);

      // Находим медианное значение фактора
      var values = data.Select(d => Convert.ToDouble(d[feature])).ToList();
      double median = values.OrderBy(x => x).ElementAt(values.Count / 2);

      // Делим данные на две группы
      var left = data.Where(d => Convert.ToDouble(d[feature]) <= median).ToList();
      var right = data.Where(d => Convert.ToDouble(d[feature]) > median).ToList();

      // Вычисляем веса групп
      double weightLeft = (double)left.Count / data.Count;
      double weightRight = (double)right.Count / data.Count;

      // Энтропия после разделения
      double entropyAfter = weightLeft * this.GetEntropy(left, targetColumn) +
                            weightRight * this.GetEntropy(right, targetColumn);

      return entropyBefore - entropyAfter;
    }

    /// <summary>
    /// Вычисляет энтропию (или коэффициент Джини) для набора данных
    /// </summary>
    private double GetEntropy(List<Dictionary<string, object>> data, string targetColumn)
    {
      // Подсчитываем частоту встречаемости каждого класса
      var classCounts = data.GroupBy(d => d[targetColumn]).ToDictionary(g => g.Key, g => g.Count());
      int total = data.Count;

      // Если используется энтропия
      if (m_criterion == DiscriminantParameters.SplitCriterion.Entropy)
      {
        return -classCounts.Values.Sum(count => ((double)count / total) *
            Math.Log((double)count / total));
      }
      else // Коэффициент Джини
      {
        return 1.0 - classCounts.Values.Sum(count => Math.Pow((double)count / total, 2));
      }
    }
  }
}
