using System;
using System.Collections.Generic;
using System.Linq;
using Notung.Threading;

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
    ICollection<T> MandatoryDependencies { get; }

    /// <summary>
    /// Ключи объектов, от которых текущий объект зависит, но нестрого
    /// </summary>
    ICollection<T> OptionalDependencies { get; }
  }

  /// <summary>
  /// Топологический сортировщик
  /// </summary>
  /// <typeparam name="T">Тип ключа зависимости</typeparam>
  public class TopologicalSort<T>
  {
    private readonly Dictionary<T, IDependencyItem<T>> m_dependency_items;
    private readonly List<T> m_sorted_keys = new List<T>();

    /// <summary>
    /// Инициализирует экземпляр сортировщика
    /// </summary>
    /// <param name="dependencyItems">Неупорядоченный список объектов, подлежащих топологической сортировке</param>
    public TopologicalSort(ICollection<IDependencyItem<T>> dependencyItems)
    {
      if (dependencyItems == null)
        throw new ArgumentNullException("dependencyItems");

      m_dependency_items = dependencyItems.ToDictionary(a => a.Key, a => a);
    }

    /// <summary>
    /// Ключи отсортированных объектов
    /// </summary>
    public List<T> SortedKeys
    {
      get { return m_sorted_keys; }
    }

    /// <summary>
    /// Запуск сортировки объектов
    /// </summary>
    public IList<IDependencyItem<T>> Sort()
    {
      m_sorted_keys.Clear();

      var added_keys = new HashSet<T>();

      foreach (var item in m_dependency_items)
      {
        this.ProcessItem(added_keys, item.Key, true);
      }

      var cpy = m_sorted_keys.ToArray();
      m_sorted_keys.Clear();

      added_keys.Clear();

      foreach (var key in cpy)
      {
        this.CheckRecursion(key, new HashSet<T>());
        this.ProcessItem(added_keys, key, false);
      }

      return m_sorted_keys.Select(key => m_dependency_items[key]).ToList();
    }

    private void ProcessItem(HashSet<T> addedKeys, T key, bool optional)
    {
      if (addedKeys == null) throw new ArgumentNullException("addedKeys");

      if (optional && !m_dependency_items.ContainsKey(key))
        return;

      if (!addedKeys.Add(key))
        return;

      foreach (var reference in optional ? m_dependency_items[key].OptionalDependencies
        : m_dependency_items[key].MandatoryDependencies)
      {
        this.ProcessItem(addedKeys, reference, optional);
      }

      if (!m_sorted_keys.Contains(key))
        m_sorted_keys.Add(key);
    }

    private void CheckRecursion(T key, HashSet<T> chain)
    {
      if (chain == null) throw new ArgumentNullException("chain");

      if (!chain.Add(key))
        throw new ApplicationException();

      foreach (var reference in m_dependency_items[key].MandatoryDependencies)
        this.CheckRecursion(reference, new HashSet<T>(chain));
    }
  }
}
