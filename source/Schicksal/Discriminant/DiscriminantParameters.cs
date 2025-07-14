using System;
using Schicksal.Basic;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Discriminant
{
  /// <summary>
  /// Параметры для запуска дискриминантного анализа
  /// Наследуется от базового класса PredictedWithProbabilityResponseParameters
  /// </summary>
  public class DiscriminantParameters : PredictedWithProbabilityResponseParameters
    {
    /// <summary>
    /// Перечисление
    /// - Entropy — информационная энтропия
    /// - Gini — коэффициент Джини
    /// </summary>
    public enum SplitCriterion { Entropy, Gini }
    /// <summary>
    /// Критерий разделения узлов
    /// </summary>
    public SplitCriterion Criterion { get; set; }

      public DiscriminantParameters(
          DataTable table,
          string filter,
          FactorInfo predictors,
          string response,
          float probability,
          SplitCriterion criterion)
          : base(table, filter, predictors, response, probability)
      {
      this.Criterion = criterion;
      }
    }
}
