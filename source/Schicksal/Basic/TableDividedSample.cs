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
  public sealed class TableDividedSample : IDividedSample<GroupKey>, IDisposable
  {
    private readonly DataViewSample[] m_views;
    private readonly Dictionary<GroupKey, int> m_indexes;
    private readonly GroupKey[] m_keys;

    /// <summary>
    /// Инициализация новой обёртки над таблицей для статистического анализа
    /// </summary>
    /// <param name="tableParameters">Таблица, помещаемая в обёртку, с параметрами</param>
    /// <param name="sort">Колонка, по которой сортируются данные</param>
    public TableDividedSample(PredictedResponseParameters tableParameters, string sort = null)
    {
      if (tableParameters == null)
        throw new ArgumentNullException("tableParameters");

      if ((!tableParameters.Table.Columns[tableParameters.Response].DataType.IsPrimitive
        || tableParameters.Table.Columns[tableParameters.Response].DataType == typeof(bool))
        && tableParameters.Table.Columns[tableParameters.Response].DataType != typeof(decimal))
        throw new ArgumentException("Result column must be numeric");

      var sets = new HashSet<GroupKey>();
      var columnIndexes = new int[tableParameters.Predictors.Count];
      var factorColumns = tableParameters.Predictors.ToArray();
      var tuples = new List<DataViewSample>();

      for (int i = 0; i < tableParameters.Predictors.Count; i++)
        columnIndexes[i] = tableParameters.Table.Columns[factorColumns[i]].Ordinal;

      m_indexes = new Dictionary<GroupKey, int>();

      using (var filtered_table = new DataView(tableParameters.Table, tableParameters.Filter, null, DataViewRowState.CurrentRows))
      {
        foreach (DataRowView row in filtered_table)
        {
          Dictionary<string, object> values = new Dictionary<string, object>();

          for (int i = 0; i < factorColumns.Length; i++)
            values.Add(factorColumns[i], row[columnIndexes[i]]);

          var gk = new GroupKey(tableParameters, values);

          if (!sets.Add(gk))
            continue;

          var view = new DataView(tableParameters.Table, gk.ToString(), sort, DataViewRowState.CurrentRows);

          if (view.Count > 0)
          {
            tuples.Add(new DataViewSample(view, tableParameters.Response));
            m_indexes[gk] = m_indexes.Count;
          }
          else
            view.Dispose();
        }
      }

      m_views = tuples.ToArray();
      m_keys = new GroupKey[m_indexes.Count];

      foreach (var kv in m_indexes)
        m_keys[kv.Value] = kv.Key;
    }

    /// <summary>
    /// Количество подвыборок в выборке
    /// </summary>
    public int Count
    {
      get { return m_views.Length; }
    }

    /// <summary>
    /// Получение подвыборки по ключу группы
    /// </summary>
    /// <param name="rowFilter">Ключ группы</param>
    /// <returns>Подвыборка, соответствующая этому ключу</returns>
    public IPlainSample this[GroupKey rowFilter]
    {
      get { return m_views[m_indexes[rowFilter]]; }
    }

    /// <summary>
    /// Получение подвыборки по порядковому номеру
    /// </summary>
    /// <param name="index">Порядковый номер подвыборки, начиная с 0</param>
    /// <returns>Подвыборка с соответствующим порядковым номером</returns>
    public IPlainSample this[int index]
    {
      get { return m_views[index]; }
    }

    /// <summary>
    /// Получение ключа подвыборки по её порядковому номеру
    /// </summary>
    /// <param name="index">Порядковый номер подвыборки, начиная с 0</param>
    /// <returns>Ключ подвыборки</returns>
    public GroupKey GetKey(int index)
    {
      return m_keys[index];
    }

    /// <summary>
    /// Получение порядкового номера подвыборки по ключу группы
    /// </summary>
    /// <param name="rowFilter">Ключ группы, ассоциированный с подвыборкой</param>
    /// <returns>Порядковый номер подвыборки, начиная с 0</returns>
    public int GetIndex(GroupKey rowFilter)
    {
      return m_indexes[rowFilter];
    }

    /// <summary>
    /// Закрытие всех представлений, через которые читаются данные из таблицы
    /// </summary>
    public void Dispose()
    {
      foreach (DataViewSample sample in m_views)
        sample.View.Dispose();
    }

    /// <summary>
    /// Возвращает итератор, выполняющий перебор подвыборок в выборке
    /// </summary>
    /// <returns>Итератор, который можно использовать для обхода подвыборок</returns>
    public IEnumerator<IPlainSample> GetEnumerator()
    {
      return m_views.Select(v => v).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    private class DataViewSample : IPlainSample
    {
      private readonly DataView m_view;
      private readonly int m_column;
      private string m_string;

      public DataViewSample(DataView view, string column)
      {
        m_view = view;
        m_column = view.Table.Columns[column].Ordinal;
      }

      public DataView View
      {
        get { return m_view; }
      }

      public int Count
      {
        get { return m_view.Count; }
      }

      public double this[int index]
      {
        get { return Convert.ToDouble(m_view[index][m_column]); }
      }

      public IEnumerator<double> GetEnumerator()
      {
        return m_view.Cast<DataRowView>().Select(r => Convert.ToDouble(r[m_column])).GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        if (m_string == null)
        {
          m_string = string.Format("[{0}] IS NOT NULL", m_view.Table.Columns[m_column].ColumnName);

          if (m_view.RowFilter.Contains(m_string + " AND"))
            m_string += " AND";

          m_string = string.Format("Count={0}, {1}", m_view.Count, m_view.RowFilter).Replace(m_string, "");
        }

        return m_string;
      }
    }
  }
}