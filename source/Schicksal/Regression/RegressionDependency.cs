﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Notung.Data;
using Schicksal.Basic;
using Schicksal.VectorField;
using Schicksal.Properties;
using Schicksal.Optimization;
using System.Linq;

namespace Schicksal.Regression
{
  public abstract class RegressionDependency
  {
    private string m_factor = "x";
    private string m_effect = "y";

    protected RegressionDependency(IPlainSample factor, IPlainSample result)
    {
      if (factor == null)
        throw new ArgumentNullException("factor");

      if (result == null)
        throw new ArgumentNullException("result");

      if (factor.Count != result.Count)
        throw new ArgumentException(Resources.DATA_SAMPLE_SIZE_MISMATCH);
    }

    public string Factor
    {
      get { return m_factor; }
      set { m_factor = string.IsNullOrWhiteSpace(value) ? "x" : value.Trim(); }
    }

    public string Effect
    {
      get { return m_effect; }
      set { m_effect = string.IsNullOrWhiteSpace(value) ? "y" : value.Trim(); }
    }

    public Heteroscedasticity Heteroscedasticity { get; internal set; }

    public double Consistency { get; internal set; }

    public double ConsistencyWeighted { get; internal set; }

    public double RMSError { get; internal set; }

    public double RMSErrorWeighted { get; internal set; }

    public abstract double Calculate(double x);

    public virtual double[] GetGaps()
    {
      return ArrayExtensions.Empty<double>();
    }

    public static Dictionary<Type, string> GetDependencyTypeNames()
    {
      Dictionary<Type, string> types = new Dictionary<Type, string>();

      types.Add(typeof(LinearDependency), SchicksalResources.LINEAR);
      types.Add(typeof(ParabolicDependency), SchicksalResources.PARABOLIC);
      types.Add(typeof(HyperbolicDependency), SchicksalResources.HYPERBOLIC);
      //types.Add(typeof(MichaelisDependency), SchicksalResources.MICHAELIS);
      types.Add(typeof(LikehoodMichaelisDependency), SchicksalResources.MICHAELIS);
      types.Add(typeof(LogisticDependency), SchicksalResources.LOGISTIC);
      types.Add(typeof(ExponentialDependency), SchicksalResources.EXPONENT);
      return types;
    }

    protected static string ConvertNumber(double number)
    {
      if (Math.Abs(number) < 0.1 || Math.Abs(number) > 999999)
        return number.ToString("0.000e+0");
      else
        return number.ToString("0.000");
    }
  }

  public sealed class LinearDependency : RegressionDependency
  {
    public LinearDependency(IPlainSample factor, IPlainSample result) : base(factor, result)
    {
      double avg_x = DescriptionStatistics.Mean(factor);
      double avg_y = DescriptionStatistics.Mean(result);
      double sum_up = 0;
      double sum_dn = 0;

      for (int i = 0; i < factor.Count; i++)
      {
        double x = factor[i];
        double y = result[i];

        sum_up += (x - avg_x) * (y - avg_y);
        sum_dn += (x - avg_x) * (x - avg_x);
      }

      double byx = sum_up / sum_dn;
      K = byx;
      B = avg_y - byx * avg_x;
    }

    public double K { get; private set; }

    public double B { get; private set; }

    public override double Calculate(double x)
    {
      return K * x + B;
    }

    public override string ToString()
    {
      return string.Format("{0} = {1} * {2} {3} {4}",
        this.Effect, ConvertNumber(this.K), this.Factor,
        this.B >= 0 ? '+' : '-', ConvertNumber(Math.Abs(this.B)));
    }
  }

  public sealed class ParabolicDependency : RegressionDependency
  {
    public ParabolicDependency(IPlainSample factor, IPlainSample result) : base(factor, result)
    {
      RectangleMatrix<double> x_m = new RectangleMatrix<double>(factor.Count, 3);

      for (int i = 0; i < factor.Count; i++)
      {
        x_m[i, 0] = 1;
        x_m[i, 1] = factor[i];
        x_m[i, 2] = factor[i] * factor[i];
      }

      IMatrix<double> x_t = TransposedMatrix.Transpose(x_m);
      CultureInfo ci = CultureInfo.CurrentCulture;
      IMatrix<double> b_m = MatrixFunctions.Invert(x_t.Multiply(x_m, ci), ci)
        .Multiply(x_t, ci).Multiply(new SampleMatrixColumn(result), ci);

      this.A = b_m[2, 0];
      this.B = b_m[1, 0];
      this.C = b_m[0, 0];
    }

    public double A { get; private set; }

    public double B { get; private set; }

    public double C { get; private set; }

    public override double Calculate(double x)
    {
      return A * x * x + B * x + C;
    }

    public override string ToString()
    {
      return string.Format("{0} = {1} * {2}^2 {3} {4} * {2} {5} {6}",
        this.Effect, ConvertNumber(this.A), this.Factor,
        this.B >= 0 ? '+' : '-', ConvertNumber(Math.Abs(this.B)),
        this.C >= 0 ? '+' : '-', ConvertNumber(Math.Abs(this.C)));
    }
  }

  /*public sealed class MichaelisDependency : RegressionDependency
  {
    private readonly double[] m_gaps;

    public MichaelisDependency(IPLainSample factor, IPLainSample result) : base(factor, result)
    {
      double avg_x = 0;
      double avg_y = 0;
      double sum_up = 0;
      double sum_dn = 0;

      int counter = 0;

      for (int i = 0; i < factor.Count; i++)
      {
        double x = factor[i];

        if (x == 0)
          continue;

        avg_x += result[i] / x;
        avg_y += result[i];
        counter++;
      }

      avg_x /= counter;
      avg_y /= counter;

      for (int i = 0; i < factor.Count; i++)
      {
        double x = factor[i];

        if (x == 0)
          continue;

        sum_up += (result[i] / x - avg_x) * (result[i] - avg_y);
        sum_dn += (result[i] / x - avg_x) * (result[i] / x - avg_x);
      }

      double byx = sum_up / sum_dn;
      A = -byx;
      B = avg_y - byx * avg_x;

      m_gaps = new double[] { -A };
    }

    public double A { get; private set; }

    public double B { get; private set; }

    public IPLainSample Coefs { get { return new ArrayDataGroup(new double[] { B, A }); } }

    public override double[] GetGaps()
    {
      return m_gaps;
    }

    public override double Calculate(double x)
    {
      return B * x / (x + A);
    }

    public override string ToString()
    {
      return string.Format("{0} = ({1} * {2}) / ({2} {3} {4})",
        this.Effect, ConvertNumber(this.B), this.Factor,
        this.A >= 0 ? '+' : '-', ConvertNumber(Math.Abs(this.A)));
    }
  }*/

  public sealed class HyperbolicDependency : RegressionDependency
  {
    private readonly double[] m_gaps;

    public HyperbolicDependency(IPlainSample factor, IPlainSample result) : base(factor, result)
    {
      RectangleMatrix<double> x_m = new RectangleMatrix<double>(factor.Count, 4);

      for (int i = 0; i < factor.Count; i++)
      {
        x_m[i, 0] = 1;
        x_m[i, 1] = factor[i];
        x_m[i, 2] = factor[i] * factor[i];
        x_m[i, 3] = factor[i] * result[i];
      }

      IMatrix<double> x_t = TransposedMatrix.Transpose(x_m);
      CultureInfo ci = CultureInfo.CurrentCulture;
      IMatrix<double> a = MatrixFunctions.Invert(x_t.Multiply(x_m, ci), ci)
        .Multiply(x_t, ci).Multiply(new SampleMatrixColumn(result), ci);

      if (a[3, 0] == 0)
        throw new ArgumentException(Resources.IMPOSSSIBLE_DEPENDENCY);

      this.B = 1.0 / a[3, 0];
      this.C = -a[2, 0] * this.B;
      this.D = this.B * (this.C - a[1, 0]);
      this.A = this.B * (this.D - a[0, 0]);

      m_gaps = new double[] { this.B };
    }

    public double A { get; private set; }

    public double B { get; private set; }

    public double C { get; private set; }

    public double D { get; private set; }

    public override double[] GetGaps()
    {
      return m_gaps;
    }

    public override double Calculate(double x)
    {
      return A / (x - B) + C * x + D;
    }

    public override string ToString()
    {
      return string.Format("{0} = {1} / ({2} {3} {4}) {5} {6} * {2} {7} {8}",
        this.Effect, ConvertNumber(this.A), this.Factor,
        this.B > 0 ? '-' : '+', ConvertNumber(Math.Abs(this.B)),
        this.C >= 0 ? '+' : '-', ConvertNumber(Math.Abs(this.C)),
        this.D >= 0 ? '+' : '-', ConvertNumber(Math.Abs(this.D)));
    }
  }

  public sealed class ExponentialDependency : RegressionDependency
  {
    public ExponentialDependency(IPlainSample factor, IPlainSample result) : base(factor, result)
    {
      double avg_x = 0;
      double avg_y = 0;
      double sum_up = 0;
      double sum_dn = 0;

      int counter = 0;

      for (int i = 0; i < factor.Count; i++)
      {
        double y = result[i];

        if (y <= 0)
          continue;

        avg_x += factor[i];
        avg_y += Math.Log(y);

        counter++;
      }

      avg_x /= counter;
      avg_y /= counter;

      for (int i = 0; i < factor.Count; i++)
      {
        double x = factor[i];
        double y = result[i];

        if (y <= 0)
          continue;

        sum_up += (x - avg_x) * (Math.Log(y) - avg_y);
        sum_dn += (x - avg_x) * (x - avg_x);
      }

      double byx = sum_up / sum_dn;
      double k = byx;
      double d = avg_y - byx * avg_x;

      A = Math.Exp(d);
      B = Math.Exp(k);

      if (B == 1)
        throw new ArgumentException(Resources.IMPOSSSIBLE_DEPENDENCY);
    }

    public double A { get; private set; }

    public double B { get; private set; }

    public override double Calculate(double x)
    {
      return A * Math.Pow(B, x);
    }

    public override string ToString()
    {
      return string.Format("{0} = {1} * {2} ^ {3}",
        this.Effect, ConvertNumber(this.A), ConvertNumber(this.B), this.Factor);
    }
  }

  public sealed class LikehoodMichaelisDependency : RegressionDependency
  {
    const double Y_COEF = 2;
    const double X_COEF = 2;
    private readonly double[] m_param;
    private readonly PolylineFit m_variance;

    public double A { get { return m_param[0]; } }

    public double B { get { return m_param[1]; } }

    public LikehoodMichaelisDependency(IPlainSample factor, IPlainSample result) : base(factor, result)
    {
      double maxY = result.Max(); double minY = result.Min();
      double minX = factor.Min(); double maxX = factor.Max();

      VectorDataGroup lowBound = new VectorDataGroup(minY, minX);
      VectorDataGroup highBound = new VectorDataGroup(Y_COEF * maxY, X_COEF * maxX);

      LikelyhoodFunction likelyhood = new LikelyhoodFunction(factor, result, MathFunction.Michaelis);
      //m_param = MathOptimization.DIRECTSearch(regression.Calculate, lowBound, highBound);
      var optimizator = new MathOptimization.Direct(likelyhood.Calculate, lowBound.ToArray(), highBound.ToArray());
      m_param = optimizator.Process();

      IPlainSample residual = Residual.Calculate(factor, result, this.Calculate);
      m_variance = new PolylineFit(factor, residual);
    }

    public override double Calculate(double x)
    {
      return MathFunction.Michaelis(x, m_param);
    }

    public override string ToString()
    {
      return string.Format("{0} = {1} * {2} / ({3} + {2})",
        this.Effect, ConvertNumber(this.A), this.Factor, this.B);
    }
  }

  public sealed class LogisticDependency : RegressionDependency
  {
    const double MIN_BASE = 0.5;
    const double MAX_BASE = 5;
    const double Y_COEF = 2;
    const double X_COEF = 5;
    const double MAX_X = 100;
    private readonly double[] m_param;

    public double A { get { return m_param[0]; } }
    public double B { get { return m_param[1]; } }
    public double C { get { return m_param[2]; } }

    public LogisticDependency(IPlainSample factor, IPlainSample result) : base(factor, result)
    {
      double[] lowBound;
      double[] highBound;
      LikelyhoodFunction likelyhood;
      double[] x = factor.ToArray();
      double[] y = result.ToArray();
      //Ветвление масштабирования изначальной выборки и преобразования полученных резульатов
      if (x.Max() >= MAX_X)
      {
        //Масштабирование выборки
        double scaleCoef = 0;
        scaleCoef = x.Max() / MAX_X;
        x = x.Select(xi => xi / scaleCoef).ToArray();
        y = y.Select(yi => yi / scaleCoef).ToArray();

        //Определение границ
        lowBound = new double[] { y.Min(), MIN_BASE, x.Min() };
        highBound = new double[] { Y_COEF * y.Max(), MAX_BASE, X_COEF * x.Max() };

        //Инициализация функции правдоподобия и оптимизация
        likelyhood = new LikelyhoodFunction(new ArrayPlainSample(x), new ArrayPlainSample(y), MathFunction.Logistic);
        //IPLainSample tempParam = MathOptimization.DIRECTSearch(likelyhood.Calculate, lowBound, highBound);
        var optimizator = new MathOptimization.Direct(likelyhood.Calculate, lowBound.ToArray(), highBound.ToArray());
        var tempParam = optimizator.Process();
        //Преобразование коэффициентов
        m_param = new double[] {
          scaleCoef*tempParam[0],
          Math.Pow(tempParam[1], 1 / scaleCoef),
          tempParam[2]
          };
      }
      else
      {
        //Определение границ
        lowBound = new double[] { y.Min(), MIN_BASE, x.Min() };
        highBound = new double[] { Y_COEF * y.Max(), MAX_BASE, X_COEF * x.Max() };

        //Инициализация функции правдоподобия и оптимизация
        likelyhood = new LikelyhoodFunction(new ArrayPlainSample(x), new ArrayPlainSample(y), MathFunction.Logistic);
        //m_param = MathOptimization.DIRECTSearch(likelyhood.Calculate, lowBound, highBound);
        var optimizator = new MathOptimization.Direct(likelyhood.Calculate, lowBound.ToArray(), highBound.ToArray());
        m_param = optimizator.Process();
      }
    }

    public override double Calculate(double x)
    {
      return MathFunction.Logistic(x, m_param);
    }

    public override string ToString()
    {
      return string.Format("{0} = {1} * {2} ^ {3} / ({4} + {2} ^ {3})", 
        this.Effect, ConvertNumber(A), ConvertNumber(B), this.Factor, ConvertNumber(C));
    }
  }
}