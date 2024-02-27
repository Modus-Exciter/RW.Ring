using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Clustering
{
  /// <summary>
  /// Метод удаления ребер из минимального остовного дерева для кластеризации
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IArcDeleter<T> where T:IComparable<T>
  {
    void DeleteArcs(Tuple<int, int, T>[] arcs);
  }

  public class DoubleSimpleArcDeleter : SimpleArcDeleter<double> { }

  public class SimpleArcDeleter<T> : IArcDeleter<T> where T : IComparable<T>
  {
    private uint m_count = 2;

    public uint ClusterCount
    {
      get { return m_count; }
      set
      {
        if (value < 2)
          throw new ArgumentOutOfRangeException("ClusterCount");

        m_count = value;
      }
    }

    public void DeleteArcs(Tuple<int, int, T>[] arcs)
    {
      if (arcs == null)
        throw new ArgumentNullException("arcs");

      bool is_desc;
      bool is_asc;

      int empty = CheckSort(arcs, out is_desc, out is_asc);

      if (is_asc)
      {
        int j = arcs.Length - 1;

        for (int i = empty; i < m_count; i++)
          arcs[j--] = null;
      }
      else if (is_desc)
      {
        int j = 0;

        for (int i = empty; i < m_count; i++)
          arcs[j++] = null;
      }
      else
      {
        Array.Sort(arcs, new TupleComparer());

        int j = arcs.Length - 1;

        for (int i = empty; i < m_count; i++)
          arcs[j--] = null;
      }
    }

    private class TupleComparer : IComparer<Tuple<int, int, T>>
    {
      public int Compare(Tuple<int, int, T> x, Tuple<int, int, T> y)
      {
        if (x == null)
        {
          if (y == null)
            return 0;
          else
            return 1;
        }
        else if (y == null)
          return -1;

        return x.Item3.CompareTo(y.Item3);
      }
    }

    private static int CheckSort(Tuple<int, int, T>[] arcs, out bool isDesc, out bool isAsc)
    {
      int empty = 0;
      isDesc = true;
      isAsc = true;

      for (int i = 1; i < arcs.Length; i++)
      {
        if (arcs[i - 1] == null)
        {
          if (i == 1)
            empty++;

          continue;
        }
        if (arcs[i] == null)
        {
          empty++;
          continue;
        }

        var cmp = arcs[i - 1].Item3.CompareTo(arcs[i].Item3);

        if (cmp < 0)
          isDesc = false;
        else if (cmp > 0)
          isAsc = false;
      }

      return empty;
    }
  }
}
