using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Schicksal.Regression
{
  /// <summary>
  /// Результаты теста корреляции
  /// </summary>
  public sealed class CorrelationMetrics
  {
    /// <summary>
    /// Фактор, для которого рассчитывается влияние
    /// </summary>
    public string Factor { get; internal set; }

    /// <summary>
    /// Количество пар значений при вычислении корреляции
    /// </summary>
    public int N { get; internal set; }

    /// <summary>
    /// Критическое значение коэффициента корреляции для уровня значимости 0,05
    /// </summary>
    [DisplayName("T 5%")]
    public double T005 { get; internal set; }

    /// <summary>
    /// Критическое значение коэффициента корреляции для уровня значимости 0,01
    /// </summary>
    [DisplayName("T 1%")]
    public double T001 { get; internal set; }

    /// <summary>
    /// Коэффициент линейной корреляции
    /// </summary>
    [DisplayName("r")]
    public double R { get; internal set; }

    /// <summary>
    /// Фактический критерий значимости линейной корреляции
    /// </summary>
    [DisplayName("Tr")]
    public double TR { get; internal set; }

    /// <summary>
    /// Вероятность нулевой гипотезы для полученного значения критерия R
    /// </summary>
    [DisplayName("pr")]
    public double PR { get; internal set; }

    /// <summary>
    /// Коэффициент криволинейной корреляции
    /// </summary>
    [DisplayName("η")]
    public double Eta { get; internal set; }

    /// <summary>
    /// Фактический критерий значимости криволинейной корреляции
    /// </summary>
    [DisplayName("Tη")]
    public double TH { get; internal set; }

    /// <summary>
    /// Вероятность нулевой гипотезы для полученного значения критерия η
    /// </summary>
    [DisplayName("pη")]
    public double PH { get; internal set; }

    /// <summary>
    /// Нормализованный коэффициент корреляции
    /// </summary>
    [Browsable(false)]
    public double Z { get; internal set; }

    /// <summary>
    /// Формула регрессионной зависимости
    /// </summary>
    [Browsable(false)]
    public CorrelationFormula Formula { get; internal set; }
  }

  /// <summary>
  /// Точка на графике регрессионной зависимости
  /// </summary>
  public struct Point2D
  {
    /// <summary>
    /// Абсцисса
    /// </summary>
    public double X { get; internal set; }

    /// <summary>
    /// Ордината
    /// </summary>
    public double Y { get; internal set; }

    /// <summary>
    /// Преобразование в строку
    /// </summary>
    /// <returns>Координаты точки в строковом представлении</returns>
    public override string ToString()
    {
      return string.Format("X = {0}, Y = {1}", this.X, this.Y);
    }
  }

  /// <summary>
  /// Пара коэффициентов, описывающих гетероскедастичность данных
  /// </summary>
  public struct Heteroscedasticity
  {
    /// <summary>
    /// Коэффициент Спирмана для корреляции
    /// </summary>
    public double SpearmanCoefficent { get; internal set; }

    /// <summary>
    /// Вероятность наличия гетероскедастичности
    /// </summary>
    public double Probability { get; internal set; }

    /// <summary>
    /// Преобразование в строку
    /// </summary>
    /// <returns>Пара коэффициентов в строковом представлении</returns>
    public override string ToString()
    {
      return string.Format("{0:0.0000} ({1:0.00}%)", this.SpearmanCoefficent, this.Probability * 100);
    }
  }

  /// <summary>
  /// Формулы, описывающие конкретную регрессионную зависимость
  /// </summary>
  public class CorrelationFormula
  {
    /// <summary>
    /// Минимальное значение независимой переменной
    /// </summary>
    public double MinX { get; internal set; }

    /// <summary>
    /// Максимальное значение независимой переменной
    /// </summary>
    public double MaxX { get; internal set; }

    /// <summary>
    /// Минимальное значение зависимой переменной
    /// </summary>
    public double MinY { get; internal set; }

    /// <summary>
    /// Максимальное значение зависимой переменной
    /// </summary>
    public double MaxY { get; internal set; }

    /// <summary>
    /// Исходные точки, по которым построена регрессия
    /// </summary>
    public Point2D[] SourcePoints { get; internal set; }

    /// <summary>
    /// Конкретные виды зависимостей, подходящие для этих точек
    /// </summary>
    public RegressionDependency[] Dependencies { get; internal set; }
  }
}