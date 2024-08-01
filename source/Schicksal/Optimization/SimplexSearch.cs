using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    /// <summary>
    /// Инициализация начального симплекса
    /// </summary>
    /// <param name="optFunction">Оптимизируемая функция</param>
    /// <param name="x0">Начальное приближение</param>
    /// <param name="n">Размерность задачи</param>
    /// <param name="options">Параметры оптимизации</param>
    /// <returns>Массив точек, представляющих собой изначальный симплекс</returns>
    private static FuncPoint[] SimplexInitialization(Func<VectorDataGroup, double> optFunction, VectorDataGroup x0, int n, OptimizationOptions options)
    {
      FuncPoint[] simplex = new FuncPoint[n + 1];
      const double MINIMAL_STEP = 2.5E-4;

      for (int i = 0; i < n; i++)
      {
        double[] x = x0.ToArray();
        x[i] *= 1.05;
        if (x[i] == 0) x[i] = MINIMAL_STEP;
        simplex[i] = new FuncPoint(new VectorDataGroup(x), optFunction);
      }
      simplex[n] = new FuncPoint(x0, optFunction);
      Array.Sort(simplex);

      return simplex;
    }
    /// <summary>
    /// Локальный аглоритм оптимизации с помощью симплексов
    /// </summary>
    /// <param name="optFunction">Оптимизируемая функция</param>
    /// <param name="x0">Начальное приближение</param>
    /// <param name="options">Параметры оптимизации</param>
    /// <returns>Минимум исследуемой функции</returns>
    public static VectorDataGroup SimplexSearch(Func<VectorDataGroup, double> optFunction, VectorDataGroup x0, OptimizationOptions options = null)
    {
      options = options ?? OptimizationOptions.Default;
      int countIter = 0;
      int n = x0.Count;
      double deltaY = double.MaxValue;
      double deltaX = double.MaxValue;

      FuncPoint OptFuncPoint(VectorDataGroup inputVector) => new FuncPoint(inputVector, optFunction);
      FuncPoint[] simplex = SimplexInitialization(optFunction, x0, n, options);

      while (deltaY > options.m_tolY && deltaX > options.m_tolX && countIter < options.m_maxIter)
      {
        VectorDataGroup m = VectorDataGroup.Zeros(n);

        for (int i = 0; i < n; i++)
          m += simplex[i].x;
        m /= n;

        FuncPoint r = OptFuncPoint(2 * m - simplex[n].x);

        if (r.y < simplex[n - 1].y)
        {
          if (r.y < simplex[0].y)
          {
            FuncPoint s = OptFuncPoint(m + 2 * (m - simplex[n].x));
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
          FuncPoint c = OptFuncPoint(m + (simplex[n].x - m) / 2);
          if (c.y <= simplex[n].y)
            simplex[n] = c;
          else
            for (int i = 1; i < n + 1; i++)
              simplex[i] = OptFuncPoint((simplex[0].x + simplex[i].x) / 2);
        }

        Array.Sort(simplex);
        deltaY = Math.Abs(DescriptionStatistics.PlainDispersion(new ArrayPlainSample(simplex.Select(point => point.y).ToArray())) / simplex[0].y);
        deltaX = Math.Abs((simplex[0].x - simplex[1].x).Length());
        countIter++;
      }
      return simplex[0].x;
    }
  }
}
