using System;
using System.Collections.Generic;
using System.Linq;

namespace Notung.Data
{
  /// <summary>
  /// Базовый интерфейс для работы с графами
  /// </summary>
  public interface IGraph
  {
    /// <summary>
    /// Количество вершин
    /// </summary>
    int PeakCount { get; }

    /// <summary>
    /// Ориентированный граф или не ориентированный
    /// </summary>
    bool IsOriented {get; }

    /// <summary>
    /// Проверка наличия дуги между парой вершин
    /// </summary>
    /// <param name="from">Номер вершины, из которой исходит дуга</param>
    /// <param name="to">Номер вершины, в которую приходит дуга</param>
    /// <returns>True, если такая вершина в графе есть. Иначе, false</returns>
    bool HasArc(int from, int to);

    /// <summary>
    /// Удаление вершины из графа
    /// </summary>
    /// <param name="from">Номер вершины, из которой исходит дуга</param>
    /// <param name="to">Номер вершины, в которую приходит дуга</param>
    /// <returns>True, если такая вершина была удалена. False, если вершины изначально не было</returns>
    bool RemoveArc(int from, int to);

    /// <summary>
    /// Возвращает количество дуг, приходящих в указанную вершину
    /// </summary>
    /// <param name="peak">Номер вершины</param>
    /// <returns>Количество дуг, приходящих в указанную вершину</returns>
    int IncomingCount(int peak);

    /// <summary>
    /// Возвращает количество дуг, исходящих из указанной вершины
    /// </summary>
    /// <param name="peak">Номер вершины</param>
    /// <returns>Количество дуг, исходящих из указанной вершины</returns>
    int OutgoingCount(int peak);
  }

  /// <summary>
  /// Базовый интерфейс для работы с невзвешенными графами
  /// </summary>
  public interface IUnweightedGraph : IGraph
  {
    /// <summary>
    /// Добавление дуги
    /// </summary>
    /// <param name="from">Номер вершины, из которой исходит дуга</param>
    /// <param name="to">Номер вершины, в которую приходит дуга</param>
    /// <returns>True, если дуга была добавлена. False, если такая дуга уже была</returns>
    bool AddArc(int from, int to);

    /// <summary>
    /// Получение списка дуг, приходящих в указанную вершину
    /// </summary>
    /// <param name="peak">Номер вершины</param>
    /// <returns>Номера вершин, из которых исходят дуги</returns>
    IEnumerable<int> IncomingArcs(int peak);

    /// <summary>
    /// Получение списка дуг, исходящих из указанной вершины
    /// </summary>
    /// <param name="peak">Номер вершины</param>
    /// <returns>Номера вершин, в которые приходят дуги</returns>
    IEnumerable<int> OutgoingArcs(int peak);
  }

#if WEIGHTED_GRAPH

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
    T GetWeight(int from, int to);

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

#endif
}