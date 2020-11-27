using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Properties;

namespace Notung.Data
{
  /// <summary>
  ///  Методы топологической сортировки графа
  /// </summary>
  public class TopologicalSort
  {
    /// <summary>
    /// Топологическая сортировка методом Кана
    /// </summary>
    /// <param name="graph">Граф, который требуется отсортировать</param>
    /// <returns>Массив номеров вершин, отсортированный в нужном порядке</returns>
    public static int[] Kan(IUnweightedGraph graph)
    {
      if (graph == null)
        throw new ArgumentNullException("graph");

      if (!graph.IsOriented)
        throw new ArgumentException("Graph must be oriented");

      int[] results = new int[graph.PeakCount];
      int[] arc_counts = new int[graph.PeakCount];

      int sorted_count = 0;      // Верхняя граница последнего добавленного уровня
      int last_sorted_count = 0; // Нижняя граница последнего добавленного уровня

      // Заполняем массив счётчиков связанных вершин
      for (int i = 0; i < graph.PeakCount; i++)
      {
        arc_counts[i] = graph.IncomingCount(i);

        // Если есть элементы без входящих вершин, добавляем их на первый уровень
        if (arc_counts[i] == 0)
          results[sorted_count++] = i;
      }

      // Пока не отсортирован весь граф
      while (sorted_count < results.Length)
      {
        // Сколько вершин будет обработано на новом уровне
        int addition = 0;

        // Обходим все вершины, добавленные на предыдущем уровне
        for (int i = last_sorted_count; i < sorted_count; i++)
        {
          // Обходим исходящие дуги
          foreach (var arc in graph.OutgoingArcs(results[i]))
          {
            arc_counts[arc]--;

            if (arc_counts[arc] == 0)
              results[sorted_count + addition++] = arc;
          }
        }

        // Не нашлось ни одной вершины для следующего уровня
        if (addition == 0)
          throw new ArgumentException(Resources.GRAPH_CYCLE);

        last_sorted_count = sorted_count;
        sorted_count += addition;
      }

      return results;
    }
  }
}
