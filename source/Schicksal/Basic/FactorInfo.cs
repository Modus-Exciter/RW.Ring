using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Notung.Data;

namespace Schicksal.Basic
{
  /// <summary>
  /// Список колонок таблицы, являющиеся факторами (предикторами)
  /// </summary>
  public sealed class FactorInfo : IEnumerable<string>
  {
    private readonly HashSet<string> m_data;
    private string m_text;

    /// <summary>
    /// Инициализация списка предикторов
    /// </summary>
    /// <param name="data">Имена колонок таблицы</param>
    public FactorInfo(IEnumerable<string> data)
    {
      m_data = new HashSet<string>(data);
    }

    /// <summary>
    /// Количество колонок таблицы, являющихся предикторами
    /// </summary>
    public int Count
    {
      get { return m_data.Count; }
    }

    /// <summary>
    /// Инициализаия итератора для обхода колонок таблицы
    /// </summary>
    public IEnumerator<string> GetEnumerator()
    {
      return m_data.GetEnumerator();
    }

    /// <summary>
    /// Инициализаия итератора для обхода колонок таблицы
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_data.GetEnumerator();
    }

    /// <summary>
    /// Строковое представление списка предикторов
    /// </summary>
    /// <returns>Имена колонок таблицы через знак +</returns>
    public override string ToString()
    {
      if (m_text == null)
        m_text = string.Join("+", m_data);

      return m_text;
    }

    /// <summary>
    /// Сравнение двух списков предикторов
    /// </summary>
    /// <param name="obj">Второй объект (предположительно того же типа)</param>
    /// <returns>True, если списки содержат одни и те же колонки. Иначе, False</returns>
    public override bool Equals(object obj)
    {
      var other = obj as FactorInfo;

      if (other == null)
        return false;

      return m_data.SetEquals(other.m_data);
    }

    /// <summary>
    /// Получение хеш-кода для списка предикторов
    /// </summary>
    /// <returns>Число, зависящее от набора строк в списке</returns>
    public override int GetHashCode()
    {
      return m_data.Aggregate(0, (i, s) => i ^ s.GetHashCode());
    }

    /// <summary>
    /// Преобразование строки в список колонок-предикторов
    /// </summary>
    /// <param name="text">Имена колонок, разделённые знаком +</param>
    /// <returns>Список колонок-предикторов</returns>
    public static FactorInfo Parse(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        return new FactorInfo(Enumerable.Empty<string>());

      return new FactorInfo(text.Split('+').Where(a => !string.IsNullOrEmpty(a)).Select(a => a.Trim()));
    }
  }
}