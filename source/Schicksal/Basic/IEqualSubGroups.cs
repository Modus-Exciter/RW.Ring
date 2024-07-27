using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Набор выборок одинакового размера
  /// </summary>
  public interface IEqualSubGroups
  {
    /// <summary>
    /// Размер всех выборок
    /// </summary>
    int SubGroupSize { get; }
  }
  
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
        var sub = m_group as IEqualSubGroups;

        if (sub == null)
        {
          while (index >= m_group[group_index].Count)
            index -= m_group[group_index++].Count;
        }
        else
        {
          group_index = index / sub.SubGroupSize;
          index %= sub.SubGroupSize;
        }

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

    public override string ToString()
    {
      return string.Format("Joined {0}", m_group);
    }

    public override bool Equals(object obj)
    {
      var other = obj as JoinedDataGroup;

      if (other == null)
        return false;

      return m_group.Equals(other.m_group);
    }

    public override int GetHashCode()
    {
      return m_group.GetHashCode();
    }
  }

  /// <summary>
  /// Вспомогательный класс для объединения нескольких наборов выборок в один 
  /// </summary>
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
        var sub = m_group as IEqualSubGroups;

        if (sub == null)
        {
          while (index >= m_group[group_index].Count)
            index -= m_group[group_index++].Count;
        }
        else
        {
          group_index = index / sub.SubGroupSize;
          index %= sub.SubGroupSize;
        }

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

    public override string ToString()
    {
      return string.Format("Joined {0}", m_group);
    }

    public override bool Equals(object obj)
    {
      var other = obj as JoinedMultiDataGroup;

      if (other == null)
        return false;

      return m_group.Equals(other.m_group);
    }

    public override int GetHashCode()
    {
      return m_group.GetHashCode();
    }
  }
}