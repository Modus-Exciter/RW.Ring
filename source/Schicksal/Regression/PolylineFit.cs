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

      public Line(Point2D left, Point2D right)
      {
        this.leftX = left.X;
        this.rightX = right.X;
        this.offsetY = left.Y;
        this.slope = (right.Y - left.Y) / (right.X - left.X);
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
    private readonly Point2D[] m_nodes;

    /// <summary>
    /// Отрезки ломанной
    /// </summary>
    public Line[] Lines { get { return (Line[])m_lines.Clone(); } }
    
    /// <summary>
    /// Узловые точки ломанной
    /// </summary>
    public Point2D[] Nodes { get { return (Point2D[])m_nodes.Clone(); } }
    
    /// <summary>
    /// Инициализация ломанной
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <exception cref="ArgumentOutOfRangeException">Размер массива факторов не совпадает с размером массива результатов</exception>
    public PolylineFit(IPlainSample x, IPlainSample y)
    {
      if (x.Count != y.Count) throw new ArgumentOutOfRangeException();

      List<List<Point2D>> uniqeDataPoints = this.GroupByUniqeX(x, y);
      int[] subsetsSizes = this.SubsetsSizes(uniqeDataPoints);

      Point2D[] dataPoints = uniqeDataPoints.SelectMany(i => i).ToArray();
      m_nodes = this.FitPoints(subsetsSizes, dataPoints);
      m_lines = this.CreateLines(dataPoints);
    }
    
    /// <summary>
    /// Преобразование изначальной выборки в связанную структуру по уникальным иксам
    /// </summary>
    /// <param name="x">Фактор</param>
    /// <param name="y">Результат</param>
    /// <returns>Список по x списков по y</returns>
    private List<List<Point2D>> GroupByUniqeX(IPlainSample x, IPlainSample y)
    {
      var uniqeX = new List<List<Point2D>> { new List<Point2D> { new Point2D { X = x[0], Y = y[0] } } };
      for (int i = 1; i < x.Count; i++)
      {
        if (Math.Round(x[i], TOL) == Math.Round(uniqeX[uniqeX.Count - 1][0].X, TOL))
          uniqeX[uniqeX.Count - 1].Add(new Point2D { X = x[i], Y = y[i]});
        else
          uniqeX.Add(new List<Point2D> { new Point2D { X = x[i], Y = y[i] } });
      }
      uniqeX.Sort((firstList, secondList) => {
        if (firstList[0].X < secondList[0].X) return -1;
        if (firstList[0].X > secondList[0].X) return 1;
        return 0;
      });
      return uniqeX;
    }
    
    /// <summary>
    /// Расчитывает размеры подвыборок
    /// </summary>
    /// <returns>Размеры подвыборок для расчета узлов</returns>
    private int[] SubsetsSizes(List<List<Point2D>> dataPoints)
    {
      int nodeCount = 2*(int)Math.Sqrt(dataPoints.Count);
      var domain = new LinkedList<int>(dataPoints.Select(subset => subset.Count));
      while (domain.Count > nodeCount)
      {
        int minSum = int.MaxValue;
        var node = domain.First;
        var minSets = new KeyValuePair<LinkedListNode<int>, LinkedListNode<int>>(node, node.Next);
        while(node.Next != null)
        {
          int sum = 0;
          sum = node.Value + node.Next.Value;
          if (sum < minSum)
          {
            minSum = sum;
            minSets = new KeyValuePair<LinkedListNode<int>, LinkedListNode<int>>(node, node.Next);
          }
          node = node.Next;
        }
        minSets.Key.Value += minSets.Value.Value;
        domain.Remove(minSets.Value);
      }
      return domain.ToArray();
    }
    
    /// <summary>
    /// Расчет средних геометрических для подвыборок изначальной выборки.
    /// Расчет узлов ломанной
    /// </summary>
    /// <param name="sectionCountCoef">Коэффициент количества выборок</param>
    /// <returns>Точки ломанной</returns>
    private Point2D[] FitPoints(int[] sizes, Point2D[] dataPoints)
    {
      Point2D[] nodes = new Point2D[sizes.Length];
      int offset = 0;
      for (int i = 0; i < sizes.Length; i++)
      {
        Point2D sum = new Point2D { X = 0, Y = 0 };
        for (int j = offset; j < offset + sizes[i]; j++)
        {
          sum.X += dataPoints[j].X;
          sum.Y += dataPoints[j].Y;
        }
        nodes[i] = new Point2D { X = sum.X / sizes[i], Y = sum.Y / sizes[i] };
        offset += sizes[i];
      }
      return nodes;
    }
    
    /// <summary>
    /// Расчет отрезков
    /// </summary>
    /// <returns>Отрезки, составляющие ломанную</returns>
    private Line[] CreateLines(Point2D[] dataPoints)
    {
      Line[] lines = new Line[m_nodes.Length - 1];
      lines[0] = new Line(m_nodes[0], m_nodes[1]) { leftX = dataPoints[0].X };
      for (int i = 1; i < lines.Length - 1; i++)
        lines[i] = new Line(m_nodes[i], m_nodes[i + 1]);
      lines[lines.Length - 1] = new Line(m_nodes[m_nodes.Length - 2], m_nodes.Last()) { rightX = dataPoints.Last().X };
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
