using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Schicksal.Basic;
using MWDataFit;
using MathWorks.MATLAB.NET.Arrays;
using Schicksal.Regression;
using Microsoft.Office.Interop.Excel;

namespace RegressionTest
{
  class ModelComparison
  {
    const double PROB_BORDER = 0.95;

    readonly Func<double, IDataGroup, double> m_function;
    int m_size;
    IDataGroup m_coef1;
    IDataGroup m_coef2;
    IDataGroup m_x;
    IDataGroup m_y;
    IDataGroup m_weight;
    double[] m_z;
    double[] m_w;

    double m_lambda;
    double m_sl;
    double m_tquantile;
    double m_threshold;

    public double Lambda { get { return m_lambda; } }
    public double SLambda { get { return m_sl; } }
    public double TQuantile { get { return m_tquantile; } }
    public double Threshold { get { return m_threshold; } }

    public ModelComparison(Func<double, IDataGroup, double> function, IDataGroup coef1, IDataGroup coef2, IDataGroup x, IDataGroup y) 
    {
      if (coef1.Count != coef2.Count || x.Count != y.Count)
        throw new ArgumentOutOfRangeException();
      m_size = x.Count;
      m_function = function;
      m_coef1 = coef1;
      m_coef2 = coef2;
      m_x = x;
      m_y = y;
      m_z = new double[m_size];
      m_w = new double[m_size];
      this.CalculateWeights(new Dispersion(x, y, (arg) => function(arg, coef1)).Calculate);
      this.CalculateXY();
      this.CalculateLambda();
      this.CalculateS();
      this.Result();
    }

    private void CalculateXY()
    {
      double y1, y2;

      for (int i = 0; i < m_size; i++)
      {
        y1 = m_function(m_x[i], m_coef1);
        y2 = m_function(m_x[i], m_coef2);

        m_w[i] = y2 - y1;
        m_z[i] = m_y[i] - (y2 + y1) / 2;
        m_z[i] *= m_weight[i];
      }
    }

    private void CalculateLambda()
    {
      double sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0;

      for (int i = 0; i < m_size; i++)
      {
        sum1 += m_z[i] * m_w[i];
        sum2 += m_w[i];
        sum3 += m_z[i];
        sum4 += m_w[i] * m_w[i];
      }

      m_lambda = m_size * sum1 - sum2 * sum3;
      m_lambda /= m_size * sum4 - sum2 * sum2;
    }

    private void CalculateS()
    {
      double s = 0, sx = 0;
      double meanW = m_w.Average();

      for (int i = 0; i < m_size; i++)
      {
        s += (m_z[i] - m_lambda * m_w[i]) * (m_z[i] - m_lambda * m_w[i]);
        sx += (m_w[i] - meanW) * (m_w[i] - meanW);
      }
      s /= m_size - 2;
      s = Math.Sqrt(s);
      
      sx /= m_size - 1;
      sx = Math.Sqrt(sx);

      m_sl = s / sx / Math.Sqrt(m_size - 1);
    }

    private void Result()
    {
      var dataFit = new DataFit();
      m_tquantile = dataFit.TDistributionQuantile(new MWNumericArray(PROB_BORDER), new MWNumericArray(m_size - 2))
        .ToArray().Cast<double>().ToArray()[0];
      m_threshold = m_tquantile * m_sl;
    }

    private void CalculateWeights(Func<double, double> disp)
    {
      double[] weights = new double[m_size];
      double sum = 0; double coef = 0;
      for (int i = 0; i < m_size; i++)
      {
        weights[i] = 1 / disp(m_x[i]);
        sum += weights[i];
      }
      coef = 1 / sum;
      for (int i = 0; i < m_size; i++)
        weights[i] *= coef;
      m_weight = new ArrayDataGroup(weights);
    }
  }
}
