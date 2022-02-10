using System;
using System.Collections.Generic;
using Notung.Data;
using Schicksal.Basic;

namespace Schicksal.Regression
{
  class ListColumn : IMatrix<double>
  {
    private readonly List<double> m_list = new List<double>();

    public double this[int row, int column]
    {
      get { return m_list[row]; }
      set { m_list[row] = value; }
    }

    public int RowCount
    {
      get { return m_list.Count; }
    }

    public int ColumnCount
    {
      get { return 1; }
    }
  }
}
