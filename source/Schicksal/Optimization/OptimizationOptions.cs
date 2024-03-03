using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
  /// <summary>
  /// Параметры оптимизации
  /// </summary>
  public class OptimizationOptions
  {
    /// <summary>
    /// Параметры по умолчанию
    /// </summary>
    public static readonly OptimizationOptions Default = new OptimizationOptions();
    
    /// <summary>
    /// Точность по аргументу
    /// </summary>
    public readonly double m_tolX;
    
    /// <summary>
    /// Точность по результату
    /// </summary>
    public readonly double m_tolY;
    
    /// <summary>
    /// Максимальное количество итераций оптимизационного алгоритма
    /// </summary>
    public readonly int m_maxIter;

    /// <summary>
    /// Максимальное количество исполнений вычисления функици
    /// </summary>
    public readonly int m_maxFunEval;

    /// <summary>
    /// Конструктор параметров оптимизации
    /// </summary>
    /// <param name="tolX">Точность по аргументу</param>
    /// <param name="tolY">Точность по результату</param>
    /// <param name="maxIter">Максимальное количество итераций</param>
    /// <param name="maxFunEval">Максимальное количество</param>
    public OptimizationOptions(double tolX = 1E-10, double tolY = 1E-10, int maxIter = 500, int maxFunEval = 10000)
    {
      this.m_tolX = tolX;
      this.m_tolY = tolY;
      this.m_maxIter = maxIter;
      this.m_maxFunEval = maxFunEval;
    }
  }
}
