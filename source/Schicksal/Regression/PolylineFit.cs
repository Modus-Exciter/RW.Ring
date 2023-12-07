using Schicksal.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Schicksal.Regression
{
  public class PolylineFit
  {
    /// <summary>
    /// Отрезок
    /// </summary>
    public struct Line
    {
      public double leftX;
      public double rightX;
      public double offsetY;
      public double slope;

      public Line((double x, double y) left, (double x, double y) right)
      {
        this.leftX = left.x;
        this.rightX = right.x;
        this.offsetY = left.y;
        this.slope = (right.y - left.y) / (right.x - left.x);
      }
      /// <summary>
      /// Принадлежит ли аргумент области определения отрезца
      /// </summary>
      /// <param name="x">Аргумент</param>
      /// <returns></returns>
      public bool IsXBelong(double x)
      {
        if (x >= leftX && x <= rightX)
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
        return (x - leftX) * slope + offsetY;
      }
    }
    /// <summary>
    /// Точность операции эквивалентности фактора
    /// </summary>
    const int TOL = 4;

    private readonly Line[] m_lines;
    private readonly (double x, double y)[] m_nodes;
    /// <summary>
    /// Отрезки ломанной
    /// </summary>
    public Line[] Lines { get { return (Line[])m_lines.Clone(); } }
    /// <summary>
    /// Узловые точки ломанной
    /// </summary>
    public (double x, double y)[] Nodes { get { return ((double, double)[])m_nodes.Clone(); } }
    /// <summary>
    /// Инициализация ломанной
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <exception cref="ArgumentOutOfRangeException">Размер массива факторов не совпадает с размером массива результатов</exception>
    public PolylineFit(IDataGroup x, IDataGroup y)
    {
      if (x.Count != y.Count) throw new ArgumentOutOfRangeException();

      List<List<(double x, double y)>> uniqeDataPoints = this.GroupByUniqeX(x, y);
      int[] subsetsSizes = this.SubsetsSizes(uniqeDataPoints);
      
      (double x, double y)[] dataPoints = uniqeDataPoints.SelectMany(i => i).ToArray();
      m_nodes = this.FitPoints(subsetsSizes, dataPoints);
      m_lines = this.CreateLines(dataPoints);
    }
    /// <summary>
    /// Преобразование изначальной выборки в связанную структуру по уникальным иксам
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <returns>Список по x списков по y</returns>
    private List<List<(double, double)>> GroupByUniqeX(IDataGroup x, IDataGroup y)
    {
      var uniqeX = new List<List<(double x, double y)>> { new List<(double, double)> { (x[0], y[0]) } };
      for (int i = 1; i < x.Count; i++)
      {
        if (Math.Round(x[i], TOL) == Math.Round(uniqeX[uniqeX.Count - 1][0].x, TOL))
          uniqeX[uniqeX.Count - 1].Add((x[i], y[i]));
        else
          uniqeX.Add(new List<(double, double)> { (x[i], y[i]) });
      }
      uniqeX.Sort((firstList, secondList) => {
        if (firstList[0].x < secondList[0].x) return -1;
        if (firstList[0].x > secondList[0].x) return 1;
        return 0;
      });
      return uniqeX;
    }
    /// <summary>
    /// Расчитывает размеры подвыборок
    /// </summary>
    /// <returns>Размеры подвыборок для расчета узлов</returns>
    private int[] SubsetsSizes(List<List<(double x, double y)>> dataPoints)
    {
      int nodeCount = 2*(int)Math.Sqrt(dataPoints.Count);
      var domain = new LinkedList<int>(dataPoints.Select(subset => subset.Count));
      while (domain.Count > nodeCount)
      {
        int minSum = int.MaxValue;
        var node = domain.First;
        var minSets = (First: node, Second: node.Next);
        while(node.Next != null)
        {
          int sum = 0;
          sum = node.Value + node.Next.Value;
          if (sum < minSum)
          {
            minSum = sum;
            minSets = (node, node.Next);
          }
          node = node.Next;
        }
        minSets.First.Value += minSets.Second.Value;
        domain.Remove(minSets.Second);
      }
      return domain.ToArray();
    }
    /// <summary>
    /// Расчет средних геометрических для подвыборок изначальной выборки.
    /// Расчет узлов ломанной
    /// </summary>
    /// <param name="sectionCountCoef">Коэффициент количества выборок</param>
    /// <returns>Точки ломанной</returns>
    private (double, double)[] FitPoints(int[] sizes, (double x, double y)[] dataPoints)
    {
      (double x, double y)[] nodes = new (double, double)[sizes.Length];
      int offset = 0;
      for (int i = 0; i < sizes.Length; i++)
      {
        (double x, double y) sum = (0, 0);
        for (int j = offset; j < offset + sizes[i]; j++)
        {
          sum.x += dataPoints[j].x;
          sum.y += dataPoints[j].y;
        }
        nodes[i] = (sum.x / sizes[i], sum.y / sizes[i]);
        offset += sizes[i];
      }
      return nodes;
    }
    /// <summary>
    /// Расчет отрезков
    /// </summary>
    /// <returns>Отрезки, составляющие ломанную</returns>
    private Line[] CreateLines((double x, double y)[] dataPoints)
    {
      Line[] lines = new Line[m_nodes.Length - 1];
      lines[0] = new Line(m_nodes[0], m_nodes[1]) { leftX = dataPoints[0].x };
      for (int i = 1; i < lines.Length - 1; i++)
        lines[i] = new Line(m_nodes[i], m_nodes[i + 1]);
      lines[lines.Length - 1] = new Line(m_nodes[m_nodes.Length - 2], m_nodes.Last()) { rightX = dataPoints.Last().x };
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
  }
}
