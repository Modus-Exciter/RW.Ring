using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Schicksal.Properties;

namespace Schicksal.Anova
{
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

  public sealed class DifferenceInfoList : IBindingList, IList<DifferenceInfo>
  {
    private DifferenceInfo[] m_list;
    private readonly DifferenceInfo[] m_source_list;
    private ListSortDescription m_sort;
    private readonly Dictionary<string, Dictionary<object, int>> m_indexes 
      = new Dictionary<string, Dictionary<object, int>>();

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
      get { return m_list[index]; }
      set { throw new NotSupportedException(); }
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
      get { return this[index]; }
      set { throw new NotSupportedException(); }
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