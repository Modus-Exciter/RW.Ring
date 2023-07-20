using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using Notung.Data;
using Schicksal.Basic;
using Schicksal.Properties;
using Schicksal.Optimization;

namespace Schicksal.Regression
{
    public abstract class RegressionDependency
    {
        private string m_factor = "x";
        private string m_effect = "y";

        protected RegressionDependency(IDataGroup factor, IDataGroup result)
        {
            if (factor == null)
                throw new ArgumentNullException("factor");

            if (result == null)
                throw new ArgumentNullException("result");

            if (factor.Count != result.Count)
                throw new ArgumentException(Resources.DATA_GROUP_SIZE_MISMATCH);
        }

        public string Factor
        {
            get { return m_factor; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    m_factor = "x";
                else
                    m_factor = value.Trim();
            }
        }

        public string Effect
        {
            get { return m_effect; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    m_effect = "y";
                else
                    m_effect = value.Trim();
            }
        }

        public Heteroscedasticity Heteroscedasticity { get; internal set; }

        public double Consistency { get; internal set; }

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
            types.Add(typeof(MichaelisDependency), SchicksalResources.MICHAELIS);
            types.Add(typeof(ExponentialDependency), SchicksalResources.EXPONENT);
            types.Add(typeof(LogisticDependency), SchicksalResources.LOGISTIC);
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
        public LinearDependency(IDataGroup factor, IDataGroup result) : base(factor, result)
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
        public ParabolicDependency(IDataGroup factor, IDataGroup result) : base(factor, result)
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
              .Multiply(x_t, ci).Multiply(new DataGroupColumn(result), ci);

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

    public sealed class MichaelisDependency : RegressionDependency
    {
        private readonly double[] m_gaps;

        public MichaelisDependency(IDataGroup factor, IDataGroup result) : base(factor, result)
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
    }

    public sealed class HyperbolicDependency : RegressionDependency
    {
        private readonly double[] m_gaps;

        public HyperbolicDependency(IDataGroup factor, IDataGroup result) : base(factor, result)
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
              .Multiply(x_t, ci).Multiply(new DataGroupColumn(result), ci);

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
        public ExponentialDependency(IDataGroup factor, IDataGroup result) : base(factor, result)
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

    public sealed class LogisticDependency : RegressionDependency
    {
        private double[] param;

        public double A { get{ return param[0]; } }
        public double B { get { return param[1]; } }
        public double C { get { return param[2]; } }

        public LogisticDependency(IDataGroup factor, IDataGroup result) : base(factor, result)
        {
            double maxY = double.MinValue;
            foreach (double y in result)
                if (y > maxY)
                    maxY = y;

            double[] standartParam = new double[3] { maxY / 1.5, 2, 20 };
            double[] residualParam = new double[2] { 0, 0 };
            double[] weightedParam;
            IDataGroup residualResult;

            LikelyhoodFunction regression = new LikelyhoodFunction(factor, result, MathFunction.LogisticFunction);
            standartParam = MathOptimization.SimplexSearch((double[] t) => -regression.Calculate(t), standartParam);
            residualResult = regression.CalculateResidual(standartParam);

            LikelyhoodFunction residualRegression = new LikelyhoodFunction(factor, residualResult, MathFunction.LinearFunction);
            residualParam = MathOptimization.SimplexSearch((double[] t) => -residualRegression.Calculate(t), residualParam) ;

            LikelyhoodFunction weightedRegression = new LikelyhoodFunction(factor, result, MathFunction.LogisticFunction, (x) => MathFunction.LinearFunction(x, residualParam));
            weightedParam = MathOptimization.SimplexSearch((double[] t) => -weightedRegression.Calculate(t), standartParam);

            param = weightedParam;
        }

        public override double Calculate(double x)
        {
            return MathFunction.LogisticFunction(x, param);
        }

        public override string ToString()
        {

            return string.Format("{0} = {1} * {2} ^ {3} / ({4} + {2} ^ {3})", Effect, ConvertNumber(A), ConvertNumber(B), Factor, ConvertNumber(C));
        }
    }
}