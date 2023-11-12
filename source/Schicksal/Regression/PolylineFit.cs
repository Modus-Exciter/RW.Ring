using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Regression
{
  public class PolylineFit
  {
    /// <summary>
    /// Точка
    /// </summary>
    public struct Point
    {
      public readonly double x;
      public readonly double y;

      public Point(double x, double y)
      {
        this.x = x; this.y = y;
      }

      public override string ToString()
      {
        return x.ToString() + ' ' + y.ToString();
      }
    }
    /// <summary>
    /// Отрезок
    /// </summary>
    public struct Line
    {
      /// <summary>
      /// Правая точка
      /// </summary>
      public readonly Point left;
      /// <summary>
      /// Левая точка
      /// </summary>
      public readonly Point right;
      /// <summary>
      /// Угол наклона
      /// </summary>
      public readonly double slope;
      /// <summary>
      /// Инициализация отрезка
      /// </summary>
      /// <param name="left">Левая точка</param>
      /// <param name="right">Правая точкуа</param>
      public Line(Point left, Point right)
      {
        this.left = left;
        this.right = right;
        this.slope = (right.y - left.y) / (right.x - left.x);
      }
      /// <summary>
      /// Принадлежит ли аргумент области определения отрезца
      /// </summary>
      /// <param name="x">Аргумент</param>
      /// <returns></returns>
      public bool IsXBelong(double x)
      {
        if (x >= left.x && x <= right.x)
          return true;
        return false;
      }
      /// <summary>
      /// Расчет значения отрезка в точке
      /// </summary>
      /// <param name="x">Аргумент</param>
      /// <returns>Значение y отрезка</returns>
      public double Calculate(double x)
      {
        return (x - left.x) * slope + left.y;
      }
    }
    /// <summary>
    /// Точность операции эквивалентности фактора
    /// </summary>
    const int TOL = 4;
    /// <summary>
    /// Обычное значение коэффициента количества точек
    /// </summary>
    const double DEFAULT_SECTION_COUNT_COEF = 1;

    private readonly Line[] m_lines;
    private readonly Point[] m_points;
    /// <summary>
    /// Упорядоченные точки
    /// </summary>
    private readonly List<List<Point>> m_data_points;
    /// <summary>
    /// Отрезки ломанной
    /// </summary>
    public Line[] Lines { get { return (Line[])m_lines.Clone(); } }
    /// <summary>
    /// Узловые точки ломанной
    /// </summary>
    public Point[] Points { get { return (Point[])m_points.Clone(); } }
    /// <summary>
    /// Инициализация ломанной
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <param name="sectionCountCoef">Коэффициент количества точек</param>
    /// <exception cref="ArgumentOutOfRangeException">Размер массива факторов не совпадает с размером массива результатов</exception>
    public PolylineFit(IDataGroup x, IDataGroup y, double sectionCountCoef = DEFAULT_SECTION_COUNT_COEF)
    {
      if (x.Count != y.Count) throw new ArgumentOutOfRangeException();
      m_data_points = this.GetPointsByUniqeX(x, y);
      m_data_points.Sort((firstList, secondList) => {
        if (firstList[0].x < secondList[0].x) return -1;
        if (firstList[0].x > secondList[0].x) return 1;
        return 0;
      });
      m_points = this.FitPoints(sectionCountCoef);
      m_lines = this.CreateLines();
    }
    /// <summary>
    /// Преобразование изначальной выборки в связанную структуру по уникальным иксам
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <returns>Список по x списков по y</returns>
    private List<List<Point>> GetPointsByUniqeX(IDataGroup x, IDataGroup y)
    {
      List<List<Point>> points = new List<List<Point>> { new List<Point> { new Point(x[0], y[0]) } };
      for (int i = 1; i < x.Count; i++)
      {
        if (Math.Round(x[i], TOL) == Math.Round(points[points.Count - 1][0].x, TOL))
          points[points.Count - 1].Add(new Point(x[i], y[i]));
        else
          points.Add(new List<Point> { new Point(x[i], y[i]) });
      }
      return points;
    }
    /// <summary>
    /// Расчет средних геометрических для подвыборок изначальной выборки.
    /// Расчет узлов ломанной
    /// </summary>
    /// <param name="sectionCountCoef">Коэффициент количества выборок</param>
    /// <returns>Точки ломанной</returns>
    private Point[] FitPoints(double sectionCountCoef)
    {
      int sectionCount = (int)(sectionCountCoef * Math.Sqrt(m_data_points.Count));
      int sectionSize = (int)((double)m_data_points.Count / sectionCount);
      int modulo = m_data_points.Count - sectionSize * sectionCount;
      Point[] linePoints = new Point[sectionCount + 2];

      linePoints[0] = new Point
        (m_data_points[0].Select(point => point.x).Average(),
        m_data_points[0].Select(point => point.y).Average());

      int index = 0;
      for (int i = 0; i < sectionCount; i++)
      {
        double midY = 0;
        double midX = 0;
        int pointsCount = 0;
        for (int j = 0; j < sectionSize || (i < modulo && j < (sectionSize + 1)); j++)
        {
          midY += m_data_points[index].Select(point => point.y).Sum();
          midX += m_data_points[index].Select(point => point.x).Sum();
          pointsCount += m_data_points[index].Count;
          index++;
        }
        midY /= pointsCount;
        midX /= pointsCount;
        linePoints[i + 1] = new Point(midX, midY);
      }

      linePoints[linePoints.Length - 1] = new Point
        (m_data_points.Last().Select(point => point.x).Average(),
        m_data_points.Last().Select(point => point.y).Average());

      return linePoints;
    }
    /// <summary>
    /// Расчет отрезков
    /// </summary>
    /// <returns>Отрезки, составляющие ломанную</returns>
    private Line[] CreateLines()
    {
      Line[] lines = new Line[m_points.Length - 1];
      for (int i = 0; i < lines.Length; i++)
        lines[i] = new Line(m_points[i], m_points[i + 1]);
      return lines;
    }
    /// <summary>
    /// Расчет значения ломанной
    /// </summary>
    /// <param name="x">Аргумент</param>
    /// <returns>Значение ломанной</returns>
    public double Calculate(double x)
    {
      int i = 0;
      while (!m_lines[i].IsXBelong(x)) i++;
      return m_lines[i].Calculate(x);
    }
    /// <summary>
    /// Расчет выборки невязок
    /// </summary>
    /// <returns>Массив невязок</returns>
    public IDataGroup CalculateResidual()
    {
      List<double> res = new List<double>();

      for (int i = 0; i < m_data_points.Count; i++)
        for (int j = 0; j < m_data_points[i].Count; j++)
          res.Add(Math.Abs(this.Calculate(m_data_points[i][j].x) - m_data_points[i][j].y));

      return new ArrayDataGroup(res.ToArray());
    }
  }
}
