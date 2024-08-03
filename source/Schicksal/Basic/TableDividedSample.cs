using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Обёртка над таблицей данных для статистического анализа
  /// </summary>
  public sealed class TableDividedSample : IDividedSample<GroupKey>
  {
    private readonly DataViewSample[] m_samples;
    private readonly Dictionary<GroupKey, int> m_indexes;

    /// <summary>
    /// Инициализация обёртки над таблицей для статистического анализа
    /// </summary>
    /// <param name="tableParameters">Таблица с настройками отбора данных</param>
    /// <param name="sort">Колонка, по которой требуется сортировка в группах</param>
    public TableDividedSample(PredictedResponseParameters tableParameters, string sort = null)
    {
      if (tableParameters == null)
        throw new ArgumentNullException("tableParameters");

      if ((!tableParameters.Table.Columns[tableParameters.Response].DataType.IsPrimitive
        || tableParameters.Table.Columns[tableParameters.Response].DataType == typeof(bool))
        && tableParameters.Table.Columns[tableParameters.Response].DataType != typeof(decimal))
        throw new ArgumentException("Result column must be numeric");

      Dictionary<GroupKey, List<DataRow>> dic = CreateDataDictionary(tableParameters);

      if (!string.IsNullOrEmpty(sort))
      {
        var sort_col = tableParameters.Table.Columns[sort].Ordinal;

        foreach (var kv in dic)
          kv.Value.Sort((a, b) => ((IComparable)a[sort_col]).CompareTo(b[sort_col]));
      }

      m_samples = new DataViewSample[dic.Count];
      m_indexes = new Dictionary<GroupKey, int>();

      int index = 0;

      foreach (var kv in dic)
      {
        m_samples[index] = new DataViewSample(kv.Value.ToArray(), tableParameters.Response, kv.Key);
        m_indexes[kv.Key] = index++;
      }
    }

    /// <summary>
    /// Получение подвыборки данных по ключу
    /// </summary>
    /// <param name="key">Ключ со значениями колонок для отбора</param>
    /// <returns>Подвыборка строк таблицы, соответствующих ключу</returns>
    public IPlainSample this[GroupKey key]
    {
      get { return m_samples[m_indexes[key]]; }
    }

    /// <summary>
    /// Получение подвыборки данных по индексу
    /// </summary>
    /// <param name="index">Индекс подвыборки (порядковый номер, начиная с 0)</param>
    /// <returns>Подвыборка данных по индексу</returns>
    public IPlainSample this[int index]
    {
      get { return m_samples[index]; }
    }

    /// <summary>
    /// Общее количество подвыборок в выборке
    /// </summary>
    public int Count
    {
      get { return m_samples.Length; }
    }

    /// <summary>
    /// Получение индекса подвыборки по ключу
    /// </summary>
    /// <param name="key">Ключ со значениями колонок для отбора</param>
    /// <returns>Индекс подвыборки (порядковый номер, начиная с 0)</returns>
    public int GetIndex(GroupKey key)
    {
      return m_indexes.TryGetValue(key, out int index) ? index : -1;
    }

    /// <summary>
    /// Получение ключа подвыборки по её индексу
    /// </summary>
    /// <param name="index">Индекс подвыборки (порядковый номер, начиная с 0)</param>
    /// <returns>Ключ со значениями колонок для отбора</returns>
    public GroupKey GetKey(int index)
    {
      return m_samples[index].Key;
    }

    /// <summary>
    /// Получение итератора для обхода подвыборок
    /// </summary>
    /// <returns>Итератор, позволяющий перебрать все подвыборки выборки</returns>
    public IEnumerator<IPlainSample> GetEnumerator()
    {
      return (m_samples as IList<IPlainSample>).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #region Implementation ------------------------------------------------------------------------

    private static Dictionary<GroupKey, List<DataRow>> CreateDataDictionary(PredictedResponseParameters tableParameters)
    {
      var dic = new Dictionary<GroupKey, List<DataRow>>();

      foreach (var row in tableParameters.Table.Select(tableParameters.GetFullFilter()))
      {
        var gk = new GroupKey(tableParameters, row);
        List<DataRow> list;

        if (!dic.TryGetValue(gk, out list))
        {
          list = new List<DataRow>();
          dic.Add(gk, list);
        }

        list.Add(row);
      }

      return dic;
    }

    private class DataViewSample : IPlainSample
    {
      private readonly DataRow[] m_rows;
      private readonly int m_column;
      private readonly GroupKey m_key;

      public DataViewSample(DataRow[] rows, string column, GroupKey key)
      {
        m_rows = rows;
        m_column = rows[0].Table.Columns[column].Ordinal;
        m_key = key;
      }

      public double this[int index]
      {
        get { return Convert.ToDouble(m_rows[index][m_column]); }
      }

      public int Count
      {
        get { return m_rows.Length; }
      }

      public GroupKey Key
      {
        get { return m_key; }
      }

      public IEnumerator<double> GetEnumerator()
      {
        return m_rows.Select(r => Convert.ToDouble(r[m_column])).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return m_key.Query;
      }

      public override bool Equals(object obj)
      {
        var other = obj as DataViewSample;

        if (other == null || ReferenceEquals(this, obj))
          return other != null;

        return m_column == other.m_column && ReferenceEquals(m_rows, other.m_rows) && m_key.Equals(other.m_key);
      }

      public override int GetHashCode()
      {
        return m_column ^ m_key.GetHashCode() ^ m_rows.GetHashCode();
      }
    }

    #endregion
  }
}