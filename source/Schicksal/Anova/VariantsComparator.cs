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
    private readonly DataTable m_source;
    private readonly string[] m_factors;
    private readonly string m_result;
    private readonly string[] m_ignorable_factors;
    private readonly string m_filter;
    private readonly string m_conjugate;
    private double m_within_dispersion;
    private double m_within_df;

    /// <summary>
    /// Инициализация детализации
    /// </summary>
    /// <param name="table">Таблица с исходными данными</param>
    /// <param name="factor">Факторы, градации которых сравниваются</param>
    /// <param name="ignoredFactors">Факторы, градации которых игнорируются, но тоже влияют</param>
    /// <param name="result">Колонка таблицы с эффектом</param>
    /// <param name="filter">Фильтр по таблице</param>
    public VariantsComparator(DataTable table, string factor, string ignoredFactors, string result, string filter, string conjugate)
    {
      m_source = table;
      m_factors = factor.Split('+');
      m_ignorable_factors = string.IsNullOrEmpty(ignoredFactors) ? ArrayExtensions.Empty<string>() : ignoredFactors.Split('+');
      m_result = result;
      m_filter = filter;
      m_conjugate = conjugate;
    }

    /// <summary>
    /// Колонка таблицы с эффектом
    /// </summary>
    public string ResultField
    {
      get { return m_result; }
    }

    /// <summary>
    /// Факторы, градации которых сравниваются
    /// </summary>
    public string[] Factors
    {
      get { return m_factors; }
    }

    /// <summary>
    /// Факторы, градации которых игнорируются, но тоже влияют
    /// </summary>
    public string Filter
    {
      get { return m_filter; }
    }

    /// <summary>
    /// Генерация таблицы с описательными статистиками по факторам, градации которых сравниваются
    /// </summary>
    /// <param name="p">Уровень значимости для расчёта доверительных интервалов</param>
    /// <returns>Таблица содержит градации всех факторов. По каждой градации показано
    /// количество наблюдений, среднее арифметическое, количество наблюдений, стандартное
    /// отклонение, доверительный  интервал и количество градаций игнорируемого фактора</returns>
    public DataTable CreateDescriptiveTable(double p)
    {
      var res = new DataTable();

      res.Columns.Add("Factor", typeof(string));

      foreach (string factor in m_factors)
        res.Columns.Add(factor, m_source.Columns[factor].DataType);

      res.Columns.Add("Count", typeof(int));
      res.Columns.Add("Mean", typeof(double));
      res.Columns.Add("Std error", typeof(double));
      res.Columns.Add("Interval", typeof(double));
      res.Columns.Add("Ignorable", typeof(int));

      using (var groupset = new TableSetDataGroup(m_source, m_factors, m_ignorable_factors, m_result, m_filter, m_conjugate))
      {
        for (int i = 0; i < groupset.Count; i++)
        {
          DataRow row = res.NewRow();
          string filter = groupset.GetKey(i);

          foreach (string factor in m_factors)
          {
            var search = GetFactorLevel(filter, factor);

            if (m_source.Columns[factor].DataType == typeof(string)
              && search.StartsWith("'") && search.EndsWith("'"))
              search = search.Substring(1, search.Length - 2);

            row[factor] = TypeDescriptor.GetConverter(
              m_source.Columns[factor].DataType).ConvertFromInvariantString(search);
          }

          var join = new JoinedDataGroup(groupset[i]);

          row["Factor"] = string.Join(", ", m_factors.Select(f => row[f]));
          row["Mean"] = DescriptionStatistics.Mean(join);
          row["Count"] = join.Count;
          row["Ignorable"] = groupset[i].Count;

          if (join.Count > groupset[i].Count)
          {
            var sum = groupset[i].Sum(b => DescriptionStatistics.SquareDerivation(b));
            row["Std error"] = Math.Sqrt(sum / (join.Count - groupset[i].Count));
            row["Interval"] = ((double)row["Std error"]) / Math.Sqrt((double)join.Count / groupset[i].Count) *
              SpecialFunctions.invstudenttdistribution(join.Count - groupset[i].Count, 1 - p / 2);
          }
          else
          {
            row["Interval"] = double.NaN;
            row["Std error"] = double.NaN;
          }

          res.Rows.Add(row);
        }

        if (string.IsNullOrEmpty(m_conjugate))
        {
          m_within_dispersion = FisherCriteria.GetWithinDispersion(groupset);
          m_within_df = (int)FisherCriteria.GetWithinDegreesOfFreedom(groupset);
        }
        else 
        {
          var criteria = FisherCriteria.CalculateConjugate(groupset);
          m_within_dispersion = criteria.MSw;
          m_within_df = criteria.Ndf;
        }
      }

      return res;
    }

    /// <summary>
    /// Сравнение двух градаций факторов
    /// </summary>
    /// <param name="row1">Первая из сравниваемых строк, содержащих описание градации фактора</param>
    /// <param name="row2">Вторая из сравниваемых строк, содержащих описание градации фактора</param>
    /// <param name="p">Уровень значимости</param>
    /// <returns>Результаты сравнения двух градаций фактора</returns>
    public DifferenceInfo GetDifferenceInfo(DataRowView row1, DataRowView row2, double p)
    {
      double error = this.GetError(row1, row2);
      var result = new DifferenceInfo
      {
        Factor1 = row1["Factor"].ToString(),
        Factor2 = row2["Factor"].ToString(),
        Mean1 = (double)row1["Mean"],
        Mean2 = (double)row2["Mean"],
        Result = m_result
      };

      result.ActualDifference = Math.Abs(result.Mean1 - result.Mean2);

      if (m_within_df > 0)
      {
        result.MinimalDifference = error * SpecialFunctions.invstudenttdistribution((int)m_within_df, 1 - p / 2);
        result.Probability = (1 - SpecialFunctions.studenttdistribution((int)m_within_df,
          Math.Abs(result.Mean2 - result.Mean1) / error)) * 2;
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

    private static string GetFactorLevel(string filter, string factor)
    {
      var search = string.Format("[{0}] = ", factor);
      var index = filter.IndexOf(search);
      if (index >= 0)
      {
        var next = filter.IndexOf(" AND ", index + search.Length);

        if (next >= 0)
          search = filter.Substring(index + search.Length, next - (index + search.Length));
        else
          search = filter.Substring(index + search.Length);
      }
      else
      {
        search = string.Format("[{0}] IS NULL", factor);

        if (!filter.Contains(search))
          search = filter;
        else
          search = CoreResources.NULL;
      }

      return search;
    }

    private double GetError(DataRowView row1, DataRowView row2)
    {
      int count1 = (int)row1["Count"];
      int count2 = (int)row2["Count"];
      int ig1 = (int)row1["Ignorable"];
      int ig2 = (int)row2["Ignorable"];

      if (string.IsNullOrEmpty(m_conjugate))
        return Math.Sqrt(m_within_dispersion * ig1 / count1 + m_within_dispersion * ig2 / count2);
      else
        return Math.Sqrt(m_within_dispersion / count1 + m_within_dispersion / count2);
    }
  }

  /// <summary>
  /// Задача, запускающая сравнение градаций фактора
  /// </summary>
  public sealed class MultiVariantsComparator : RunBase, IServiceProvider
  {
    private readonly VariantsComparator m_comparator;
    private readonly double m_probability;
    private string m_factor1_max;
    private string m_factor2_max;

    /// <summary>
    /// Инициализация новой задачи на сравнение градаций факторов
    /// </summary>
    /// <param name="comparator">Объект для сравнения градаций фактора</param>
    /// <param name="p">Уровень значимости</param>
    public MultiVariantsComparator(VariantsComparator comparator, double p)
    {
      if (comparator == null)
        throw new ArgumentNullException("comparator");

      m_comparator = comparator;
      m_probability = p;
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
      this.ReportProgress(string.Format("{0}({1}) [{2}]", m_comparator.ResultField,
        string.Join(", ", m_comparator.Factors), m_comparator.Filter));

      if (this.Source == null)
        this.Source = m_comparator.CreateDescriptiveTable(m_probability);

      var result = new DifferenceInfo[this.Source.Rows.Count * (this.Source.Rows.Count - 1) / 2];
      int k = 0;

      for (int i = 0; i < this.Source.Rows.Count - 1; i++)
      {
        for (int j = i + 1; j < this.Source.Rows.Count; j++)
        {
          result[k] = m_comparator.GetDifferenceInfo(
            this.Source.DefaultView[i], this.Source.DefaultView[j], m_probability);

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