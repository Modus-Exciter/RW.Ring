using System;
using System.Collections;
using System.Collections.Generic;

namespace Notung.Data
{
  /// <summary>
  /// Декартово произведение множеств
  /// </summary>
  /// <typeparam name="TKey">Тип ключа множества</typeparam>
  /// <typeparam name="TValue">Тип элемента множества</typeparam>
  public sealed class CartesianMultiplier<TKey, TValue> : IEnumerable<Dictionary<TKey, TValue>>
  {
    private readonly MultiplierEntry[] m_source;
    private readonly ulong m_count;

    public CartesianMultiplier(IDictionary<TKey, TValue[]> source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      m_source = new MultiplierEntry[source.Count];
      m_count = 1;

      int i = 0;

      foreach (var kv in source)
      {
        m_source[i++] = new MultiplierEntry
        {
          Source = kv,
          TotalCount = m_count
        };

        m_count *= (ulong)kv.Value.Length;
      }
    }

    /// <summary>
    /// Получение множества кортежей из Декартова произведения множеств
    /// </summary>
    /// <returns>Итератор, с помощью которого можно обойти кортежи произведения множеств</returns>
    public IEnumerator<Dictionary<TKey, TValue>> GetEnumerator()
    {
      var result = new Dictionary<TKey, TValue>(m_source.Length);

      for (ulong i = 0; i < m_count; i++)
      {
        result.Clear();

        foreach (var item in m_source)
        {
          result[item.Source.Key] = item.Source.Value[i / item.TotalCount %
            (ulong)item.Source.Value.Length];
        }

        yield return result;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    private struct MultiplierEntry
    {
      public KeyValuePair<TKey, TValue[]> Source;
      public ulong TotalCount;
    }
  }
}