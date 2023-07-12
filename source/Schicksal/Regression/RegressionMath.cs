using Notung.Data;
using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Regression
{
    public class Vector
    {
        double[] values;

        public Vector(double[] array)
        {
            values = new double[array.Length];
            Array.Copy(array, values, array.Length);
        }

        public int Dim { get { return values.Length; } }

        public double this[int index] { get { return values[index]; } set { values[index] = value; } }

        public string ToString()
        {
            string str = "";
            for (int i = 0; i < Dim; i++)
                str += (" " + values[i].ToString("E3")); ;
            return str;
        }

        public void Zeros()
        {
            for (int i = 0; i < this.Dim; i++) values[i] = 0;
        }

        public static implicit operator double[](Vector a)
        {
            double[] res = new double[a.Dim];
            for (int i = 0; i < res.Length; i++)
                res[i] = a.values[i];
            return res;
        }

        public static Vector operator -(Vector a)
        {
            double[] res = new double[a.Dim];
            for (int i = 0; i < res.Length; i++)
                res[i] = -a[i];
            return new Vector(res);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Dim != b.Dim) throw new ArgumentException("Dimensions of vectors don't agree");
            double[] res = new double[a.Dim];
            for (int i = 0; i < res.Length; i++)
                res[i] = a[i] + b[i];
            return new Vector(res);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return a + (-b);
        }

        public static Vector operator /(Vector a, double b)
        {
            double[] res = new double[a.Dim];
            for (int i = 0; i < res.Length; i++)
                res[i] = a[i] / b;
            return new Vector(res);
        }

        public static Vector operator *(double b, Vector a)
        {
            double[] res = new double[a.Dim];
            for (int i = 0; i < res.Length; i++)
                res[i] = a[i] * b;
            return new Vector(res);
        }

        public static Vector operator *(Vector a, double b) => b * a;

    }

    public class Point
    {
        public static MathFunction.MultyFunction function;

        public Vector x;
        public double y;

        public Point(Vector x) { this.x = x; y = function(x); }
        
        public void ReCalculate() => y = function(x);
        
        public static int Compare(Point a, Point b)
        {
            if(a.y < b.y) return -1;
            if(a.y > b.y) return 1;
            return 0;
        }
    }

    public static class MathFunction
    {
        public delegate double Function(double x);                                                //Шаблон обычной функции
        public delegate double MultyFunction(double[] x);                                         //Шаблон функции от вектора
        public delegate double ParamFunction(double x, double[] t);                               //Шаблон параметрической 

        public static double LogisticFunction(double x, double[] t)                               //Параметрическая логистическая функция
        {
            if (t.Length != 3) throw new ArgumentException("Wrong size of parameter t");
            double power = Math.Pow(t[1], x);
            return t[0] * power / (t[2] + power);
        }

        public static double LinearFunction(double x, double[] t)                                 //Параметрическая линейная функция
        {
            if (t.Length != 2) throw new ArgumentException("Wrong size of parameter t");
            return t[0] * x + t[1];
        }

        public static double StandartVariance(IDataGroup x, IDataGroup y, Function regrFunction)    //Стандартная несмещенная дисперсия
        {
            if (x.Count != y.Count) throw new ArgumentException("Sizes of selection doesn't match");

            double res = 0;
            for (int i = 0; i < x.Count; i++)
                res += Math.Pow(y[i] - regrFunction(x[i]), 2);
            res /= (x.Count - 1);

            return Math.Sqrt(res); ;
        }
        public static double Mean(IDataGroup x)
        {
            double res = 0;
            foreach(double val in x)
                res += val;
            return res / x.Count;
        }
        public static double PlainVariance(IDataGroup y)    //Стандартная несмещенная дисперсия
        {
            double res = 0;
            double m = Mean(y);
            for (int i = 0; i < y.Count; i++)
                res += Math.Abs(y[i] - m);
            res /= Math.Sqrt(y.Count - 1);
            return res;
        }

    }

    public static class MathOptimization
    {
        public class Options
        {
            public readonly double tolX;
            public readonly double tolY;
            public readonly int maxIter;
            public Options(double tolX = 1E-12, double tolY = 1E-12, int maxIter = 1000)
            {
                this.tolX = tolX;
                this.tolY = tolY;
                this.maxIter = maxIter;
            }
        }
        public static double[] SimplexSearch(MathFunction.MultyFunction optFunction, double[] x0, Options options = null)
        {
            options = options ?? new Options();
            Point.function = optFunction;

            int n = x0.Length;
            int countIter = 0;
            double deltaY = double.MaxValue;
            double deltaX = double.MaxValue;

            List<Point> simplex = new List<Point>(n + 1);
            for (int i = 0; i < n; i++)
            {
                Vector x = new Vector(x0);
                x[i] *= 1.05;
                if (x[i] == 0) x[i] = 2.5E-4;
                simplex.Add(new Point(x));
            }
            simplex.Add(new Point(new Vector(x0)));

            while (deltaY > options.tolY && countIter < options.maxIter)
            {
                simplex.Sort(Point.Compare);

                Vector m = new Vector(new double[n]);
                m.Zeros();
                for(int i = 0; i < n; i++)
                    m += simplex[i].x;
                m /= n;

                Point r = new Point(2*m - simplex[n].x);
                
                if (r.y < simplex[n - 1].y)
                {
                    if (r.y < simplex[0].y)
                    {
                        Point s = new Point(m + 2 * (m - simplex[n].x));
                        if (s.y < r.y)
                            simplex[n] = s;
                        else
                            simplex[n] = r;
                    }
                    else
                        simplex[n] = r;
                }
                else
                {
                    if (r.y < simplex[n].y)
                        simplex[n] = r;
                    Point c = new Point(m + (simplex[n].x - m) / 2);
                    if (c.y <= simplex[n].y)
                        simplex[n] = c;
                    else
                        for (int i = 1; i < n + 1; i++)
                            simplex[i] = new Point((simplex[0].x + simplex[i].x) / 2);
                }

                double[] y = new double[n + 1];
                for (int i = 0; i < n + 1; i++)
                    y[i] = simplex[i].y;
                deltaY = Math.Abs(MathFunction.PlainVariance(new ArrayDataGroup(y)) / simplex[0].y);
                countIter++;
            }
            simplex.Sort(Point.Compare);
            return simplex[0].x;
        }
    }

    public class LikelyhoodFunction
    {
        IDataGroup x;                               //Массив факторов
        IDataGroup y;                               //Массив эффектов
        IDataGroup residual;                        //Массив невязок
        int n;                                      //Размер выборки
        MathFunction.ParamFunction regrFunction;    //Регрессируемая функция
        MathFunction.Function varFunction;          //Функция дисперсии

        public LikelyhoodFunction(IDataGroup x, IDataGroup y, MathFunction.ParamFunction regrFunction, MathFunction.Function varFunction = null)
        {
            if (x.Count != y.Count) throw new ArgumentException("Sizes of selection doesn't match");
            this.x = x; this.y = y;
            this.n = x.Count;
            this.regrFunction = regrFunction;
            this.varFunction = varFunction;
        }
        //Расчет функции правдоподобия
        public double Calculate(double[] t)
        {
            return varFunction == null ? calc(t) : calc(t, varFunction);
        }
        //Расчет значения функции правдоподобия с постоянной дисперсией
        private double calc(double[] t)
        {
            double res = 10E100;
            //double res = Math.Pow(2*Math.PI, -n/2);
            double variance = MathFunction.StandartVariance(x, y, (x) => regrFunction(x, t));
            double variance2 = variance * variance;

            for (int i = 0; i < n; i++)
            {
                res /= variance;
                res *= Math.Exp(-Math.Pow((y[i] - regrFunction(x[i], t)), 2) / (2 * variance2));
            }

            return res;
        }
        //Расчет значения функции правдоподобия с взвешенной дисперсией
        private double calc(double[] t, MathFunction.Function varFunction)
        {
            double res = 10E100;
            //double res = Math.Pow(2 * Math.PI, -n / 2);
            double standartVariance = MathFunction.StandartVariance(x, y, (x) => regrFunction(x, t));
            double variance = 0; double variance2 = 0;

            for (int i = 0; i < n; i++)
            {
                variance = varFunction(x[i]) * standartVariance;
                if (variance <= 0) variance = 0.01 * standartVariance;
                variance2 = variance * variance;

                res /= variance;
                res *= Math.Exp(-Math.Pow((y[i] - regrFunction(x[i], t)), 2) / (2 * variance2));
            }

            return res;
        }

        //Расчет относительного значения значений невязок при заданном параметре для регресионной функции
        public IDataGroup CalculateResidual(double[] t)
        {
            double[] res = new double[n];
            double variance = MathFunction.StandartVariance(x, y, (x) => regrFunction(x, t));
            for (int i = 0; i < n; i++)
                res[i] = Math.Abs(y[i] - regrFunction(x[i], t)) / variance;

            return new ArrayDataGroup(res);
        }
    }
}
