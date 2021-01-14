using System;
using System.Collections.Generic;
using System.Linq;

namespace Notung.Data
{
  /// <summary>
  /// Базовый интерфейс для работы с взвешенными графами
  /// </summary>
  /// <typeparam name="T">Тип веса дуги</typeparam>
  public interface IWeightedGraph<T> : IGraph where T : IConvertible
  {
    /// <summary>
    /// Добавление дуги
    /// </summary>
    /// <param name="from">Номер вершины, из которой исходит дуга</param>
    /// <param name="to">Номер вершины, в которую приходит дуга</param>
    /// <param name="weight">Вес добавляемой дуги</param>
    void AddArc(int from, int to, T weight);

    /// <summary>
    /// Получение веса дуги
    /// </summary>
    /// <param name="from">Номер вершины, из которой исходит дуга</param>
    /// <param name="to">Номер вершины, в которую приходит дуга</param>
    /// <returns>Вес дуги</returns>
    T this[int from, int to] { get; set; }

    /// <summary>
    /// Получение списка дуг, приходящих в указанную вершину
    /// </summary>
    /// <param name="peak">Номер вершины</param>
    /// <returns>Номера вершин, из которых исходят дуги, с весами этих дуг</returns>
    IEnumerable<Tuple<int, T>> IncomingArcs(int peak);

    /// <summary>
    /// Получение списка дуг, исходящих из указанной вершины
    /// </summary>
    /// <param name="peak">Номер вершины</param>
    /// <returns>Номера вершин, в которые приходят дуги, с весами этих дуг</returns>
    IEnumerable<Tuple<int, T>> OutgoingArcs(int peak);
  }

  /// <summary>
  /// Обёртка над взвешенным графом для возможности работы с
  /// ним алгоритмов, предназначенных для невзвешенного графа
  /// </summary>
  /// <typeparam name="T">Тип веса дуги во взвешенном графе</typeparam>
  public sealed class UnweightedWrapper<T> : IUnweightedGraph where T : IConvertible
  {
    private readonly IWeightedGraph<T> m_graph;

    public UnweightedWrapper(IWeightedGraph<T> graph)
    {
      if (graph == null)
        throw new ArgumentNullException("graph");

      m_graph = graph;
    }

    bool IUnweightedGraph.AddArc(int from, int to)
    {
      throw new NotSupportedException();
    }

    bool IGraph.RemoveArc(int from, int to)
    {
      throw new NotSupportedException();
    }

    public IEnumerable<int> IncomingArcs(int peak)
    {
      return m_graph.IncomingArcs(peak).Select(t => t.Item1);
    }

    public IEnumerable<int> OutgoingArcs(int peak)
    {
      return m_graph.OutgoingArcs(peak).Select(t => t.Item1);
    }

    public int PeakCount
    {
      get { return m_graph.PeakCount; }
    }

    public bool IsOriented
    {
      get { return m_graph.IsOriented; }
    }

    public bool HasArc(int from, int to)
    {
      return m_graph.HasArc(from, to);
    }

    public int IncomingCount(int peak)
    {
      return m_graph.IncomingCount(peak);
    }

    public int OutgoingCount(int peak)
    {
      return m_graph.OutgoingCount(peak);
    }
  }
}
