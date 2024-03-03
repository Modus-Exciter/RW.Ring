using System;
using System.Collections.Generic;
using System.Linq;
using static Schicksal.Optimization.MathOptimization.Domain;
using static Schicksal.Optimization.MathOptimization;
using System.Collections;

namespace Schicksal.Optimization
{
  public class OptimalSet : IEnumerable<Node>
  {
    Domain m_domain;
    Node[] m_set;
    Node m_last;
    int m_length;
    double m_tol;

    public double Tol { get { return m_tol; } set { if (value > 0) m_tol = value; } }

    public OptimalSet(Domain domain, int size, double tol)
    {
      m_set = new Node[size];
      m_domain = domain;
      m_tol = tol;
    }

    private void GetHull()
    {
      this.GetLast();
      double lastMetric = double.MaxValue;
      double metric;
      int i = 0;

      foreach (Node node in m_domain)
      {
        metric = node.Value.Peek().F / node.Value.Peek().Diag;
        if(metric <= lastMetric)
        {
          m_set[i] = node;
          metric = lastMetric;
          i++;
        }
        if(node == m_last)
          break;
      }
      m_length = i;
    }

    private void GetLast()
    {
      double minF = m_domain.Min.Value.Peek().F;
      double minMetric = double.MaxValue;
      double metric;

      foreach (Node node in m_domain)
      {
        metric = (node.Value.Peek().F - minF + m_tol * Math.Abs(minF)) / node.Value.Peek().Diag;
        if (metric <= minMetric)
        {
          minMetric = metric;
          m_last = node;
        }
      }
    }

    public IEnumerator<Node> GetEnumerator()
    {
      this.GetHull();
      for (int i = 0; i < m_length; i++)
        yield return m_set[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }

}
