using Schicksal.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Schicksal.VectorField
{
  public static class NumberConvert
  {
    public static string Do(double number)
    {
      if (number == 0)
        return "0";
      if (Math.Abs(number) < 0.1 || Math.Abs(number) > 999999)
        return number.ToString("0.000e+0");
      else
        return number.ToString("0.000");
    }
  }
  /// <summary>
  /// Структура, осуществляющая векторные операции
  /// </summary>
  public struct VectorDataGroup : IDataGroup
  {
    private readonly double[] m_values;

    public double[] Values { get { return m_values; } }

    public VectorDataGroup(params double[] array)
    {
      if (array == null) throw new ArgumentNullException("array");
      m_values = array;
    }

    public VectorDataGroup(IDataGroup array)
    {
      if(array.Count == 0) throw new ArgumentNullException("array");
      m_values = array.ToArray();
    }

    public int Count { get { return m_values.Length; } }

    public double this[int index] { get { return m_values[index]; } }

    public override string ToString() { return string.Join(" ", m_values.Select(x => NumberConvert.Do(x))); }

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

    public static VectorDataGroup operator -(in VectorDataGroup a)
    {
      return new VectorDataGroup(a.m_values.Select(value => -value).ToArray());
    }

    public static VectorDataGroup operator +(in VectorDataGroup a, in VectorDataGroup b)
    {
      if (a.Count != b.Count) throw new ArgumentException("Dimensions of vectors don't agree");
      double[] res = new double[a.Count];
      for (int i = 0; i < res.Length; i++)
        res[i] = a[i] + b[i];
      return new VectorDataGroup(res);
    }

    public static VectorDataGroup operator -(in VectorDataGroup a, in VectorDataGroup b)
    {
      return a + (-b);
    }

    public static VectorDataGroup operator /(in VectorDataGroup a, double b)
    {
      return new VectorDataGroup(a.m_values.Select(value => value / b).ToArray());
    }

    public static VectorDataGroup operator *(double b, in VectorDataGroup a)
    {
      return new VectorDataGroup(a.m_values.Select(value => value * b).ToArray());
    }

    public static VectorDataGroup operator *(in VectorDataGroup a, double b) { return b * a; }

    public static VectorDataGroup operator *(in VectorDataGroup a, in VectorDataGroup b)
    {
      if (a.Count != b.Count) throw new ArgumentException("Sizes doesn't natch");
      double[] res = new double[a.Count];
      for (int i = 0; i < res.Length; i++)
        res[i] = a[i] * b[i];
      return new VectorDataGroup(res);
    }

    public static VectorDataGroup Zeros(int Dim)
    {
      return new VectorDataGroup((new double[Dim]).Select(value => value = 0).ToArray());
    }

    public static VectorDataGroup Ones(int Dim)
    {
      return new VectorDataGroup((new double[Dim]).Select(value => value = 1).ToArray());
    }

    public static VectorDataGroup Unit(int Dim, int Index)
    {
      double[] res = Zeros(Dim).ToArray();
      res[Index] = 1;
      return new VectorDataGroup(res);
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
