﻿using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Linq;
#if DEBUG
using Schicksal.Optimization;
#endif
namespace Schicksal.Regression
{
  /// <summary>
  /// Функции для передачи в класс функции правдоподобия
  /// </summary>
  public static class MathFunction
  {
    /// <summary>
    /// Изменение знака функции
    /// </summary>
    /// <param name="func">Изменяемая параметрическая функция</param>
    /// <returns></returns>
    public static Func<double[], double> ReverseOnF(Func<double[], double> func)
    {
      return (double[] x) => -func(x);
    }
    /// <summary>
    /// Логистическая параметрическая функция
    /// </summary>
    /// <param name="x">Аргумент</param>
    /// <param name="t">Коэффициенты</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Неправильный размер массива коэффициентов</exception>
    public static double Logistic(double x, double[] t)
    {
      if (t.Length != 3) throw new ArgumentException("Wrong size of parameter t");
      double power = Math.Pow(t[1], x);
      return t[0] * power / (t[2] + power);
    }

    /// <summary>
    /// Функция параметрическая Михаэлиса
    /// </summary>
    /// <param name="x">Аргумент</param>
    /// <param name="t">Коэффициенты</param>
    /// <returns>Значение функции</returns>
    /// <exception cref="ArgumentException">Неправильный размер массива коэффициентов</exception>
    public static double Michaelis(double x, double[] t)
    {
      if (t.Length != 2) throw new ArgumentException("Wrong size of parameter t");
      return t[0] * x / (t[1] + x);
    }

    /// <summary>
    /// Линейная параметрическая функция
    /// </summary>
    /// <param name="x">Аргумент</param>
    /// <param name="t">Коэффициенты функции</param>
    /// <returns>Значение функции</returns>
    /// <exception cref="ArgumentException">Неправильный размер массива коэффициентов</exception>
    public static double Linear(double x, double[] t)
    {
      if (t.Length != 2) throw new ArgumentException("Wrong size of parameter t");
      return t[0] * x + t[1];
    }
  }
  /// <summary>
  /// Класс для построения функции правдоподобия
  /// </summary>
  public class LikelyhoodFunction
  {
    /// <summary>
    /// Минимальное значение для дисперсии
    /// </summary>
    const double MIN_VAR = 10E-3;
    /// <summary>
    /// Граница, определяющая, использовать ли константное значение дисперсии или функцию
    /// </summary>
    const double HET_THRESHOLD = 0.5;
    /// <summary>
    /// Граница, определяющая, использовать ли константное значение дисперсии или функцию
    /// </summary>
    const double SAMPLE_COUNT_THRESHOLD = 20;
    const double SAMPLE_COUNT_PER_NODE_THRESHOLD = 10;
    /// <summary>
    /// Фактор
    /// </summary>
    private readonly double[] x;
    /// <summary>
    /// Результат
    /// </summary>
    private readonly double[] y;
    /// <summary>
    /// Среднее значение точек функции дисперсии
    /// </summary>
    private double midVar;
    /// <summary>
    /// Исследуемая функция зависимости
    /// </summary>
    private readonly Func<double, double[], double> dependencyFunction;
    /// <summary>
    /// Интерфейс, содердащий актуальную функцию для расчета
    /// </summary>
    private readonly Func<double[], double> calculate;
    /// <summary>
    /// Ломанная функция дисперсии
    /// </summary>
    private PolylineFit variance;
    /// <summary>
    /// Функция расчета значения функции правдоподобия
    /// </summary>
    public Func<double[], double> Calculate { get { return calculate; } }
    /// <summary>
    /// Инициализация функции правдоподобия
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <param name="dependencyFunction">Определяемая функциональная параметрическая зависимость</param>
    /// <exception cref="ArgumentOutOfRangeException">Размер массива фактора не совпадает с размером массива результатов</exception>
    public LikelyhoodFunction(IDataGroup x, IDataGroup y, Func<double, double[], double> dependencyFunction)
    {
      if (x.Count != y.Count) throw new ArgumentOutOfRangeException();
      this.x = x.ToArray();
      this.y = y.ToArray();
      this.dependencyFunction = dependencyFunction;
      ///ADD sorting
      variance = new PolylineFit(x, Residual.Calculate(x, y, new PolylineFit(x, y).Calculate));

      if (this.IsHeteroscedascity()) 
        calculate = this.CalculateHet;
      else 
        calculate = this.CalculateDef;
    }
    /// <summary>
    /// Определение наличия гетероскедастичности
    /// </summary>
    /// <returns>Наличие гетероскедастичности</returns>
    private bool IsHeteroscedascity()
    {
      double[] varVals = variance.Nodes.Select(point => point.y).ToArray();

      if (x.Length >= SAMPLE_COUNT_THRESHOLD && x.Length / variance.Nodes.Length > SAMPLE_COUNT_PER_NODE_THRESHOLD)
      {
        midVar = varVals.Sum() / varVals.Length;
        double maxDiff = varVals.Max() - varVals.Min();

        if (maxDiff / midVar <= HET_THRESHOLD) return false;
        else return true;
      }
      else
      {
        midVar = varVals.Max();
        return false;
      }
    }
    /// <summary>
    /// Расчет функции правдоподобия с учетом гетероскедастичности для заданного параметра
    /// </summary>
    /// <param name="t">Параметры исследуемой функции</param>
    /// <returns>Значение функции правдодобия</returns>
    private double CalculateHet(double[] t)
    {
      double res = 0;
      double a, b;

      for (int i = 0; i < x.Length; i++)
      {
        a = y[i] - dependencyFunction(x[i], t);
        b = variance.Calculate(x[i]);
        if (b == 0) b = MIN_VAR;
        res += (a * a) / (2 * b * b);
      }
      if (double.IsNaN(res)) res = double.MaxValue;
      return res;
    }
    /// <summary>
    /// Расчет функции правдоподобия без учета гетероскедастичности для заданного параметра 
    /// </summary>
    /// <param name="t">Параметры исследуемой функции</param>
    /// <returns>Значение функции правдодобия</returns>
    private double CalculateDef(double[] t)
    {
      double res = 0;
      double b = 2 * midVar * midVar;
      double a;
      
      for (int i = 0; i < x.Length; i++)
      {
        a = y[i] - dependencyFunction(x[i], t);
        res += (a * a) / b;
      }

      return res;
    }

  }
}
