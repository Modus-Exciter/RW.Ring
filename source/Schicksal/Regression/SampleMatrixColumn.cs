using Notung.Data;
using Schicksal.Basic;
using System;

namespace Schicksal.Regression
{
  class SampleMatrixColumn : IMatrix<double>
  {
    private readonly IPlainSample m_sample;

    public SampleMatrixColumn(IPlainSample sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      m_sample = sample;
    }

    public double this[int row]
    {
      get { return m_sample[row]; }
    }

    double IMatrix<double>.this[int row, int column]
    {
      get { return m_sample[row]; }
      set
      {
        throw new NotSupportedException();
      }
    }

    public int RowCount
    {
      get { return m_sample.Count; }
    }

    public int ColumnCount
    {
      get { return 1; }
    }
  }

  class SampleMatrixRow : IMatrix<double>
  {
    private readonly IPlainSample m_sample;

    public SampleMatrixRow(IPlainSample sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      m_sample = sample;
    }

    public double this[int column]
    {
      get { return m_sample[column]; }
    }

    public double this[int row, int column]
    {
      get { return m_sample[column]; }
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
      get { return m_sample.Count; }
    }
  }
}