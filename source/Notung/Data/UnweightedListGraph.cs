using System;
using System.Collections;
using System.Collections.Generic;

namespace Notung.Data
{
  /// <summary>
  /// Невзвешенный граф, хранимый в виде списка смежных вершин.
  /// Вершины пронумерованы начиная с нуля, дуги связывают верщины по номерам
  /// </summary>
  [Serializable]
  public sealed class UnweightedListGraph : IUnweightedGraph
  {
    private readonly SetWrapper[] m_forward;
    private readonly SetWrapper[] m_reverse;

    private static readonly Func<SetWrapper> _create_set = () => new SetWrapper();

    /// <summary>
    /// Создание нового графа, хранимого в виде списка смежных вершин
    /// </summary>
    /// <param name="peakCount">Количество вершин графа</param>
    /// <param name="isOriented">Будет ли граф ориентированным</param>
    public UnweightedListGraph(int peakCount, bool isOriented)
    {
      m_forward = ArrayExtensions.CreateAndFill(peakCount, _create_set);

      if (isOriented)
        m_reverse = ArrayExtensions.CreateAndFill(peakCount, _create_set);
    }

    public int PeakCount
    {
      get { return m_forward.Length; }
    }

    public bool IsOriented
    {
      get { return m_reverse != null; }
    }

    public bool HasArc(int from, int to)
    {
      if (from == to)
        throw new ArgumentException("from == to");

      if ((uint)to >= (uint)m_forward.Length)
        throw new IndexOutOfRangeException();

      return m_forward[from].m_set.Contains(to);
    }

    public bool AddArc(int from, int to)
    {
      if (this.HasArc(from, to))
        return false;

      m_forward[from].m_set.Add(to);
      (m_reverse ?? m_forward)[to].m_set.Add(from);

      return true;
    }

    public bool RemoveArc(int from, int to)
    {
      if (!this.HasArc(from, to))
        return false;

      m_forward[from].m_set.Remove(to);
      (m_reverse ?? m_forward)[to].m_set.Remove(from);

      return true;
    }

    public int IncomingCount(int peak)
    {
      return (m_reverse ?? m_forward)[peak].m_set.Count;
    }

    public int OutgoingCount(int peak)
    {
      return m_forward[peak].m_set.Count;
    }

    public IEnumerable<int> IncomingArcs(int peak)
    {
      return (m_reverse ?? m_forward)[peak];
    }

    public IEnumerable<int> OutgoingArcs(int peak)
    {
      return m_forward[peak];
    }

    private class SetWrapper : IEnumerable<int>
    {
      public readonly HashSet<int> m_set = new HashSet<int>();

      public IEnumerator<int> GetEnumerator()
      {
        return m_set.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return m_set.GetEnumerator();
      }
    }
  }
}