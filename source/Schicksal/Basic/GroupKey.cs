using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Schicksal.Basic
{
  public sealed class GroupKey : IEnumerable<KeyValuePair<string, object>>
  {
    private readonly Dictionary<string, object> m_data;
    private readonly string m_base_filter;
    private readonly string m_query;
    private readonly string m_response;

    private FactorInfo m_factor_info;

    private static readonly Func<KeyValuePair<string, object>, KeyValuePair<string, object>> _omit_nulls =
      kv => new KeyValuePair<string, object>(kv.Key, OmitNulls(kv.Value));

    public GroupKey(PredictedResponseParameters parameters, Dictionary<string, object> data)
    {
      if (parameters is null) 
        throw new ArgumentNullException("parameters");

      if (data is null) 
        throw new ArgumentNullException("data");

      foreach (var kv in data)
      {
        if (!parameters.Table.Columns.Contains(kv.Key))
          throw new ArgumentException(string.Format("Column {0} not found in the table", kv.Key));
      }

      m_base_filter = parameters.Filter;
      m_response = parameters.Response;
      m_data = data;
      m_query = this.GetQueryText();
    }

    private GroupKey(Dictionary<string, object> data, string baseFilter, string response)
    {
      m_data = data;
      m_base_filter = baseFilter;
      m_response = response;
      m_query = this.GetQueryText();
    }

    public object this[string column]
    {
      get { return OmitNulls(m_data[column]); }
    }

    public string BaseFilter
    {
      get { return m_base_filter; }
    }

    public string Response
    {
      get { return m_response; }
    }

    public string Query
    {
      get { return m_query; }
    }

    public int Count
    {
      get { return m_data.Count; }
    }

    public FactorInfo FactorInfo
    {
      get
      {
        if (m_factor_info == null)
          m_factor_info = new FactorInfo(m_data.Keys);

        return m_factor_info;
      }
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return m_data.Select(_omit_nulls).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public override bool Equals(object obj)
    {
      var other = obj as GroupKey;

      if (other == null)
        return false;

      if (!object.Equals(m_base_filter, other.m_base_filter))
        return false;

      if (m_data.Count != other.m_data.Count)
        return false;

      foreach (var kv in m_data)
      {
        object value;

        if (!other.m_data.TryGetValue(kv.Key, out value))
          return false;

        if (!object.Equals(OmitNulls(kv.Value), OmitNulls(value)))
          return false;
      }

      return true;
    }

    public override string ToString()
    {
      return m_query;
    }

    public override int GetHashCode()
    {
      var ret = m_base_filter == null ? 0 : m_base_filter.GetHashCode();

      foreach (var kv in m_data)
        ret ^= (kv.Key.GetHashCode() ^ OmitNulls(kv.Value).GetHashCode());

      return ret;
    }

    public static string GetInvariant(object value)
    {
      var formattable = value as IFormattable;

      if (value is string || value is char)
        return string.Format("'{0}'", value);
      else if (value is DateTime)
        return string.Format("#{0}#", ((DateTime)value).ToString(CultureInfo.InvariantCulture));
      else if (value is DBNull)
        return "NULL";
      else if (formattable != null)
        return formattable.ToString(null, CultureInfo.InvariantCulture);
      else if (value != null)
        return value.ToString();
      else
        return "NULL";
    }

    public static IDividedSample<GroupKey> Repack(IDividedSample<GroupKey> sample, FactorInfo factor)
    {
      if (factor is null)
        throw new ArgumentNullException("factor");

      if (sample is null)
        throw new ArgumentNullException("sample");

      var first_key = sample.GetKey(0);

      if (factor.Any(p => !first_key.m_data.ContainsKey(p)))
        throw new ArgumentException();

      if (sample.Count == 0)
        return sample;

      if (factor.Count == first_key.FactorInfo.Count)
        return sample;

      Dictionary<GroupKey, List<double>> tmp = CreateRepackData(sample, factor);

      GroupKey[] keys = new GroupKey[tmp.Count];
      double[][] values = new double[tmp.Count][];
      int index = 0;

      foreach (var kv in tmp)
      {
        keys[index] = kv.Key;
        values[index++] = kv.Value.ToArray();
      }

      return new ArrayDividedSample<GroupKey>(values, keys);
    }

    private static Dictionary<GroupKey, List<double>> CreateRepackData(IDividedSample<GroupKey> sample, FactorInfo factor)
    {
      var tmp = new Dictionary<GroupKey, List<double>>();

      for (int i = 0; i < sample.Count; i++)
      {
        var key = sample.GetKey(i);
        var dic = new Dictionary<string, object>();

        foreach (var p in factor)
          dic[p] = key.m_data[p];

        var new_key = new GroupKey(dic, key.BaseFilter, key.Response);
        List<double> list;

        if (tmp.TryGetValue(new_key, out list))
          list.AddRange(sample[i]);
        else
          tmp.Add(new_key, sample[i].ToList());
      }

      return tmp;
    }

    private string GetQueryText()
    {
      var sb = new StringBuilder();

      sb.AppendFormat("[{0}] IS NOT NULL", m_response);

      foreach (var kv in m_data)
      {
        if (OmitNulls(kv.Value) is DBNull)
          sb.AppendFormat(" AND [{0}] IS NULL", kv.Key);
        else
          sb.AppendFormat(" AND [{0}] = {1}", kv.Key, GetInvariant(kv.Value));
      }

      if (!string.IsNullOrWhiteSpace(m_base_filter))
        sb.AppendFormat(" AND {0}", m_base_filter);

      return sb.ToString();
    }

    private static object OmitNulls(object value)
    {
      return value == null ? Convert.DBNull : value;
    }
  }
}
