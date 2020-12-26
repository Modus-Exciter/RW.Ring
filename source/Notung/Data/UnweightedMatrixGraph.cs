using System;
using System.Collections.Generic;

namespace Notung.Data
{
  /// <summary>
  /// Неориентированный граф, хранящийся в виде матрицы смежности
  /// </summary>
  [Serializable]
  public class UnweightedMatrixGraph : IUnweightedGraph
  {
    private readonly IMatrix<bool> m_matrix;
    private readonly int[] m_forward;
    private readonly int[] m_reverse;

    public UnweightedMatrixGraph(int peakCount, bool isOriented)
    {
      m_forward = new int[peakCount];

      if (isOriented)
      {
        m_matrix = new RectangleBitMatrix(peakCount);
        m_reverse = new int[peakCount];
      }
      else
        m_matrix = new TriangleBitMatrix(peakCount);
    }
    
    public int PeakCount
    {
      get { return m_matrix.RowCount; }
    }

    public bool IsOriented
    {
      get { return m_matrix is RectangleBitMatrix; }
    }

    public bool AddArc(int from, int to)
    {
      if (this.HasArc(from, to))
        return false;

      m_matrix[from, to] = true;
      m_forward[from]++;
      (m_reverse ?? m_forward)[to]++;

      return true;
    }

    public bool HasArc(int from, int to)
    {
      if (from == to)
        throw new ArgumentException("from == to");

      return m_matrix[from, to];
    }

    public bool RemoveArc(int from, int to)
    {
      if (!this.HasArc(from, to))
        return false;

      m_matrix[from, to] = false;
      m_forward[from]--;
      (m_reverse ?? m_forward)[to]--;

      return true;
    }

    public int IncomingCount(int peak)
    {
      return (m_reverse ?? m_forward)[peak];
    }

    public int OutgoingCount(int peak)
    {
      return m_forward[peak];
    }

    public IEnumerable<int> IncomingArcs(int peak)
    {
      for (int i = 0; i < m_matrix.RowCount; i++)
      {
        if (i == peak)
          continue;

        if (m_matrix[i, peak])
          yield return i;
      }
    }

    public IEnumerable<int> OutgoingArcs(int peak)
    {
      for (int i = 0; i < m_matrix.RowCount; i++)
      {
        if (i == peak)
          continue;

        if (m_matrix[peak, i])
          yield return i;
      }
    }
  }
}
