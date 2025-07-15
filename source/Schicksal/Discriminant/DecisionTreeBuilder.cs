using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Schicksal.Discriminant
{
    public class DecisionTreeBuilder
    {
        private readonly DiscriminantParameters.SplitCriterion m_criterion;

        public DecisionTreeBuilder(DiscriminantParameters.SplitCriterion criterion)
        {
            m_criterion = criterion;
        }

        public DiscriminantTreeNode BuildTree(List<Dictionary<string, object>> data, List<string> features, string targetColumn)
        {
            if (data == null || data.Count == 0) return null;

            // Шаг 1: Заполняем пропущенные значения в числовых столбцах
            data = FillMissingValues(data, features);

            // Шаг 2: Очищаем строковые значения
            data = CleanStringValues(data, features);

            var uniqueClasses = data.Select(d => d[targetColumn]?.ToString()).Distinct().ToList();

            if (uniqueClasses.Count == 1)
            {
                return new DiscriminantTreeNode { ClassName = uniqueClasses[0] };
            }

            if (features.Count == 0)
            {
                var majorityClass = data.GroupBy(d => d[targetColumn]?.ToString())
                                        .OrderByDescending(g => g.Count())
                                        .First().Key;
                return new DiscriminantTreeNode { ClassName = majorityClass };
            }

            var bestFeature = SelectBestSplit(data, features, targetColumn);
            var node = new DiscriminantTreeNode { FeatureName = bestFeature };

            var featureValues = data
                .Select(d => new { Value = d[bestFeature], IsNum = TryGetNumericValue(d[bestFeature], out double num), NumVal = num })
                .ToList();

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

                node.Left = BuildTree(leftData, remainingFeatures, targetColumn);
                node.Right = BuildTree(rightData, remainingFeatures, targetColumn);
            }
            else
            {
                node.SplitType = SplitType.Categorical;
                node.Categories = new Dictionary<string, DiscriminantTreeNode>();

                var groups = data.GroupBy(d => d[bestFeature]?.ToString() ?? "Unknown");

                foreach (var group in groups)
                {
                    var remainingFeatures = new List<string>(features);
                    remainingFeatures.Remove(bestFeature);

                    var childNode = BuildTree(group.ToList(), remainingFeatures, targetColumn);
                    node.Categories[group.Key] = childNode;
                }
            }

            return node;
        }

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

        private string SelectBestSplit(List<Dictionary<string, object>> data, List<string> features, string targetColumn)
        {
            return features.OrderByDescending(f => GetInformationGain(data, f, targetColumn)).FirstOrDefault();
        }

        private double GetInformationGain(List<Dictionary<string, object>> data, string feature, string targetColumn)
        {
            var entropyBefore = GetEntropy(data, targetColumn);

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

                double entropyAfter = weightLeft * GetEntropy(left, targetColumn) +
                                      weightRight * GetEntropy(right, targetColumn);

                return entropyBefore - entropyAfter;
            }
            else
            {
                var groups = data.GroupBy(d => d[feature]?.ToString() ?? "Unknown").ToList();
                double entropyAfter = groups.Sum(g =>
                {
                    double weight = (double)g.Count() / data.Count;
                    return weight * GetEntropy(g.ToList(), targetColumn);
                });

                return entropyBefore - entropyAfter;
            }
        }

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

        private double GetEntropy(List<Dictionary<string, object>> data, string targetColumn)
        {
            var classCounts = data
                .GroupBy(d => d[targetColumn]?.ToString() ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            int total = data.Count;

            if (m_criterion == DiscriminantParameters.SplitCriterion.Entropy)
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