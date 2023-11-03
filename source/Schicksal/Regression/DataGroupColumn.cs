using Notung.Data;
using Schicksal.Basic;
using System;

namespace Schicksal.Regression
{
  class DataGroupColumn : IMatrix<double>
  {
    private readonly IDataGroup m_group;

    public DataGroupColumn(IDataGroup group)
    {
      if (group == null)
        throw new ArgumentNullException("group");

      m_group = group;
    }

    public double this[int row]
    {
      get { return m_group[row]; }
    }

    double IMatrix<double>.this[int row, int column]
    {
      get { return m_group[row]; }
      set
      {
        throw new NotSupportedException();
      }
    }

    public int RowCount
    {
      get { return m_group.Dim; }
    }

    public int ColumnCount
    {
      get { return 1; }
    }
  }

  class DataGroupRow : IMatrix<double>
  {
    private readonly IDataGroup m_group;

    public DataGroupRow(IDataGroup group)
    {
      if (group == null)
        throw new ArgumentNullException("group");

      m_group = group;
    }

    public double this[int column]
    {
      get { return m_group[column]; }
    }

    public double this[int row, int column]
    {
      get { return m_group[column]; }
      set
      {
        throw new NotSupportedException();
      }
    }

    public int RowCount
    {
      get { return 1; }
    }

    public int ColumnCount
    {
      get { return m_group.Dim; }
    }
  }
}