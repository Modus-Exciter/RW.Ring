using System.Collections.Generic;

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
    bool IsOriented { get; }

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
}