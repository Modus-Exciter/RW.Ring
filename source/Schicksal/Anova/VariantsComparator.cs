using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Notung;
using Notung.Data;
using Schicksal.Basic;
using Schicksal.Properties;

namespace Schicksal.Anova
{
  /// <summary>
  /// Детализация дисперсионного анализа, сравнение отдельных градаций факторов
  /// </summary>
  public class VariantsComparator
  {
    private readonly IPrimaryAnovaResults m_primary_results;
    private readonly FactorInfo m_predictor;
    private readonly SampleVariance m_msw;

    public VariantsComparator(IPrimaryAnovaResults primaryResults, FactorInfo predictor, SampleVariance msw)
    {
      if (primaryResults is null)
        throw new ArgumentNullException("primaryResults");

      if (predictor is null)
        throw new ArgumentNullException("predictor");

      m_primary_results = primaryResults;
      m_predictor = predictor;
      m_msw = msw;
    }

    /// <summary>
    /// Результаты первичного анализа, на базе которых делается детальный анализ
    /// </summary>
    public IPrimaryAnovaResults PrimaryResluts
    {
      get { return m_primary_results; }
    }

    /// <summary>
    /// Генерация таблицы с описательными статистиками по факторам, градации которых сравниваются
    /// </summary>
    /// <returns>Таблица содержит градации всех факторов. По каждой градации показано
    /// количество наблюдений, среднее арифметическое, количество наблюдений, стандартное
    /// отклонение, доверительный  интервал и количество градаций игнорируемого фактора</returns>
    public DataTable CreateDescriptiveTable()
    {
      var res = new DataTable();

      res.Columns.Add("+Factor", typeof(string)).ColumnMapping = MappingType.Hidden;

      foreach (string p in m_predictor)
        res.Columns.Add(p, m_primary_results.Parameters.Table.Columns[p].DataType);

      res.Columns.Add("+Count", typeof(int));
      res.Columns.Add("+Mean", typeof(double));
      res.Columns.Add("+Std error", typeof(double));
      res.Columns.Add("+Interval", typeof(double));
      res.Columns.Add("+MeanNormalized", typeof(double)).ColumnMapping = MappingType.Hidden;
      res.Columns.Add("+ErrorNormalized", typeof(double)).ColumnMapping = MappingType.Hidden;
      res.Columns.Add("+Sample", typeof(IDividedSample<GroupKey>)).ColumnMapping = MappingType.Hidden;

      var groupset = new DoubleGroupedSample(m_primary_results.DataSet, m_predictor);

      res.BeginLoadData();

      for (int i = 0; i < groupset.Count; i++)
      {
        DataRow row = res.NewRow();

        this.FillDataRow(row, groupset.GetKey(i), groupset[i]);

        res.Rows.Add(row);
      }

      res.EndLoadData();

      return res;
    }

    public DifferenceInfo GetDifferenceInfo(DataRowView row1, DataRowView row2)
    {
      var result = new DifferenceInfo
      {
        Factor1 = row1["+Factor"].ToString(),
        Factor2 = row2["+Factor"].ToString(),
        Mean1 = (double)row1["+Mean"],
        Mean2 = (double)row2["+Mean"],
        Result = m_primary_results.Parameters.Response
      };

      result.ActualDifference = Math.Abs(result.Mean1 - result.Mean2);

      double n_mean1 = (double)row1["+MeanNormalized"];
      double n_mean2 = (double)row2["+MeanNormalized"];
      ErrorInfo error = this.GetError(row1, row2);

      if (error.DegreesOfFreedom > 0)
      {
        result.MinimalDifference = this.RecalcLSD(this.GetLSD(error), n_mean1, n_mean2);
        result.Probability = this.GetErrorProbability(Math.Abs(n_mean1 - n_mean2), error);
      }
      else
      {
        result.MinimalDifference = double.PositiveInfinity;
        result.Probability = 1;
      }

      if (result.Probability < 0)
        result.Probability = 0;

      return result;
    }

    private void FillDataRow(DataRow row, GroupKey groupKey, IDividedSample group)
    {
      foreach (var kv in groupKey)
      {
        var col_type = m_primary_results.Parameters.Table.Columns[kv.Key].DataType;

        row[kv.Key] = kv.Value.GetType() == col_type || kv.Value is DBNull
          ? kv.Value
          : TypeDescriptor.GetConverter(col_type).ConvertFromInvariantString(kv.Value.ToString());
      }

      row["+Factor"] = string.Join(", ", m_predictor.Select(f => row[f]));

      var join = new JoinedSample(group);
      var mean = DescriptionStatistics.Mean(join);
      var std_der = m_primary_results.ResudualsCalculator.GetStandardDerivation(group, join);

      row["+Count"] = join.Count;
      row["+Mean"] = m_primary_results.ValueTransform.Denormalize(mean);
      row["+MeanNormalized"] = mean;
      row["+Sample"] = group;

      if (std_der.DegreesOfFreedom > 0)
      {
        var std_error = Math.Sqrt(std_der.MeanSquare);
        var interval = std_error / Math.Sqrt(join.Count)
          * SpecialFunctions.invstudenttdistribution
          (
            std_der.DegreesOfFreedom,
            1 - m_primary_results.Parameters.Probability / 2
          );

        row["+Std error"] = this.RecalcDerivation(mean, std_error);
        row["+Interval"] = this.RecalcDerivation(mean, interval);
        row["+ErrorNormalized"] = std_der.MeanSquare;
      }
      else
      {
        row["+Std error"] = double.NaN;
        row["+Interval"] = double.NaN;
        row["+ErrorNormalized"] = double.NaN;
      }
    }

    private double RecalcDerivation(double center, double derivation)
    {
      var up = m_primary_results.ValueTransform.Denormalize(center + derivation);
      var dn = m_primary_results.ValueTransform.Denormalize(center - derivation);

      return Math.Abs(up - dn) / 2;
    }

    private double RecalcLSD(double lsd, double bigger, double smaller)
    {
      var up = m_primary_results.ValueTransform.Denormalize(bigger);
      var dn = m_primary_results.ValueTransform.Denormalize(smaller);

      return Math.Abs((up - dn) * lsd / (bigger - smaller));
    }

    private double GetLSD(ErrorInfo error)
    {
      return error.Value * SpecialFunctions.invstudenttdistribution
      (
        error.DegreesOfFreedom,
        1 - m_primary_results.Parameters.Probability / 2
      );
    }

    private double GetErrorProbability(double normalizedDifference, ErrorInfo error)
    {
      return
        (
          1 - SpecialFunctions.studenttdistribution
          (
            error.DegreesOfFreedom,
            normalizedDifference / error.Value
          )
        ) * 2;
    }

    private ErrorInfo GetError(DataRowView row1, DataRowView row2)
    {
      int count1 = (int)row1["+Count"];
      int count2 = (int)row2["+Count"];

      if (m_primary_results.Parameters.IndividualError)
      {
        var sample1 = new Gradation
        {
          ErrorValue = (double)row1["+ErrorNormalized"],
          TotalCount = count1,
          Sample = (IDividedSample)row1["+Sample"]
        };

        var sample2 = new Gradation
        {
          ErrorValue = (double)row2["+ErrorNormalized"],
          TotalCount = count2,
          Sample = (IDividedSample)row2["+Sample"]
        };

        return m_primary_results.ResudualsCalculator.GetErrorInfo(sample1, sample2);
      }

      return new ErrorInfo
      {
        Value = Math.Sqrt(m_msw.MeanSquare / count1 + m_msw.MeanSquare / count2),
        DegreesOfFreedom = m_msw.DegreesOfFreedom
      };
    }
  }

  /// <summary>
  /// Задача, запускающая сравнение градаций фактора
  /// </summary>
  public sealed class MultiVariantsComparator : RunBase, IServiceProvider
  {
    private readonly VariantsComparator m_comparator;
    private string m_factor1_max;
    private string m_factor2_max;

    /// <summary>
    /// Инициализация новой задачи на сравнение градаций факторов
    /// </summary>
    /// <param name="comparator">Объект для сравнения градаций фактора</param>
    /// <param name="p">Уровень значимости</param>
    public MultiVariantsComparator(VariantsComparator comparator)
    {
      if (comparator == null)
        throw new ArgumentNullException("comparator");

      m_comparator = comparator;
      m_factor1_max = string.Empty;
      m_factor2_max = string.Empty;
    }

    /// <summary>
    /// Результаты попарного сравнения градаций фактора
    /// </summary>
    public DifferenceInfo[] Results { get; private set; }

    /// <summary>
    /// Информация обо всех градациях фактора
    /// </summary>
    public DataTable Source { get; private set; }

    /// <summary>
    /// Запуск задачи на выполнение
    /// </summary>
    public override void Run()
    {
      this.ReportProgress(string.Format("{0}({1}) [{2}]", 
        m_comparator.PrimaryResluts.Parameters.Response,
        string.Join(", ", m_comparator.PrimaryResluts.Parameters.Predictors), 
        m_comparator.PrimaryResluts.Parameters.Filter));

      if (this.Source == null)
        this.Source = m_comparator.CreateDescriptiveTable();

      var result = new DifferenceInfo[this.Source.Rows.Count * (this.Source.Rows.Count - 1) / 2];
      int k = 0;

      for (int i = 0; i < this.Source.Rows.Count - 1; i++)
      {
        for (int j = i + 1; j < this.Source.Rows.Count; j++)
        {
          result[k] = m_comparator.GetDifferenceInfo(
            this.Source.DefaultView[i], this.Source.DefaultView[j]);

          if (result[k].Factor1.Length > m_factor1_max.Length)
            m_factor1_max = result[k].Factor1;

          if (result[k].Factor2.Length > m_factor2_max.Length)
            m_factor2_max = result[k].Factor2;

          k++;
        }
      }

      this.Results = result;
    }

    /// <summary>
    /// Создание примера строки для быстрой настройки ширины колонок таблицы
    /// </summary>
    /// <returns>Первый попавшийся объект из результатов с заполненными всеми полями</returns>
    public DifferenceInfo CreateExample()
    {
      if (this.Results == null)
        return null;

      return new DifferenceInfo
      {
        Factor1 = m_factor1_max,
        Factor2 = m_factor2_max,
        Mean1 = this.Results.Select(m => m.Mean1).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m)),
        Mean2 = this.Results.Select(m => m.Mean2).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m)),
        MinimalDifference = this.Results.Select(m => m.MinimalDifference).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m)),
        ActualDifference = this.Results.Select(m => m.ActualDifference).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m)),
        Probability = this.Results.Select(m => m.Probability).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m))
      };
    }

    public override string ToString()
    {
      return Resources.VARIANTS_COMPARISON;
    }

    public override object GetService(Type serviceType)
    {
      if (serviceType == typeof(Image))
        return Resources.Comparison;

      return base.GetService(serviceType);
    }
  }
}