using System;
using System.Collections.Generic;
using System.Linq;

namespace Notung.Data
{
  /// <summary>
  /// Невзвешенный граф, хранимый в виде списка смежных вершин
  /// </summary>
  [Serializable] 
  public sealed class UnweightedListGraph : IUnweightedGraph
  {
    private readonly HashSet<int>[] m_forward;
    private readonly HashSet<int>[] m_reverse;

    /// <summary>
    /// Создание нового графа, хранимого в виде списка смежных вершин
    /// </summary>
    /// <param name="peakCount">Количество вершин графа</param>
    /// <param name="isOriented">Будет ли граф ориентированным</param>
    public UnweightedListGraph(int peakCount, bool isOriented)
    {
      m_forward = ArrayExtensions.CreateAndFill(peakCount, () => new HashSet<int>());

      if (isOriented)
        m_reverse = ArrayExtensions.CreateAndFill(peakCount, () => new HashSet<int>());
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
      if (to < 0 || to >= m_forward.Length)
        throw new IndexOutOfRangeException();

      if (from == to)
        throw new ArgumentException("from == to");

      return m_forward[from].Contains(to);
    }

    public bool AddArc(int from, int to)
    {
      if (this.HasArc(from, to))
        return false;

      m_forward[from].Add(to);
      (m_reverse ?? m_forward)[to].Add(from);

      return true;
    }

    public bool RemoveArc(int from, int to)
    {
      if (!this.HasArc(from, to))
        return false;

      m_forward[from].Remove(to);
      (m_reverse ?? m_forward)[to].Remove(from);

      return true;
    }

    public int IncomingCount(int peak)
    {
      return (m_reverse ?? m_forward)[peak].Count;
    }

    public int OutGoingCount(int peak)
    {
      return m_forward[peak].Count;
    }

    public IEnumerable<int> IncomingArcs(int peak)
    {
      return (m_reverse ?? m_forward)[peak].Select(p => p);
    }

    public IEnumerable<int> OutgoingArcs(int peak)
    {
      return m_forward[peak].Select(p => p);
    }
  }
}