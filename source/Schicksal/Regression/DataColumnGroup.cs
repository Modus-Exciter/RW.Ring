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

    public DataColumnGroup(DataColumn column)
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
    }
    
    public int Count
    {
      get { return m_column.Table.Rows.Count; }
    }

    public double this[int index]
    {
      get { return Convert.ToDouble(m_column.Table.Rows[index][m_column]); }
    }

    public IEnumerator<double> GetEnumerator()
    {
      return m_column.Table.Rows.OfType<DataRow>().Select(row => Convert.ToDouble(row[m_column])).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
