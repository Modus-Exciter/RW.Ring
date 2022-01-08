using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Schicksal.Basic;
using Schicksal.Properties;

namespace Schicksal.Regression
{
  class DataColumnGroup : IDataGroup
  {
    private readonly DataColumn m_column;
    private readonly DataRow[] m_rows;

    public DataColumnGroup(DataColumn column, string filter)
    {
      if (column == null)
        throw new ArgumentNullException("column");

      if (column.Table == null)
        throw new ArgumentNullException("column.Table");

      if (!column.DataType.IsPrimitive
        || column.DataType == typeof(bool)
        && column.DataType != typeof(decimal))
        throw new ArgumentException(Resources.INVALID_COLUMN_TYPE);

      m_column = column;

      if (!string.IsNullOrEmpty(filter))
        m_rows = column.Table.Select(filter);
    }

    public int Count
    {
      get
      {
        if (m_rows == null)
          return m_column.Table.Rows.Count;
        else
          return m_rows.Length;
      }
    }

    public double this[int index]
    {
      get
      {
        if (m_rows == null)
          return Convert.ToDouble(m_column.Table.Rows[index][m_column]);
        else
          return Convert.ToDouble(m_rows[index][m_column]);
      }
    }

    public IEnumerator<double> GetEnumerator()
    {
      return (m_rows ?? m_column.Table.Rows.OfType<DataRow>())
        .Select(row => Convert.ToDouble(row[m_column])).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}