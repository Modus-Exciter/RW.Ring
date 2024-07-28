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
  public sealed class TableDividedSample : IDividedSample<string>, IDisposable
  {
    private readonly DataTable m_table;
    private readonly DataViewSample[] m_views;
    private readonly Dictionary<string, int> m_indexes;

    /// <summary>
    /// Инициализация новой обёртки над таблицей для статистического анализа
    /// </summary>
    /// <param name="tableParameters">Таблица, помещаемая в обёртку, с параметрами</param>
    /// <param name="conjugate">Колонка, идентифицирующая сопряжённые наблюдения</param>
    public TableDividedSample(PredictedResponseParameters tableParameters, string conjugate = null)
    {
      if (tableParameters == null)
        throw new ArgumentNullException("tableParameters");
      
      if (!tableParameters.Table.Columns[tableParameters.Response].DataType.IsPrimitive
        || tableParameters.Table.Columns[tableParameters.Response].DataType == typeof(bool))
        throw new ArgumentException("Result column must be numeric");

      m_table = tableParameters.Table;

      var sets = new HashSet<string>();
      var columnIndexes = new int[tableParameters.Predictors.Count];
      var factorColumns = tableParameters.Predictors.ToArray();

      for (int i = 0; i < tableParameters.Predictors.Count; i++)
        columnIndexes[i] = m_table.Columns[factorColumns[i]].Ordinal;

      var tuples = new List<DataViewSample>();
      m_indexes = new Dictionary<string, int>();

      using (var filtered_table = new DataView(m_table, tableParameters.Filter, null, DataViewRowState.CurrentRows))
      {
        foreach (DataRowView row in filtered_table)
        {
          var sb = new StringBuilder();
          sb.AppendFormat("[{0}] is not null", tableParameters.Response);

          for (int i = 0; i < factorColumns.Length; i++)
          {
            if (row.Row.IsNull(columnIndexes[i]))
              sb.AppendFormat(" AND [{0}] IS NULL", factorColumns[i]);
            else
              sb.AppendFormat(" AND [{0}] = {1}", factorColumns[i], GetInvariant(row[columnIndexes[i]]));
          }

          if (!string.IsNullOrEmpty(tableParameters.Filter))
            sb.AppendFormat(" AND {0}", tableParameters.Filter);

          if (!sets.Add(sb.ToString()))
            continue;

          var view = new DataView(m_table, sb.ToString(), conjugate, DataViewRowState.CurrentRows);

          if (view.Count > 0)
          {
            tuples.Add(new DataViewSample(view, tableParameters.Response));
            m_indexes[view.RowFilter] = m_indexes.Count;
          }
          else
            view.Dispose();
        }
      }

      m_views = tuples.ToArray();
    }

    /// <summary>
    /// Преобразование значения в строку для выражения фильтра
    /// </summary>
    /// <param name="value">Значение</param>
    /// <returns>Строковое представление значения в тексте фильтра</returns>
    public static string GetInvariant(object value)
    {
      var formattable = value as IFormattable;

      if (value is string || value is char)
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

    public int Count
    {
      get { return m_views.Length; }
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