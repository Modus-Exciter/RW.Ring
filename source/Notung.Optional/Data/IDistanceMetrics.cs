using System;

namespace Notung.Data
{
  public interface IDistanceMetrics<T> where T : IComparable<T>
  {
    void BeginCalculation();

    void AddDifference(T from, T to);

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
  }
}
