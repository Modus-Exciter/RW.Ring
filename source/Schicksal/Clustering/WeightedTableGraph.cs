using Notung.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace Schicksal.Clustering
{
  /// <summary>
  /// Полный граф, у которого строки таблицы являются вершинами, а расстояния между строками являются ребрами
  /// </summary>
  public class WeightedTableGraph : IWeightedGraph<double>
  {
    private readonly DataTable m_table;
    private readonly int[] m_fields;
    private readonly double[] m_weights;
    private readonly IDistanceMetrics<double> m_metrics;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table">Таблица с данными для построения графа</param>
    /// <param name="fields">Какие колонки будут использоваться для расчёта расстояний</param>
    /// <param name="metrics">Метод расчёта расстояний между строками таблицы</param>
    /// <param name="weights">Вес каждой колонки таблицы при расчёте расстояний</param>
    public WeightedTableGraph(DataTable table, string[] fields, 
      IDistanceMetrics<double> metrics = null, double[] weights = null){
      if (table == null)
        throw new ArgumentNullException("table");
      if (fields == null)
        throw new ArgumentNullException("fields");
      if (weights != null && weights.Length != fields.Length)
        throw new ArgumentException();
      m_table = table;
      m_fields = new int[fields.Length];
      m_metrics = metrics ?? new EuclidDistanceMetrics();
      m_weights = weights;
      if (weights == null){
        m_weights = new double[fields.Length];
        m_weights.Fill(1);
      }
      for (int i = 0; i < fields.Length; i++)
        m_fields[i] = table.Columns[fields[i]].Ordinal;
    }

    public double this[int from, int to]{
      get{
        if (from == to)
          return 0;
        m_metrics.BeginCalculation();
        for (int i = 0; i < m_fields.Length; i++)
        {
          m_metrics.AddDifference(
            Convert.ToDouble(m_table.Rows[from][m_fields[i]]) * m_weights[i],
            Convert.ToDouble(m_table.Rows[to][m_fields[i]]) * m_weights[i]);
        }
        return m_metrics.GetResult();
      }
      set{
        throw new NotSupportedException();
      }
    }

    public IEnumerable<Tuple<int, double>> IncomingArcs(int peak)
    {
      for (int i = 0; i < m_table.Rows.Count; i++)
      {
        if (i != peak)
          yield return new Tuple<int, double>(i, this[i, peak]);
      }
    }

    public IEnumerable<Tuple<int, double>> OutgoingArcs(int peak)
    {
      for (int i = 0; i < m_table.Rows.Count; i++)
      {
        if (i != peak)
          yield return new Tuple<int, double>(i, this[peak, i]);
      }
    }

    public int PeakCount
    {
      get { return m_table.Rows.Count; }
    }

    public bool IsOriented
    {
      get { return false; }
    }

    public bool HasArc(int from, int to)
    {
      return from != to;
    }

    public int IncomingCount(int peak)
    {
      return m_table.Rows.Count - 1;
    }

    public int OutgoingCount(int peak)
    {
      return m_table.Rows.Count - 1;
    }

    void IWeightedGraph<double>.AddArc(int from, int to, double weight)
    {
      throw new NotSupportedException();
    }

    bool IGraph.RemoveArc(int from, int to)
    {
      throw new NotSupportedException();
    }
  }
}
