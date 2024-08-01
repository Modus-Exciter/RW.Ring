using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Notung;

namespace Schicksal.Basic
{
  /// <summary>
  /// Описательные статистики для выборок
  /// </summary>
  public static class DescriptionStatistics
  {
    /// <summary>
    /// Среднее арифметическое
    /// </summary>
    public static double Mean(IPlainSample sample)
    {
      Debug.Assert(sample != null);
      return sample.Count > 0 ? sample.Average() : double.NaN;
    }

    /// <summary>
    /// Медиана
    /// </summary>
    public static double Median(IPlainSample sample)
    {
      var ordered = OrderedSample.Construct(sample);

      if (sample.Count % 2 == 0)
        return (ordered[sample.Count / 2 - 1] + ordered[sample.Count / 2]) / 2;
      else
        return ordered[sample.Count / 2];
    }

    /// <summary>
    /// Сумма квадратов отклонений
    /// </summary>
    public static double SquareDerivation(IPlainSample sample)
    {
      Debug.Assert(sample != null);

      var mean = Mean(sample);
      double sum = 0;

      foreach (var value in sample)
      {
        var derivation = value - mean;
        sum += derivation * derivation;
      }

      return sum;
    }

    /// <summary>
    /// Сумма модулей отклонений
    /// </summary>
    public static double PlainDerivation(IPlainSample sample)
    {
      Debug.Assert(sample != null);

      var mean = Mean(sample);
      double sum = 0;

      foreach (var value in sample)
        sum += Math.Abs(value - mean);

      return sum;
    }

    /// <summary>
    /// Выборочная дисперсия
    /// </summary>
    public static double Dispresion(IPlainSample sample)
    {
      Debug.Assert(sample != null);
      Debug.Assert(sample.Count > 1);

      return SquareDerivation(sample) / (sample.Count - 1);
    }

    /// <summary>
    /// Упрощенная выборочная дисперсия
    /// </summary>
    public static double PlainDispersion(IPlainSample sample)
    {
      Debug.Assert(sample != null);
      Debug.Assert(sample.Count > 1);

      return PlainDerivation(sample) / Math.Sqrt(sample.Count - 1);
    }
  }

  /// <summary>
  /// Набор параметров для расчёта описательных статистик в таблице
  /// </summary>
  public sealed class DescriptionStatisticsParameters : PredictedWithProbabilityResponseParameters
  {
    /// <summary>
    /// Инициализация набора параметров для расчёта описательных статистик в таблице
    /// </summary>
    /// <param name="table">Таблица</param>
    /// <param name="filter">Фильтр в таблице</param>
    /// <param name="predictors">Набор имён колонок, которые будут предикторами</param>
    /// <param name="response">Имя колонки, которая будет откликом</param>
    /// <param name="probability">Уровень значимости для анализа</param>
    public DescriptionStatisticsParameters(
      DataTable table,
      string filter,
      FactorInfo predictors,
      string response,
      float probability) : base(table, filter, predictors, response, probability) 
    {
      if (!IsNumeric(table.Columns[response].DataType))
        throw new ArgumentException("Result column must be numeric");
    }
  }

  /// <summary>
  /// Задача для запуска анализа описательных статистик на расчёт
  /// </summary>
  public class DescriptionStatisticsCalculator : RunBase
  {
    private readonly DescriptionStatisticsParameters m_parameters;

    /// <summary>
    /// Инициализация задачи расчёта описательных статистик
    /// </summary>
    /// <param name="parameters">Набор данных для анализа с параметрами</param>
    public DescriptionStatisticsCalculator(DescriptionStatisticsParameters parameters)
    {
      if (parameters == null)
        throw new ArgumentNullException("parameters");

      m_parameters = parameters;
    }

    /// <summary>
    /// Параметры анализа описательных статистик
    /// </summary>
    public DescriptionStatisticsParameters Parameters
    {
      get { return m_parameters; }
    }

    /// <summary>
    /// Результаты анализа описательных статистик
    /// </summary>
    public DescriptionStatisticsEntry[] Result { get; private set; }

    /// <summary>
    /// Запуск задачи на расчёт
    /// </summary>
    public override void Run()
    {
      var sample = new TableDividedSample(m_parameters);
      var res = new DescriptionStatisticsEntry[sample.Count];

      for (int i = 0; i < res.Length; i++)
      {
        var name = sample.GetKey(i).Query;

        if (name.Contains(" AND "))
          name = name.Substring(name.IndexOf(" AND ") + 5);

        if (!string.IsNullOrEmpty(m_parameters.Filter))
          name = name.Replace(" AND " + m_parameters.Filter, "");

        res[i] = new DescriptionStatisticsEntry
        {
          Description = name.Replace(" AND ", ", ").Replace("[", "").Replace("]", ""),
          Mean = DescriptionStatistics.Mean(sample[i]),
          Median = DescriptionStatistics.Median(sample[i]),
          Min = sample[i].Min(),
          Max = sample[i].Max(),
          Count = sample[i].Count
        };

        if (res[i].Count > 1)
        {
          res[i].StdError = Math.Sqrt(DescriptionStatistics.Dispresion(sample[i]));
          res[i].ConfidenceInterval = res[i].StdError / Math.Sqrt(sample[i].Count) *
            SpecialFunctions.invstudenttdistribution(sample[i].Count - 1, 1 - m_parameters.Probability / 2);
        }
        else
        {
          res[i].StdError = double.NaN;
          res[i].ConfidenceInterval = double.NaN;
        }
      }

      this.Result = res;
    }
  }

  /// <summary>
  /// Результат расчёта описательных статистик по одной выборке
  /// </summary>
  public class DescriptionStatisticsEntry
  {
    internal DescriptionStatisticsEntry() { }

    /// <summary>
    /// Описание градации по всем предикторам
    /// </summary>
    public string Description { get; internal set; }

    /// <summary>
    /// Среднее арифметическое
    /// </summary>
    public double Mean { get; internal set; }

    /// <summary>
    /// Медиана
    /// </summary>
    public double Median { get; internal set; }

    /// <summary>
    /// Минимальное значение
    /// </summary>
    public double Min { get; internal set; }

    /// <summary>
    /// Максимальное значение
    /// </summary>
    public double Max { get; internal set; }

    /// <summary>
    /// Объём выборки
    /// </summary>
    public int Count { get; internal set; }

    /// <summary>
    /// Стандартное отклонение
    /// </summary>
    public double StdError { get; internal set; }

    /// <summary>
    /// Доверительный интервал для уровня значимости, указанного в параметрах расчёта
    /// </summary>
    public double ConfidenceInterval { get; internal set; }
  }
}