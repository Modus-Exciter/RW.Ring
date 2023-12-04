using Notung.Data;
using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Security.AccessControl;
using System.Runtime.InteropServices.ComTypes;
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
    public static Func<VectorDataGroup, double> ReverseOnF(Func<VectorDataGroup, double> func)
    {
      return (VectorDataGroup x) => -func(x);
    }
    /// <summary>
    /// Логистическая параметрическая функция
    /// </summary>
    /// <param name="x">Аргумент</param>
    /// <param name="t">Коэффициенты</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Неправильный размер массива коэффициентов</exception>
    public static double Logistic(double x, IDataGroup t)
    {
      if (t.Count != 3) throw new ArgumentException("Wrong size of parameter t");
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
    public static double Michaelis(double x, IDataGroup t)
    {
      if (t.Count != 2) throw new ArgumentException("Wrong size of parameter t");
      return t[0] * x / (t[1] + x);
    }

    /// <summary>
    /// Линейная параметрическая функция
    /// </summary>
    /// <param name="x">Аргумент</param>
    /// <param name="t">Коэффициенты функции</param>
    /// <returns>Значение функции</returns>
    /// <exception cref="ArgumentException">Неправильный размер массива коэффициентов</exception>
    public static double Linear(double x, IDataGroup t)
    {
      if (t.Count != 2) throw new ArgumentException("Wrong size of parameter t");
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
    private readonly IDataGroup x;
    /// <summary>
    /// Результат
    /// </summary>
    private readonly IDataGroup y;
    /// <summary>
    /// Среднее значение точек функции дисперсии
    /// </summary>
    private double midVar;
    /// <summary>
    /// Исследуемая функция зависимости
    /// </summary>
    private readonly Func<double, IDataGroup, double> dependencyFunction;
    /// <summary>
    /// Интерфейс, содердащий актуальную функцию для расчета
    /// </summary>
    private readonly Func<IDataGroup, double> calculate;
    /// <summary>
    /// Ломанная функция дисперсии
    /// </summary>
    private PolylineFit variance;
    /// <summary>
    /// Функция расчета значения функции правдоподобия
    /// </summary>
    public Func<IDataGroup, double> Calculate { get { return calculate; } }
    /// <summary>
    /// Инициализация функции правдоподобия
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <param name="dependencyFunction">Определяемая функциональная параметрическая зависимость</param>
    /// <exception cref="ArgumentOutOfRangeException">Размер массива фактора не совпадает с размером массива результатов</exception>
    public LikelyhoodFunction(IDataGroup x, IDataGroup y, Func<double, IDataGroup, double> dependencyFunction)
    {
      if (x.Count != y.Count) throw new ArgumentOutOfRangeException();
      this.x = x;
      this.y = y;
      this.dependencyFunction = dependencyFunction;
      ///ADD sorting
      variance = new PolylineFit(x, new PolylineFit(x, y).CalculateResidual());

      if (this.IsHeteroscedascity()) calculate = this.CalculateHet;
      else calculate = this.CalculateDef;
    }
    /// <summary>
    /// Определение наличия гетероскедастичности
    /// </summary>
    /// <returns>Наличие гетероскедастичности</returns>
    private bool IsHeteroscedascity()
    {
      double[] varVals = variance.Nodes.Select(point => point.y).ToArray();

      if (x.Count >= SAMPLE_COUNT_THRESHOLD && x.Count / variance.Nodes.Length > SAMPLE_COUNT_PER_NODE_THRESHOLD)
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
    private double CalculateHet(IDataGroup t)
    {
      double res = 0;
      double a, b;

      for (int i = 0; i < x.Count; i++)
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
    private double CalculateDef(IDataGroup t)
    {
      double res = 0;
      double b = 2 * midVar * midVar;
      double a;
      
      for (int i = 0; i < x.Count; i++)
      {
        a = y[i] - dependencyFunction(x[i], t);
        res += (a * a) / b;
      }

      return res;
    }
  }
}
