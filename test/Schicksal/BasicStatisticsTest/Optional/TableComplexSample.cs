using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Schicksal.Basic
{
  /// <summary>
  /// Обёртка над таблицей для многофакторного анализа
  /// </summary>
  public sealed class TableComplexSample : IComplexSample, IDisposable
  {
    private readonly DataTable m_table;
    private readonly MultyViewSample[] m_views;
    private readonly Dictionary<string, int> m_indexes;

    public TableComplexSample(DataTable table, string[] factorColumns, string[] ignorableColumns, string resultColumn, string filter = null, string conjugate = null)
    {
      CheckConstrictorParameters(table, factorColumns, ignorableColumns, resultColumn);

      m_table = table;

      var tuples = new List<MultyViewSample>();
      var sets = new HashSet<string>();
      var columnIndexes = new int[factorColumns.Length];

      for (int i = 0; i < factorColumns.Length; i++)
        columnIndexes[i] = m_table.Columns[factorColumns[i]].Ordinal;

      m_indexes = new Dictionary<string, int>();

      using (var filtered_table = new DataView(table, filter, null, DataViewRowState.CurrentRows))
      {
        foreach (DataRowView row in filtered_table)
        {
          var sb = new StringBuilder();
          sb.AppendFormat("[{0}] is not null", resultColumn);

          if (!string.IsNullOrEmpty(filter))
            sb.AppendFormat(" AND {0}", filter);

          for (int i = 0; i < factorColumns.Length; i++)
          {
            if (row.Row.IsNull(columnIndexes[i]))
              sb.AppendFormat(" AND [{0}] IS NULL", factorColumns[i]);
            else
              sb.AppendFormat(" AND [{0}] = {1}", factorColumns[i], GroupKey.GetInvariant(row[columnIndexes[i]]));
          }

          if (!sets.Add(sb.ToString()))
            continue;

          var mul = new MultyViewSample(table, ignorableColumns, resultColumn, sb.ToString(), conjugate);
          tuples.Add(mul);

          for (int i = 0; i < mul.Count; i++)
            m_indexes[mul.GetKey(i)] = tuples.Count - 1;
        }
      }

      m_views = tuples.ToArray();
    }

    private static void CheckConstrictorParameters(DataTable table, string[] factorColumns, string[] ignorableColumns, string resultColumn)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      if (factorColumns == null)
        throw new ArgumentNullException("factorColumns");

      if (factorColumns.Length == 0)
        throw new ArgumentException("Factor columns list is empty");

      if (ignorableColumns == null)
        throw new ArgumentNullException("ignorableColumns");

      if (string.IsNullOrEmpty(resultColumn))
        throw new ArgumentNullException("resultColumn");

      if (!table.Columns.Contains(resultColumn))
        throw new ArgumentException("Result column not found in the table");

      if (!table.Columns[resultColumn].DataType.IsPrimitive || table.Columns[resultColumn].DataType == typeof(bool))
        throw new ArgumentException("Result column must be numeric");

      if (factorColumns.Contains(resultColumn))
        throw new ArgumentException("Result column intercects with factor columns");

      //проверка на содержание игнорируемых факторов в таблице и на НЕ содержание их же в искомых факторах
      foreach (var ign in ignorableColumns)
      {
        if (!table.Columns.Contains(ign))
          throw new ArgumentException(string.Format("Column {0} not found in the table", ign));

        if (factorColumns.Contains(ign))
          throw new ArgumentException(string.Format("Column {0} intercects with factor columns", ign));
      }

      foreach (var fc in factorColumns)
      {
        if (!table.Columns.Contains(fc))
          throw new ArgumentException(string.Format("Column {0} not found in the table", fc));
      }
    }

    public IDividedSample this[string rowFilter]
    {
      get { return m_views[m_indexes[rowFilter]]; }
    }

    public IDividedSample this[int index]
    {
      get { return m_views[index]; }
    }

    public int Count { get { return m_views.Length; } }

    public IEnumerator<IDividedSample> GetEnumerator()
    {
      return m_views.Select(v => v).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public string GetKey(int index)
    {
      return m_views[index].ToString();
    }

    public int GetIndex(string rowFilter)
    {
      return m_indexes[rowFilter];
    }

    public void Dispose()
    {
      foreach (MultyViewSample sample in m_views)
        sample.Dispose();
    }

    private class MultyViewSample : IDividedSample<string>, IDisposable
    {
      private readonly DataViewSample[] m_views;
      private readonly string m_filter;

      private readonly Dictionary<string, int> m_indexes;

      public MultyViewSample(DataTable m_table, string[] ignorableColumns, string resultColumn, string filter, string conjugate)
      {
        var ignorableIndexes = new int[ignorableColumns.Length];
        var sets = new HashSet<string>();

        for (int i = 0; i < ignorableColumns.Length; i++)
          ignorableIndexes[i] = m_table.Columns[ignorableColumns[i]].Ordinal;

        var tuples = new List<DataViewSample>();
        m_indexes = new Dictionary<string, int>();
        
        using (var filtered_table = new DataView(m_table, filter, null, DataViewRowState.CurrentRows))
        {
          foreach (DataRowView row in filtered_table)
          {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(filter))
              sb.AppendFormat("{0}", filter);

            for (int i = 0; i < ignorableColumns.Length; i++)
            {
              if (row.Row.IsNull(ignorableIndexes[i]))
                sb.AppendFormat(" AND [{0}] IS NULL", ignorableColumns[i]);
              else
                sb.AppendFormat(" AND [{0}] = {1}", ignorableColumns[i], GroupKey.GetInvariant(row[ignorableIndexes[i]]));
            }
            if (!sets.Add(sb.ToString()))
              continue;

            var view = new DataView(m_table, sb.ToString(), conjugate, DataViewRowState.CurrentRows);

            if (view.Count > 0)
            {
              tuples.Add(new DataViewSample(view, resultColumn));
              m_indexes[view.RowFilter] = m_indexes.Count;
            }
            else
              view.Dispose();
          }
        }

        m_filter = filter;
        m_views = tuples.ToArray();
      }

      public override string ToString()
      {
        return m_filter;
      }

      public IPlainSample this[string rowFilter]
      {
        get { return m_views[m_indexes[rowFilter]]; }
      }

      public IPlainSample this[int index]
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

      public int Count
      {
        get { return m_views.Length; }
      }

      public IEnumerator<IPlainSample> GetEnumerator()
      {
        return m_views.Select(v => v).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public void Dispose()
      {
        foreach (DataViewSample sample in m_views)
          sample.View.Dispose();
      }
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