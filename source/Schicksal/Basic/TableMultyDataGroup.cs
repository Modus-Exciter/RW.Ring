using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Schicksal.Basic
{
  /// <summary>
  /// Обёртка над таблицей данных для статистического анализа
  /// </summary>
  public sealed class TableMultyDataGroup : IMultyDataGroup<string>, IDisposable
  {
    private readonly DataTable m_table;
    private readonly DataViewGroup[] m_views;
    private readonly Dictionary<string, int> m_indexes;
        
    /// <summary>
    /// Инициализация новой обёртки над таблицей для статистического анализа
    /// </summary>
    /// <param name="table">Таблица, помещаемая в обёртку</param>
    /// <param name="factorColumns">Колонки таблицы, которые будут использоваться для выделения выборок</param>
    /// <param name="resultColumn">Колонка таблицы, которая будет использоваться для получения результатов</param>
    /// <param name="filter">Дополнительная фильтрация строк таблицы</param>
    public TableMultyDataGroup(DataTable table, string[] factorColumns, string resultColumn, string filter = null)
    {
      CheckConstructorParameters(table, factorColumns, resultColumn);

      m_table = table;

      var sets = new HashSet<string>();
      var columnIndexes = new int[factorColumns.Length];

      for (int i = 0; i < factorColumns.Length; i++)
        columnIndexes[i] = m_table.Columns[factorColumns[i]].Ordinal;

      var tuples = new List<DataViewGroup>();
      m_indexes = new Dictionary<string, int>();

      using (var filtered_table = new DataView(m_table, filter, null, DataViewRowState.CurrentRows))
      {
        foreach (DataRowView row in filtered_table)
        {
          StringBuilder sb = new StringBuilder();
          sb.AppendFormat("[{0}] is not null", resultColumn);

          for (int i = 0; i < factorColumns.Length; i++)
          {
            if (row.Row.IsNull(columnIndexes[i]))
              sb.AppendFormat(" AND [{0}] IS NULL", factorColumns[i]);
            else
              sb.AppendFormat(" AND [{0}] = {1}", factorColumns[i], this.GetInvariant(row[columnIndexes[i]]));
          }

          if (!string.IsNullOrEmpty(filter))
            sb.AppendFormat(" AND {0}", filter);

          if (!sets.Add(sb.ToString()))
            continue;

          var view = new DataView(m_table, sb.ToString(), null, DataViewRowState.CurrentRows);

          if (view.Count > 0)
          {
            tuples.Add(new DataViewGroup(view, resultColumn));
            m_indexes[view.RowFilter] = m_indexes.Count;
          }
          else
            view.Dispose();
        }
      }

      m_views = tuples.ToArray();
    }

    private string GetInvariant(object value)
    {
      var formattable = value as IFormattable;

      if (value is string)
        return string.Format("'{0}'", value);
      else if (value is DateTime)
        return string.Format("#{0}#", ((DateTime)value).ToString(CultureInfo.InvariantCulture));
      else if (formattable != null)
        return formattable.ToString(null, CultureInfo.InvariantCulture);
      else if (value != null)
        return value.ToString();
      else
        return "NULL";
    }

    private static void CheckConstructorParameters(DataTable table, string[] factorColumns, string resultColumn)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      if (factorColumns == null)
        throw new ArgumentNullException("factorColumns");

      if (factorColumns.Length == 0)
        throw new ArgumentException("Factor columns list is empty");

      if (string.IsNullOrEmpty(resultColumn))
        throw new ArgumentNullException("resultColumn");

      if (!table.Columns.Contains(resultColumn))
        throw new ArgumentException("Result column not found in the table");

      if (!table.Columns[resultColumn].DataType.IsPrimitive || table.Columns[resultColumn].DataType == typeof(bool))
        throw new ArgumentException("Result column must be numeric");

      if (factorColumns.Contains(resultColumn))
        throw new ArgumentException("Result column intercects with factor columns");

      foreach (var fc in factorColumns)
      {
        if (!table.Columns.Contains(fc))
          throw new ArgumentException(string.Format("Column {0} not found in the table", fc));
      }
    }

    public int Count
    {
      get { return m_views.Length; }
    }

    public IDataGroup this[string rowFilter]
    {
      get { return m_views[m_indexes[rowFilter]]; }
    }

    public IDataGroup this[int index]
    {
      get { return m_views[index]; }
    }

    public string GetKey(int index)
    {
      return m_views[index].View.RowFilter;
    }

    public int GetIndex(string rowFilter)
    {
      return m_indexes[rowFilter];
    }

    public IEnumerator<IDataGroup> GetEnumerator()
    {
      return m_views.Select(v => v).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public void Dispose()
    {
      foreach (DataViewGroup group in m_views)
        group.View.Dispose();
    }

    private class DataViewGroup : IDataGroup
    {
      private readonly DataView m_view;
      private readonly int m_column;
      private string m_string;

      public DataViewGroup(DataView view, string column)
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
        get { return (double)m_view[index][m_column]; }
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
          m_string = string.Format("[{0}] is not null", m_view.Table.Columns[m_column].ColumnName);

          if (m_view.RowFilter.Contains(m_string + " AND"))
            m_string += " AND";

          m_string = string.Format("Count={0}, {1}", m_view.Count, m_view.RowFilter).Replace(m_string, "");
        }

        return m_string;
      }
    }
  }
}