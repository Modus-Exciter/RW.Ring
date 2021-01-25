using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Notung;
using Schicksal.Basic;
using Schicksal.Properties;

namespace Schicksal.Anova
{
  public class VariantsComparator
  {
    private readonly DataTable m_source;
    private readonly string[] m_factors;
    private readonly string m_result;
    private readonly string m_filter;

    public VariantsComparator(DataTable table, string factor, string result, string filter)
    {
      m_source = table;
      m_factors = factor.Split('+');
      m_result = result;
      m_filter = filter;
    }

    public string ResultField
    {
      get { return m_result; }
    }

    public string[] Factors
    {
      get { return m_factors; }
    }

    public string Filter
    {
      get { return m_filter; }
    }

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

      using (var group = new TableMultyDataGroup(m_source, m_factors, m_result, m_filter))
      {
        for (int i = 0; i < group.Count; i++)
        {
          var row = res.NewRow();

          var filter = group.GetKey(i);

          foreach (string factor in m_factors)
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
            if (m_source.Columns[factor].DataType == typeof(string)
              && search.StartsWith("'") && search.EndsWith("'"))
              search = search.Substring(1, search.Length - 2);

            row[factor] = TypeDescriptor.GetConverter(
              m_source.Columns[factor].DataType).ConvertFromInvariantString(search);
          }

          row["Factor"] = string.Join(", ", m_factors.Select(f => row[f]));
          row["Mean"] = DescriptionStatistics.Mean(group[i]);
          row["Count"] = group[i].Count;

          if (group[i].Count > 1)
          {
            row["Std error"] = Math.Sqrt(DescriptionStatistics.Dispresion(group[i]));
            row["Interval"] = ((double)row["Std error"]) / Math.Sqrt(group[i].Count) *
              SpecialFunctions.invstudenttdistribution(group[i].Count - 1, 1 - p / 2);
          }
          else
          {
            row["Interval"] = double.NaN;
            row["Std error"] = double.NaN;
          }
          res.Rows.Add(row);
        }
      }

      return res;
    }

    public DifferenceInfo GetDifferenceInfo(DataRowView row1, DataRowView row2, double p)
    {
      int df;
      double error = GetError(row1, row2, out df);
      DifferenceInfo result = new DifferenceInfo();

      result.Factor1 = row1["Factor"].ToString();
      result.Factor2 = row2["Factor"].ToString();
      result.Mean1 = (double)row1["Mean"];
      result.Mean2 = (double)row2["Mean"];
      result.Result = m_result;
      result.ActualDifference = Math.Abs(result.Mean1 - result.Mean2);

      if (df > 0)
      {
        result.MinimalDifference = error * SpecialFunctions.invstudenttdistribution(df, 1 - p / 2);
        result.Probability = (1 - SpecialFunctions.studenttdistribution(df,
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

    private double GetError(DataRowView row1, DataRowView row2, out int df)
    {
      double std_err1 = (double)row1["Std error"];
      double std_err2 = (double)row2["Std error"];
      int count1 = (int)row1["Count"];
      int count2 = (int)row2["Count"];

      df = count1 + count2 - 2;

      return Math.Sqrt(std_err1 * std_err1 / count1 + std_err2 * std_err2 / count2);
    }
  }

  public sealed class MultiVariantsComparator : RunBase, IServiceProvider
  {
    private readonly VariantsComparator m_comparator;
    private readonly double m_probability;
    private string m_factor1_max;
    private string m_factor2_max;

    public MultiVariantsComparator(VariantsComparator comparator, double p)
    {
      if (comparator == null)
        throw new ArgumentNullException("comparator");

      m_comparator = comparator;
      m_probability = p;
      m_factor1_max = string.Empty;
      m_factor2_max = string.Empty;
    }

    public DifferenceInfo[] Results { get; private set; }

    public DataTable Source { get; private set; }

    public override void Run()
    {
      this.ReportProgress(string.Format("{0}({1}) [{2}]", m_comparator.ResultField,
        string.Join(", ", m_comparator.Factors), m_comparator.Filter));

      if (this.Source == null)
        this.Source = m_comparator.CreateDescriptiveTable(m_probability);

      Tuple<int, int>[] pairs = new Tuple<int, int>[this.Source.Rows.Count * (this.Source.Rows.Count - 1) / 2];
      int k = 0;

      for (int i = 0; i < this.Source.Rows.Count - 1; i++)
      {
        for (int j = i + 1; j < this.Source.Rows.Count; j++)
          pairs[k++] = new Tuple<int, int>(i, j);
      }

      DifferenceInfo[] result = new DifferenceInfo[pairs.Length];

      for (k = 0; k < result.Length; k++)
      {
        result[k] = m_comparator.GetDifferenceInfo(
          this.Source.DefaultView[pairs[k].Item1], this.Source.DefaultView[pairs[k].Item2], m_probability);

        if (result[k].Factor1.Length > m_factor1_max.Length)
          m_factor1_max = result[k].Factor1;

        if (result[k].Factor2.Length > m_factor2_max.Length)
          m_factor2_max = result[k].Factor2;
      }

      this.Results = result;
    }

    public DifferenceInfo CreateExample()
    {
      if (this.Results == null)
        return null;

      return new DifferenceInfo
      {
        Factor1 = m_factor1_max,
        Factor2 = m_factor2_max,
        Mean1 = Results.Select(m => m.Mean1).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m)),
        Mean2 = Results.Select(m => m.Mean2).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m)),
        MinimalDifference = Results.Select(m => m.MinimalDifference).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m)),
        ActualDifference = Results.Select(m => m.ActualDifference).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m)),
        Probability = Results.Select(m => m.Probability).FirstOrDefault(m => !double.IsNaN(m) && !double.IsInfinity(m))
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