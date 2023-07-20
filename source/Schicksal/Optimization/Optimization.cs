using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
    public class OptimizationOptions
    {
        public static readonly OptimizationOptions Default = new OptimizationOptions();

        public readonly double m_tolX;
        public readonly double m_tolY;
        public readonly int m_maxIter;
        public OptimizationOptions(double tolX = 1E-12, double tolY = 1E-12, int maxIter = 1000)
        {
            this.m_tolX = tolX;
            this.m_tolY = tolY;
            this.m_maxIter = maxIter;
        }
    }

    public static class MathOptimization
    {
        private class FuncPoint : IComparable<FuncPoint>
        {
            public VectorDataGroup m_x;
            public double m_y;

            public FuncPoint(VectorDataGroup x, double y) { this.m_x = x; this.m_y = y; }
            public FuncPoint(VectorDataGroup x, Func<double[], double> function) { this.m_x = x; this.m_y = function(x); }
            public int CompareTo(FuncPoint a)
            {
                if (this.m_y < a.m_y) return -1;
                if (this.m_y > a.m_y) return 1;
                return 0;
            }
        }

        const double MINIMAL_STEP = 2.5E-4;

        private static FuncPoint[] SimplexInitialization(Func<double[], double> optFunction, double[] x0, int n, OptimizationOptions options)
        {
            FuncPoint[] simplex = new FuncPoint[n + 1];

            for (int i = 0; i < n; i++)
            {
                VectorDataGroup x = new VectorDataGroup(x0);
                x[i] *= 1.05;
                if (x[i] == 0) x[i] = MINIMAL_STEP;
                simplex[i] = new FuncPoint(x, optFunction);
            }
            simplex[n] = new FuncPoint(new VectorDataGroup(x0), optFunction);
            Array.Sort(simplex);

            return simplex;
        }
        
        public static double[] SimplexSearch(Func<double[], double> optFunction, double[] x0, OptimizationOptions options = null)
        {
            options = options ?? OptimizationOptions.Default;
            int countIter = 0;
            int n = x0.Length;
            double deltaY = double.MaxValue;
            FuncPoint[] simplex = SimplexInitialization(optFunction, x0, n, options);

            while (deltaY > options.m_tolY && countIter < options.m_maxIter)
            {
                VectorDataGroup m = new VectorDataGroup(new double[n]);
                m.Zeros();
                for (int i = 0; i < n; i++)
                    m += simplex[i].m_x;
                m /= n;

                FuncPoint r = new FuncPoint(2 * m - simplex[n].m_x, optFunction);

                if (r.m_y < simplex[n - 1].m_y)
                {
                    if (r.m_y < simplex[0].m_y)
                    {
                        FuncPoint s = new FuncPoint(m + 2 * (m - simplex[n].m_x), optFunction);
                        if (s.m_y < r.m_y)
                            simplex[n] = s;
                        else
                            simplex[n] = r;
                    }
                    else
                        simplex[n] = r;
                }
                else
                {
                    if (r.m_y < simplex[n].m_y)
                        simplex[n] = r;
                    FuncPoint c = new FuncPoint(m + (simplex[n].m_x - m) / 2, optFunction);
                    if (c.m_y <= simplex[n].m_y)
                        simplex[n] = c;
                    else
                        for (int i = 1; i < n + 1; i++)
                            simplex[i] = new FuncPoint((simplex[0].m_x + simplex[i].m_x) / 2, optFunction);
                }

                deltaY = Math.Abs(DescriptionStatistics.PlainDispersion(new ArrayDataGroup(simplex.Select(point => point.m_y).ToArray())) / simplex[0].m_y);
                Array.Sort(simplex);
                countIter++;
            }
            return simplex[0].m_x;
        }
    }
}
