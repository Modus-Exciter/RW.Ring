using Schicksal.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.VectorField
{

  public class VectorDataGroup : IDataGroup
  {
    private double[] m_values;

    public double[] Values { get { return m_values; } }

    public VectorDataGroup(double[] array)
    {
      if (array == null) throw new ArgumentNullException("array");
      m_values = new double[array.Length];
      Array.Copy(array, m_values, array.Length);
    }

    public int Dim { get { return m_values.Length; } }

    public double this[int index] { get { return m_values[index]; } set { m_values[index] = value; } }

    public override string ToString() { return string.Join(" ", m_values.Select(x => x.ToString("E3"))); }

    public static implicit operator double[](VectorDataGroup a)
    {
      return a.m_values.ToArray();
    }

    public double Length()
    {
      double result = 0;
      foreach (double value in this.m_values)
        result += value * value;
      return Math.Sqrt(result);
    }

    public VectorDataGroup Abs()
    {
      double[] result = this.ToArray();

      for (int i = 0; i < result.Length; i++)
        if (result[i] < 0)
          result[i] = -result[i];

      return new VectorDataGroup(result);
    }

    public static VectorDataGroup operator -(VectorDataGroup a)
    {
      return new VectorDataGroup(a.m_values.Select(value => -value).ToArray());
    }

    public static VectorDataGroup operator +(VectorDataGroup a, VectorDataGroup b)
    {
      if (a.Dim != b.Dim) throw new ArgumentException("Dimensions of vectors don't agree");
      double[] res = new double[a.Dim];
      for (int i = 0; i < res.Length; i++)
        res[i] = a[i] + b[i];
      return new VectorDataGroup(res);
    }

    public static VectorDataGroup operator -(VectorDataGroup a, VectorDataGroup b)
    {
      return a + (-b);
    }

    public static VectorDataGroup operator /(VectorDataGroup a, double b)
    {
      return new VectorDataGroup(a.m_values.Select(value => value / b).ToArray());
    }

    public static VectorDataGroup operator *(double b, VectorDataGroup a)
    {
      return new VectorDataGroup(a.m_values.Select(value => value * b).ToArray());
    }

    public static VectorDataGroup operator *(VectorDataGroup a, double b) { return b * a; }

    public static VectorDataGroup Zeros(int Dim)
    {
      return new VectorDataGroup((new double[Dim]).Select(value => value = 0).ToArray());
    }

    public static VectorDataGroup Unit(int Dim, int Index)
    {
      VectorDataGroup res = Zeros(Dim);
      res[Index] = 1;
      return res;
    }

    public IEnumerator<double> GetEnumerator()
    {
      return ((IList<double>)m_values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return m_values.GetEnumerator();
    }
  }
}
