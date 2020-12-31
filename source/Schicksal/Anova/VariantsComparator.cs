using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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

  public sealed class MultiVariantsComparator : RunBase, IServiceProvider
  {
    private readonly VariantsComparator m_comparator;
    private readonly double m_probability;

    public MultiVariantsComparator(VariantsComparator comparator, double p)
    {
      if (comparator == null)
        throw new ArgumentNullException("comparator");

      m_comparator = comparator;
      m_probability = p;
      this.Factor1MaxLength = string.Empty;
      this.Factor2MaxLength = string.Empty;
    }

    public DifferenceInfo[] Results { get; private set; }

    public string Factor1MaxLength { get; private set; }

    public string Factor2MaxLength { get; private set; }

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

        if (result[k].Factor1.Length > this.Factor1MaxLength.Length)
          this.Factor1MaxLength = result[k].Factor1;

        if (result[k].Factor2.Length > this.Factor2MaxLength.Length)
          this.Factor2MaxLength = result[k].Factor2;
      }

      this.Results = result;
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

      list[1] = new Tuple<string, string>(string.Format("{0} 1. ", Resources.FACTOR), Factor1.ToString());
      list[2] = new Tuple<string, string>(Result, Mean1.ToString("0.0000", CultureInfo.InvariantCulture));

      list[4] = new Tuple<string, string>(string.Format("{0} 2. ", Resources.FACTOR), Factor2.ToString());
      list[5] = new Tuple<string, string>(Result, Mean2.ToString("0.0000", CultureInfo.InvariantCulture));

      list[7] = new Tuple<string, string>(Resources.ACTUAL_DIFFERENCE,
        ActualDifference.ToString("0.0000", CultureInfo.InvariantCulture));

      list[8] = new Tuple<string, string>(Resources.CRITICAL_DIFFERENCE,
        MinimalDifference.ToString("0.0000", CultureInfo.InvariantCulture));

      list[9] = new Tuple<string, string>("P ",
        Probability.ToString("0.0000", CultureInfo.InvariantCulture));

      return list;
    }
  }

  public class DifferenceInfoList : IBindingList, IList<DifferenceInfo>
  {
    private DifferenceInfo[] m_list;
    private readonly DifferenceInfo[] m_source_list;
    private ListSortDescription m_sort;
    private readonly Dictionary<string, Dictionary<object, int>> m_indexes = new Dictionary<string, Dictionary<object, int>>();

    public DifferenceInfoList(DifferenceInfo[] list)
    {
      if (list == null)
        throw new ArgumentNullException("list");

      m_source_list = m_list = list;
    }

    public int IndexOf(DifferenceInfo item)
    {
      return Array.IndexOf(m_list, item);
    }

    public DifferenceInfo this[int index]
    {
      get
      {
        return m_list[index];
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public bool Contains(DifferenceInfo item)
    {
      return Array.IndexOf(m_list, item) >= 0;
    }

    public void CopyTo(DifferenceInfo[] array, int arrayIndex)
    {
      Array.Copy(m_list, 0, array, arrayIndex, m_list.Length);
    }

    public int Count
    {
      get { return m_list.Length; }
    }

    bool ICollection<DifferenceInfo>.IsReadOnly
    {
      get { return true; }
    }

    bool IList.IsReadOnly
    {
      get { return true; }
    }

    public IEnumerator<DifferenceInfo> GetEnumerator()
    {
      return ((IList<DifferenceInfo>)m_list).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return m_list.GetEnumerator();
    }

    bool IBindingList.AllowEdit
    {
      get { return false; }
    }

    bool IBindingList.AllowNew
    {
      get { return false; }
    }

    bool IBindingList.AllowRemove
    {
      get { return false; }
    }

    public void AddIndex(PropertyDescriptor property)
    {
      if (m_indexes.ContainsKey(property.Name))
        return;

      Dictionary<object, int> index = new Dictionary<object, int>();

      for (int i = 0; i < m_list.Length; i++)
      {
        var value = property.GetValue(m_list[i]);

        if (value != null && !index.ContainsKey(value))
          index.Add(value, i);
      }

      m_indexes.Add(property.Name, index);
    }

    public void RemoveIndex(PropertyDescriptor property)
    {
      m_indexes.Remove(property.Name);
    }

    public int Find(PropertyDescriptor property, object key)
    {
      if (key != null && m_indexes.ContainsKey(property.Name))
      {
        int result;

        if (m_indexes[property.Name].TryGetValue(key, out result))
          return result;
      }

      for (int i = 0; i < m_list.Length; i++)
      {
        var value = property.GetValue(m_list[i]);

        if (object.Equals(value, key))
          return i;
      }

      return -1;
    }

    public bool IsSorted
    {
      get { return m_sort != null; }
    }

    public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
    {
      if (direction == ListSortDirection.Ascending)
      {
        switch (property.Name)
        {
          case "Factor1":
            m_list = m_source_list.OrderBy(d => d.Factor1).ToArray();
            break;
          case "Mean1":
            m_list = m_source_list.OrderBy(d => d.Mean1).ToArray();
            break;
          case "Factor2":
            m_list = m_source_list.OrderBy(d => d.Factor2).ToArray();
            break;
          case "Mean2":
            m_list = m_source_list.OrderBy(d => d.Mean2).ToArray();
            break;
          case "Result":
            m_list = m_source_list.OrderBy(d => d.Result).ToArray();
            break;
          case "ActualDifference":
            m_list = m_source_list.OrderBy(d => d.ActualDifference).ToArray();
            break;
          case "MinimalDifference":
            m_list = m_source_list.OrderBy(d => d.MinimalDifference).ToArray();
            break;
          case "Probability":
            m_list = m_source_list.OrderBy(d => double.IsNaN(d.Probability) ? double.MaxValue : d.Probability).ToArray();
            break;
        }
      }
      else
      {
        switch (property.Name)
        {
          case "Factor1":
            m_list = m_source_list.OrderByDescending(d => d.Factor1).ToArray();
            break;
          case "Mean1":
            m_list = m_source_list.OrderByDescending(d => d.Mean1).ToArray();
            break;
          case "Factor2":
            m_list = m_source_list.OrderByDescending(d => d.Factor2).ToArray();
            break;
          case "Mean2":
            m_list = m_source_list.OrderByDescending(d => d.Mean2).ToArray();
            break;
          case "Result":
            m_list = m_source_list.OrderByDescending(d => d.Result).ToArray();
            break;
          case "ActualDifference":
            m_list = m_source_list.OrderByDescending(d => d.ActualDifference).ToArray();
            break;
          case "MinimalDifference":
            m_list = m_source_list.OrderByDescending(d => d.MinimalDifference).ToArray();
            break;
          case "Probability":
            m_list = m_source_list.OrderByDescending(d => double.IsNaN(d.Probability) ? double.MaxValue : d.Probability).ToArray();
            break;
        }
      }

      m_sort = new ListSortDescription(property, direction);

      if (this.ListChanged != null)
        this.ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    public void RemoveSort()
    {
      m_sort = null;
      m_list = m_source_list;

      if (this.ListChanged != null)
        this.ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    public ListSortDirection SortDirection
    {
      get { return m_sort != null ? m_sort.SortDirection : ListSortDirection.Ascending; }
    }

    public PropertyDescriptor SortProperty
    {
      get { return m_sort != null ? m_sort.PropertyDescriptor : null; }
    }

    public event ListChangedEventHandler ListChanged;

    public bool SupportsChangeNotification
    {
      get { return true; }
    }

    public bool SupportsSearching
    {
      get { return true; }
    }

    public bool SupportsSorting
    {
      get { return true; }
    }

    bool IList.Contains(object value)
    {
      return Array.IndexOf(m_list, value) >= 0;
    }

    int IList.IndexOf(object value)
    {
      return Array.IndexOf(m_list, value);
    }

    bool IList.IsFixedSize
    {
      get { return true; }
    }

    object System.Collections.IList.this[int index]
    {
      get
      {
        return this[index];
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public void CopyTo(Array array, int index)
    {
      Array.Copy(m_list, 0, array, index, m_list.Length);
    }

    public bool IsSynchronized
    {
      get { return false; }
    }

    public object SyncRoot
    {
      get { return this; }
    }

    void ICollection<DifferenceInfo>.Add(DifferenceInfo item)
    {
      throw new NotSupportedException();
    }

    int IList.Add(object value)
    {
      throw new NotSupportedException();
    }

    object IBindingList.AddNew()
    {
      throw new NotSupportedException();
    }

    void ICollection<DifferenceInfo>.Clear()
    {
      throw new NotSupportedException();
    }

    void IList.Clear()
    {
      throw new NotSupportedException();
    }

    void IList<DifferenceInfo>.Insert(int index, DifferenceInfo item)
    {
      throw new NotSupportedException();
    }

    void IList.Insert(int index, object value)
    {
      throw new NotSupportedException();
    }

    void IList<DifferenceInfo>.RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    void IList.RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    bool ICollection<DifferenceInfo>.Remove(DifferenceInfo item)
    {
      throw new NotSupportedException();
    }

    void IList.Remove(object value)
    {
      throw new NotSupportedException();
    }
  }
}