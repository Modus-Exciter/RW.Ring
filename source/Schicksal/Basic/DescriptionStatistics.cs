using System.Diagnostics;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Описательные статистики для выборок
  /// </summary>
  public static class DescriptionStatistics
  {
    public static double Mean(IDataGroup group)
    {
      Debug.Assert(group != null);

      return group.Sum() / group.Count;
    }

    /// <summary>
    /// Сумма квадратов отклонений
    /// </summary>
    public static double SquareDerivation(IDataGroup group)
    {
      Debug.Assert(group != null);

      var mean = Mean(group);
      double sum = 0;

      foreach (var value in group)
      {
        var derivation = value - mean;
        sum += derivation * derivation;
      }

      return sum;
    }

    /// <summary>
    /// Выборочная дисперсия
    /// </summary>
    public static double Dispresion(IDataGroup group)
    {
      Debug.Assert(group != null);
      Debug.Assert(group.Count > 1);

      return SquareDerivation(group) / (group.Count - 1);
    }
  }
}
