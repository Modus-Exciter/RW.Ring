using System;

namespace Schicksal.Clustering
{
  /// <summary>
  /// Метод определения расстояния между двумя точками в многомерном пространстве
  /// </summary>
  /// <typeparam name="T">Может не являться числом, является выражением отношения порядка</typeparam>
  public interface IDistanceMetrics<T> where T : IComparable<T>
  {
    /// <summary>
    /// Начало расчёта расстояния между точками
    /// </summary>
    void BeginCalculation();
    /// <summary>
    /// Расчёт расстояния по одному из измерений
    /// </summary>
    /// <param name="from">координата начальной точки по измерению</param>
    /// <param name="to">координата конечной точки по измерению</param>
    void AddDifference(T from, T to);
    /// <summary>
    /// Расчёт итогового расстояния по всем измерениям
    /// </summary>
    /// <returns>Расстояние</returns>
    T GetResult();
  }

  public class EuclidDistanceMetrics : IDistanceMetrics<double>
  {
    private double m_result = 0;

    public void BeginCalculation()
    {
      m_result = 0;
    }

    public void AddDifference(double from, double to)
    {
      m_result += (from - to) * (from - to);
    }

    public double GetResult()
    {
      return Math.Sqrt(m_result);
    }

    public override string ToString()
    {
      return SchicksalResources.EUCLIDIAN;
    }
  }

  public class ManhattanDistanceMetrics : IDistanceMetrics<double>
  {
    private double m_result = 0;

    public void BeginCalculation()
    {
      m_result = 0;
    }

    public void AddDifference(double from, double to)
    {
      m_result += Math.Abs(from - to);
    }

    public double GetResult()
    {
      return m_result;
    }

    public override string ToString()
    {
      return SchicksalResources.MANHATTAN;
    }
  }

  public class CartisDistanceMetrics : IDistanceMetrics<double>
  {
    private double m_result = 0;

    public void BeginCalculation()
    {
      m_result = 0;
    }

    public void AddDifference(double from, double to)
    {
      if (from + to != 0)
        m_result += Math.Abs((from - to) / (from + to));
    }

    public double GetResult()
    {
      return m_result;
    }

    public override string ToString()
    {
      return SchicksalResources.CARTIS;
    }
  }

  public class ChebyshevDistanceMetrics : IDistanceMetrics<double>
  {
    private double m_result = 0;

    public void BeginCalculation()
    {
      m_result = 0;
    }

    public void AddDifference(double from, double to)
    {
      if (m_result < Math.Abs(from - to))
        m_result = Math.Abs(from - to);
    }

    public double GetResult()
    {
      return m_result;
    }

    public override string ToString()
    {
      return SchicksalResources.CHEBYSHEV;
    }
  }
}
