using Schicksal.Basic;
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
    /// <summary>
    /// Фактор
    /// </summary>
    private readonly double[] m_x;
    /// <summary>
    /// Результат
    /// </summary>
    private readonly double[] m_y;
    /// <summary>
    /// Среднее значение точек функции дисперсии
    /// </summary>
    private double m_mid_var;
    /// <summary>
    /// Исследуемая функция зависимости
    /// </summary>
    private readonly Func<double, double[], double> m_dep_fun;
    /// <summary>
    /// Интерфейс, содердащий актуальную функцию для расчета
    /// </summary>
    private readonly Func<double[], double> m_calc;
    /// <summary>
    /// Ломанная функция дисперсии
    /// </summary>
    private Dispersion m_var;
    /// <summary>
    /// Функция расчета значения функции правдоподобия
    /// </summary>
    public Func<double[], double> Calculate { get { return m_calc; } }
    /// <summary>
    /// Инициализация функции правдоподобия
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <param name="dependencyFunction">Определяемая функциональная параметрическая зависимость</param>
    /// <exception cref="ArgumentOutOfRangeException">Размер массива фактора не совпадает с размером массива результатов</exception>
    public LikelyhoodFunction(IPlainSample x, IPlainSample y, Func<double, double[], double> dependencyFunction)
    {
      if (x.Count != y.Count) throw new ArgumentOutOfRangeException();

      this.m_x = x.ToArray();
      this.m_y = y.ToArray();
      this.m_dep_fun = dependencyFunction;
      ///ADD sorting
      //variance = new PolylineFit(x, Residual.Calculate(x, y, new PolylineFit(x, y).Calculate));
      m_var = new Dispersion(x, y, new PolylineFit(x, y).Calculate);

      if (this.IsHeteroscedascity()) 
        m_calc = this.CalculateHet;
      else 
        m_calc = this.CalculateDef;
    }
    /// <summary>
    /// Определение наличия гетероскедастичности
    /// </summary>
    /// <returns>Наличие гетероскедастичности</returns>
    private bool IsHeteroscedascity()
    {
      double[] varVals = m_var.Values.ToArray();

      if (m_x.Length >= SAMPLE_COUNT_THRESHOLD)
      {
        //Сделать зависимость от стандартного отклонения!
        m_mid_var = varVals.Sum() / varVals.Length;
        double maxDiff = varVals.Max() - varVals.Min();

        if (maxDiff / m_mid_var <= HET_THRESHOLD) return false;
        else return true;
      }
      else
      {
        m_mid_var = varVals.Max();
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

      for (int i = 0; i < m_x.Length; i++)
      {
        a = m_y[i] - m_dep_fun(m_x[i], t);
        b = m_var[i];
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
      double b = 2 * m_mid_var * m_mid_var;
      double a;
      
      for (int i = 0; i < m_x.Length; i++)
      {
        a = m_y[i] - m_dep_fun(m_x[i], t);
        res += (a * a) / b;
      }

      return res;
    }

  }
}
