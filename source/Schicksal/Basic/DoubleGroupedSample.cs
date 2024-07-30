using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Basic
{
  public class DoubleGroupedSample : IComplexSample, IEnumerable<IDividedSample<GroupKey>>
  {
    private readonly IDividedSample<GroupKey>[] m_groups;
    private readonly GroupKey[] m_keys;

    public DoubleGroupedSample(IDividedSample<GroupKey> source, FactorInfo divider)
    {
      if (source == null) 
        throw new ArgumentNullException("source");

      if (divider == null) 
        throw new ArgumentNullException("divider");

      var dic = new Dictionary<GroupKey, List<KeyedSample>>();

      for (int i = 0; i < source.Count; i++)
      {
        var key = source.GetKey(i).GetSubKey(divider);
        var ks = new KeyedSample
        {
          Key = source.GetKey(i),
          Value = source[i]
        };

        List<KeyedSample> list;

        if (!dic.TryGetValue(key, out list))
        {
          list = new List<KeyedSample>();
          dic.Add(key, list);
        }

        list.Add(ks);
      }

      m_groups = new IDividedSample<GroupKey>[dic.Count];
      m_keys = new GroupKey[dic.Count];

      int index = 0;

      foreach (var kv in dic)
      {
        m_groups[index] = new ArrayDividedSample<GroupKey>(
          kv.Value.Select(v => v.Value).ToArray(), i => kv.Value[i].Key);

        m_keys[index] = kv.Key;

        index++;
      }
    }

    private struct KeyedSample
    {
      public GroupKey Key;
      public IPlainSample Value;
    }

    public IDividedSample this[int index]
    {
      get { return m_groups[index]; }
    }

    public int Count
    {
      get { return m_groups.Length; }
    }

    public GroupKey GetKey(int index)
    {
      return m_keys[index];
    }

    public override string ToString()
    {
      return string.Format("Grouped sample, count={0}", m_groups.Length);
    }

    public override bool Equals(object obj)
    {
      var other = obj as DoubleGroupedSample;

      if (other == null)
        return false;

      if (m_groups.Length != other.m_groups.Length)
        return false;

      if (object.ReferenceEquals(m_groups, other.m_groups)
        && object.ReferenceEquals(m_keys, other.m_keys))
        return true;

      for (int i = 0; i < m_groups.Length; i++)
      {
        if (!object.Equals(m_groups[i], other.m_groups[i]))
          return false;

        if (!object.Equals(m_keys[i], other.m_keys[i]))
          return false;
      }

      return false;
    }

    public override int GetHashCode()
    {
      return m_groups.Aggregate(0, (c, g) => c ^ g.GetHashCode()) 
        ^ m_keys.Aggregate(0, (c, g) => c ^ g.GetHashCode());
    }

    public IEnumerator<IDividedSample<GroupKey>> GetEnumerator()
    {
      return (m_groups as IEnumerable<IDividedSample<GroupKey>>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    IEnumerator<IDividedSample> IEnumerable<IDividedSample>.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
