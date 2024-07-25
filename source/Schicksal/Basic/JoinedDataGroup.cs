using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Вспомогательный класс для объединения нескольких выборок в одну 
  /// </summary>
  [ImmutableObject(true)]
  public sealed class JoinedDataGroup : IDataGroup
  {
    private readonly IMultyDataGroup m_group;
    private readonly int m_total_count;

    public JoinedDataGroup(IMultyDataGroup group)
    {
      if (group == null)
        throw new ArgumentNullException("group");

      m_group = group;
      m_total_count = m_group.Sum(g => g.Count);
    }

    public double this[int index]
    {
      get
      {
        int group_index = 0;

        while (index >= m_group[group_index].Count)
          index -= m_group[group_index++].Count;

        return m_group[group_index][index];
      }
    }

    public int Count
    {
      get { return m_total_count; }
    }

    public IEnumerator<double> GetEnumerator()
    {
      return m_group.SelectMany(g => g).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }

  [ImmutableObject(true)]
  public sealed class JoinedMultiDataGroup : IMultyDataGroup
  {
    private readonly ISetMultyDataGroup m_group;
    private readonly int m_total_count;

    public JoinedMultiDataGroup(ISetMultyDataGroup group)
    {
      if (group == null)
        throw new ArgumentNullException("group");

      m_group = group;
      m_total_count = m_group.Sum(g => g.Count);
    }

    public IDataGroup this[int index]
    {
      get
      {
        int group_index = 0;

        while (index >= m_group[group_index].Count)
          index -= m_group[group_index++].Count;

        return m_group[group_index][index];
      }
    }

    public int Count 
    {
      get { return m_total_count; } 
    }

    public IEnumerator<IDataGroup> GetEnumerator()
    {
      return m_group.SelectMany(g => g).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
