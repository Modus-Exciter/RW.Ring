using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Notung.Data;
using Notung.Properties;

namespace Notung.Loader
{
  /// <summary>
  /// Объект в иерархии зависимостей
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
    /// <param name="useMatrix">Хранить граф в виде матрицы смежности</param>
    /// <returns>Взвешенный граф, описывающий структуру зависимостей</returns>
    public static IUnweightedGraph ToUnweightedGraph<T>(this IList<IDependencyItem<T>> dependencyItems)
    {
      var converter = new Dictionary<T, int>(dependencyItems.Count);

      for (int i = 0; i < dependencyItems.Count; i++)
        converter.Add(dependencyItems[i].Key, i);

      IUnweightedGraph graph;

      graph = new UnweightedListGraph(converter.Count, true);

      for (int i = 0; i < dependencyItems.Count; i++)
      {
        foreach (var key in dependencyItems[i].Dependencies)
          graph.AddArc(converter[key], i);
      }

      return graph;
    }

    /// <summary>
    /// Коррекция списка зависимостей: удаление дубликатов ключей очистка от неразрешённых зависимостей
    /// </summary>
    /// <typeparam name="T">Тип ключа для определения зависимостей</typeparam>
    /// <typeparam name="TItem">Тип зависимости</typeparam>
    /// <param name="dependencyItems">Список объектов, зависящих друг от друга</param>
    public static void Fix<T, TItem>(this IList<TItem> dependencyItems) where TItem : IDependencyItem<T>
    {
      var collection = new Dictionary<T, TItem>(dependencyItems.Count);
      var numbers = new Dictionary<T, int>(dependencyItems.Count);
      var duplicates = new Dictionary<T, List<TItem>>();

      bool impossible_fix;

      if (dependencyItems is IList)
        impossible_fix = ((IList)dependencyItems).IsFixedSize;
      else
        impossible_fix = dependencyItems.IsReadOnly;

      foreach (var item in dependencyItems)
      {
        if (item == null || item.Key == null)
        {
          if (impossible_fix)
            throw new ArgumentException(Resources.IMPOSSIBLE_FIX);

          continue;
        }

        if (collection.ContainsKey(item.Key))
        {
          if (impossible_fix)
            throw new ArgumentException(Resources.IMPOSSIBLE_FIX);

          List<TItem> list;

          if (!duplicates.TryGetValue(item.Key, out list))
          {
            list = new List<TItem> { collection[item.Key] };
            duplicates.Add(item.Key, list);
          }

          list.Add(item);
        }
        else
        {
          collection[item.Key] = item;
          numbers[item.Key] = numbers.Count;
        }
      }

      Func<TItem, bool> predicate = item => item.Dependencies.All(collection.ContainsKey);
      Func<TItem, int> order_selector = item => item.Dependencies.Count;

      foreach (var kv in duplicates)
      {
        var best = kv.Value.Where(predicate).OrderBy(order_selector).FirstOrDefault();

        if (best != null)
          collection[kv.Key] = best;
        else
          throw new ArgumentException(Resources.BAD_DUPLICATES);
      }

      while (dependencyItems.Count > numbers.Count)
        dependencyItems.RemoveAt(dependencyItems.Count - 1);

      foreach (var kv in numbers)
        dependencyItems[kv.Value] = collection[kv.Key];
    }
  }
}
