using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Список колонок таблицы, являющиеся факторами (предикторами)
  /// </summary>
  public sealed class FactorInfo : IEnumerable<string>
  {
    /// <summary>
    /// Пустой список предикторов
    /// </summary>
    public static readonly FactorInfo Empty = new FactorInfo(Enumerable.Empty<string>());

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
    /// Проверка наличия предиктора в списке предикторов
    /// </summary>
    /// <param name="factor">Название колонки, являющейся предикторов</param>
    /// <returns>True, если название присутствует в списке. Иначе, False</returns>
    public bool Contains(string factor)
    {
      return m_data.Contains(factor);
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
    /// Разбиение множества предикторов на непустые подмножества
    /// </summary>
    /// <param name="includeSelf">Включать ли в список выдачи текущий набор предикторов</param>
    /// <returns>Коллекция непустых наборов предикторов</returns>
    public IEnumerable<FactorInfo> Split(bool includeSelf = true)
    {
      string[] factors = new string[m_data.Count];
      int group_count = (1 << factors.Length);

      if (!includeSelf)
        group_count--;

      m_data.CopyTo(factors);

      var details = new List<string>(this.Count);
      var result = new FactorInfo[group_count - 1];

      for (int i = 1; i < group_count; i++)
      {
        details.Clear();

        for (int j = 0; j < factors.Length; j++)
        {
          if ((i & (1 << j)) != 0)
            details.Add(factors[j]);
        }

        yield return new FactorInfo(details);
      }
    }

    /// <summary>
    /// Сравнение двух списков предикторов на равенство
    /// </summary>
    /// <param name="left">Первый список предикторов</param>
    /// <param name="right">Второй список предикторов</param>
    /// <returns>True, если списки содержат одни и те же колонки. Иначе, False</returns>
    public static bool operator ==(FactorInfo left, FactorInfo right)
    {
      if (ReferenceEquals(left, null))
        return ReferenceEquals(right, null);
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Сравнение двух списков предикторов на неравенство
    /// </summary>
    /// <param name="left">Первый список предикторов</param>
    /// <param name="right">Второй список предикторов</param>
    /// <returns>False, если списки содержат одни и те же колонки. Иначе, True</returns>
    public static bool operator !=(FactorInfo left, FactorInfo right)
    {
      if (ReferenceEquals(left, null))
        return !ReferenceEquals(right, null);
      else
        return !left.Equals(right);
    }

    /// <summary>
    /// Объединение двух списков предикторов в один
    /// </summary>
    /// <param name="left">Первый список предикторов</param>
    /// <param name="right">Второй список предикторов</param>
    /// <returns>Список, содержащий предикторы из обоих списков без повторений</returns>
    public static FactorInfo operator +(FactorInfo left, FactorInfo right)
    {
      if (left == null)
        throw new ArgumentNullException("left");

      if (right == null)
        throw new ArgumentNullException("right");

      var result = new FactorInfo(left.m_data);

      result.m_data.UnionWith(right.m_data);

      return result;
    }

    /// <summary>
    /// Удаление из одного списка предикторов содержимого второго списка
    /// </summary>
    /// <param name="left">Первый список предикторов</param>
    /// <param name="right">Второй список предикторов</param>
    /// <returns>Список, содержащий предикторы из первого списка, отсутствующие во втором списке</returns>
    public static FactorInfo operator -(FactorInfo left, FactorInfo right)
    {
      if (left == null)
        throw new ArgumentNullException("left");

      if (right == null)
        throw new ArgumentNullException("right");

      var result = new FactorInfo(left.m_data);

      result.m_data.ExceptWith(right.m_data);

      return result;
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