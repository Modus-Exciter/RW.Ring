using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Schicksal.Basic;
using System.ComponentModel;
using System.Globalization;
using Notung;

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

    public DataTable CreateDescriptiveTable(double p)
    {
      var group = new TableMultyDataGroup(m_source, m_factors, m_result, m_filter);
      var res = new DataTable();

      res.Columns.Add("Factor", typeof(string));

      foreach (string factor in m_factors)
        res.Columns.Add(factor, m_source.Columns[factor].DataType);

      res.Columns.Add("Count", typeof(int));
      res.Columns.Add("Mean", typeof(double));
      res.Columns.Add("Std error", typeof(double));
      res.Columns.Add("Interval", typeof(double));

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

          row[factor] = TypeDescriptor.GetConverter(m_source.Columns[factor].DataType).ConvertFromInvariantString(search);
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

      return res;
    }

    public sealed class MultiVariantsComparator : RunBase
    {
      private readonly VariantsComparator m_comparator;
      private readonly DataTable m_source;
      private readonly double m_probability;

      public MultiVariantsComparator(VariantsComparator comparator, double p, DataTable table = null)
      {
        if (comparator == null)
          throw new ArgumentNullException("comparator");

        m_comparator = comparator;
        m_probability = p;
        m_source = table ?? m_comparator.CreateDescriptiveTable(p);
      }

      public DifferenceInfo[] Results { get; private set; }

      public override void Run()
      {
        Tuple<int, int>[] pairs = new Tuple<int, int>[m_source.Rows.Count * (m_source.Rows.Count - 1) / 2];
        int k = 0;

        for (int i = 0; i < m_source.Rows.Count - 1; i++)
        {
          for (int j = i + 1; j < m_source.Rows.Count; j++)
            pairs[k++] = new Tuple<int, int>(i, j);
        }

        DifferenceInfo[] result = new DifferenceInfo[pairs.Length];

        for (k = 0; k < result.Length; k++)
        {
          this.ReportProgress(k * 100 / result.Length, null);

          result[k] = m_comparator.GetDifferenceInfo(
            m_source.DefaultView[pairs[k].Item1], m_source.DefaultView[pairs[k].Item2], m_probability);
        }

        this.Results = result;
      }
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

  public sealed class DifferenceInfo
  {
    public string Factor1 { get; internal set; }

    public double Mean1 { get; internal set; }

    public string Factor2 { get; internal set; }

    public double Mean2 { get; internal set; }

    [Browsable(false)]
    public string Result { get; internal set; }

    public double ActualDifference { get; internal set; }

    public double MinimalDifference { get; internal set; }

    public double Probability { get; internal set; }

    public override string ToString()
    {
      return string.Format("{0} vs {1}", Factor1, Factor2);
    }

    public Tuple<string, string>[] ToTuples()
    {
      var list = new Tuple<string, string>[10];

      list[1] = new Tuple<string, string>("1. ", Factor1.ToString());
      list[2] = new Tuple<string, string>(Result, Mean1.ToString("0.0000", CultureInfo.InvariantCulture));

      list[4] = new Tuple<string, string>("2. ", Factor2.ToString());
      list[5] = new Tuple<string, string>(Result, Mean2.ToString("0.0000", CultureInfo.InvariantCulture));

      list[7] = new Tuple<string, string>("Actual difference",
        ActualDifference.ToString("0.0000", CultureInfo.InvariantCulture));

      list[8] = new Tuple<string, string>("Critical difference",
        MinimalDifference.ToString("0.0000", CultureInfo.InvariantCulture));

      list[9] = new Tuple<string, string>("P ",
        Probability.ToString("0.0000", CultureInfo.InvariantCulture));

      return list;
    }
  }
}
