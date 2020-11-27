using System.Collections.Generic;
using Notung.Data;

namespace Notung.Loader
{
  /// <summary>
  /// Объект, поддерживающий топологическую сортировку по зависимостям
  /// </summary>
  /// <typeparam name="T">Тип ключа зависимости</typeparam>
  public interface IDependencyItem<T>
  {
    /// <summary>
    /// Ключ текущего объекта
    /// </summary>
    T Key { get; }

    /// <summary>
    /// Ключи объектов, от которых текущий объект зависит обязательно
    /// </summary>
    ICollection<T> Dependencies { get; }
  }

  public static class DependencyItemExtensions
  {
    /// <summary>
    /// Преобразование списка объектов с зависимостями в ориентированный невзвешенный граф
    /// </summary>
    /// <typeparam name="T">Тип ключа для определения зависимостей</typeparam>
    /// <param name="dependencyItems">Список объектов, зависящих друг от друга</param>
    /// <returns>Взвешенный граф, описывающий структуру зависимостей</returns>
    public static IUnweightedGraph ToUnweightedGraph<T>(this IList<IDependencyItem<T>> dependencyItems)
    {
      Dictionary<T, int> converter = new Dictionary<T, int>();

      for (int i = 0; i < dependencyItems.Count; i++)
        converter.Add(dependencyItems[i].Key, i);

      IUnweightedGraph graph = new UnweightedNestedList(converter.Count, true);

      for (int i = 0; i < dependencyItems.Count; i++)
      {
        foreach (var key in dependencyItems[i].Dependencies)
          graph.AddArc(converter[key], i);
      }

      return graph;
    }
  }
}
