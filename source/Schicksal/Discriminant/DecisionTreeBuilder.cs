using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Schicksal.Discriminant
{
    public class DecisionTreeBuilder
    {
        /// <summary>
        /// Основной метод построения дерева решений.
        /// </summary>
        /// <param name="data">Данные в виде списка словарей (строки таблицы)</param>
        /// <param name="features">Список факторов (имена колонок), по которым можно разделять</param>
        /// <param name="targetColumn">Целевая колонка (класс)</param>
        /// <param name="criterion">Критерий разделения: Entropy или Gini</param>
        /// <param name="currentDepth">Текущая глубина рекурсии (для ограничения)</param>
        /// <param name="maxDepth">Максимальная глубина дерева</param>
        /// <returns>Корень дерева (DiscriminantTreeNode)</returns>
        public DiscriminantTreeNode BuildTree(
            List<Dictionary<string, object>> data,
            List<string> features,
            string targetColumn,
            DiscriminantParameters.SplitCriterion criterion,
            int currentDepth = 0,
            int maxDepth = 5)
        {
            if (data == null || data.Count == 0) return null;

            // Шаг 1: Заполняем пропущенные значения в числовых столбцах
            data = FillMissingValues(data, features);

            // Шаг 2: Очищаем строковые значения (удаляем мусор и заменяем null на "Unknown")
            data = CleanStringValues(data, features);

            // Проверяем, сколько уникальных классов в текущем узле
            var uniqueClasses = data.Select(d => d[targetColumn]?.ToString()).Distinct().ToList();

            // Если все элементы принадлежат одному классу — создаём лист
            if (uniqueClasses.Count == 1)
            {
                return new DiscriminantTreeNode { ClassName = uniqueClasses[0] };
            }

            // Если закончились факторы или достигнута максимальная глубина — создаём лист с наиболее частым классом
            if (features.Count == 0 || currentDepth >= maxDepth)
            {
                var majorityClass = data.GroupBy(d => d[targetColumn]?.ToString())
                                        .OrderByDescending(g => g.Count())
                                        .First().Key;
                return new DiscriminantTreeNode { ClassName = majorityClass };
            }

            // Выбираем лучший фактор для разделения
            var bestFeature = SelectBestSplit(data, features, targetColumn, criterion);
            var node = new DiscriminantTreeNode { FeatureName = bestFeature };

            // Получаем значения текущего фактора и проверяем, числовые ли они
            var featureValues = data
                .Select(d => new { Value = d[bestFeature], IsNum = TryGetNumericValue(d[bestFeature], out double num), NumVal = num })
                .ToList();

            // Если есть числовые значения — разделяем по медиане
            if (featureValues.Any(f => f.IsNum))
            {
                var numericValues = featureValues.Where(f => f.IsNum).Select(f => f.NumVal).ToList();
                int count = numericValues.Count;
                double median = count == 0 ? 0 : numericValues.OrderBy(x => x).ElementAt(count / 2);
                node.Znach = median;
                node.SplitType = SplitType.Numeric;

                var leftData = data.Where(d => TryGetNumericValue(d[bestFeature], out double val) && val <= median).ToList();
                var rightData = data.Where(d => TryGetNumericValue(d[bestFeature], out double val) && val > median).ToList();

                var remainingFeatures = new List<string>(features);
                remainingFeatures.Remove(bestFeature);

                // Рекурсивно строим поддеревья
                node.Left = BuildTree(leftData, remainingFeatures, targetColumn, criterion, currentDepth + 1, maxDepth);
                node.Right = BuildTree(rightData, remainingFeatures, targetColumn, criterion, currentDepth + 1, maxDepth);
            }
            else
            {
                // Если фактор категориальный — строим словарь для каждого значения
                node.SplitType = SplitType.Categorical;
                node.Categories = new Dictionary<string, DiscriminantTreeNode>();

                var groups = data.GroupBy(d => d[bestFeature]?.ToString() ?? "Unknown");

                foreach (var group in groups)
                {
                    var remainingFeatures = new List<string>(features);
                    remainingFeatures.Remove(bestFeature);

                    var childNode = BuildTree(group.ToList(), remainingFeatures, targetColumn, criterion, currentDepth + 1, maxDepth);
                    node.Categories[group.Key] = childNode;
                }
            }

            return node;
        }

        /// <summary>
        /// Заполняет пропущенные числовые значения медианой по столбцу
        /// </summary>
        private List<Dictionary<string, object>> FillMissingValues(List<Dictionary<string, object>> data, List<string> features)
        {
            foreach (var feature in features)
            {
                var values = data
                    .Where(d => TryGetNumericValue(d[feature], out double val))
                    .Select(d => Convert.ToDouble(d[feature]))
                    .ToList();

                if (values.Count == 0) continue;

                double median = values.OrderBy(x => x).ElementAt(values.Count / 2);

                foreach (var row in data)
                {
                    if (row.ContainsKey(feature) && (row[feature] == null || row[feature] is DBNull))
                    {
                        row[feature] = median;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Очищает строковые значения: удаляет спецсимволы и заменяет null на "Unknown"
        /// </summary>
        private List<Dictionary<string, object>> CleanStringValues(List<Dictionary<string, object>> data, List<string> features)
        {
            foreach (var feature in features)
            {
                foreach (var row in data)
                {
                    if (row.ContainsKey(feature) && row[feature] is string str)
                    {
                        // Удаляем лишние пробелы и специальные символы
                        row[feature] = Regex.Replace(str.Trim(), @"[^a-zA-Z0-9]", "");
                    }
                    else if (row.ContainsKey(feature) && (row[feature] == null || row[feature] is DBNull))
                    {
                        row[feature] = "Unknown";
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Выбирает лучший фактор для разделения на основе прироста информации
        /// </summary>
        private string SelectBestSplit(
            List<Dictionary<string, object>> data,
            List<string> features,
            string targetColumn,
            DiscriminantParameters.SplitCriterion criterion)
        {
            return features
                .OrderByDescending(f => GetInformationGain(data, f, targetColumn, criterion))
                .FirstOrDefault();
        }

        /// <summary>
        /// Вычисляет прирост информации для фактора (на основе Gini или Entropy)
        /// </summary>
        private double GetInformationGain(
            List<Dictionary<string, object>> data,
            string feature,
            string targetColumn,
            DiscriminantParameters.SplitCriterion criterion)
        {
            var entropyBefore = GetEntropy(data, targetColumn, criterion);

            var values = data
                .Select(d => new { Value = d[feature], IsNum = TryGetNumericValue(d[feature], out double num), NumVal = num })
                .ToList();

            if (values.Any(v => v.IsNum))
            {
                var numericValues = values.Where(v => v.IsNum).Select(v => v.NumVal).ToList();
                int count = numericValues.Count;
                double median = count == 0 ? 0 : numericValues.OrderBy(x => x).ElementAt(count / 2);

                var left = data.Where(d => TryGetNumericValue(d[feature], out double val) && val <= median).ToList();
                var right = data.Where(d => TryGetNumericValue(d[feature], out double val) && val > median).ToList();

                double weightLeft = (double)left.Count / data.Count;
                double weightRight = (double)right.Count / data.Count;

                double entropyAfter = weightLeft * GetEntropy(left, targetColumn, criterion) +
                                      weightRight * GetEntropy(right, targetColumn, criterion);

                return entropyBefore - entropyAfter;
            }
            else
            {
                var groups = data.GroupBy(d => d[feature]?.ToString() ?? "Unknown").ToList();
                double entropyAfter = groups.Sum(g =>
                {
                    double weight = (double)g.Count() / data.Count;
                    return weight * GetEntropy(g.ToList(), targetColumn, criterion);
                });

                return entropyBefore - entropyAfter;
            }
        }

        /// <summary>
        /// Пытается преобразовать значение в число
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

        /// <summary>
        /// Вычисляет неопределённость (Entropy или Gini) для текущей подвыборки
        /// </summary>
        private double GetEntropy(
            List<Dictionary<string, object>> data,
            string targetColumn,
            DiscriminantParameters.SplitCriterion criterion)
        {
            var classCounts = data
                .GroupBy(d => d[targetColumn]?.ToString() ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            int total = data.Count;

            if (criterion == DiscriminantParameters.SplitCriterion.Entropy)
            {
                return -classCounts.Values.Sum(count => ((double)count / total) *
                    Math.Log((double)count / total));
            }
            else
            {
                return 1.0 - classCounts.Values.Sum(count => Math.Pow((double)count / total, 2));
            }
        }
    }
}