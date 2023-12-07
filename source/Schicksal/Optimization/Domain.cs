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
    private class Domain
    {
      /// <summary>
      /// Установленный размер пространства дла оптимизационного алгоритма
      /// </summary>
      public const double UNIT_SIZE = 1E10;

      /// <summary>
      /// Домен всех прямоугольников. Представляет собой сортированные списки.
      /// </summary>
      public readonly List<List<DividableRectangle>> domain;

      /// <summary>
      /// Точка с минимальным значением функции с обратным преобразованием
      /// </summary>
      public FuncPoint MinPointReal
      {
        get
        {
          FuncPoint temp = domain.Select(rectList => rectList[0]).Min().Center;
          return new FuncPoint(this.UnitCubeTransfer(temp.x), temp.y);
        }
      }

      /// <summary>
      /// Точка с минимальным значением функции без обратного преобразования
      /// </summary>
      public FuncPoint MinPoint
      {
        get
        {
          return domain.Select(rectList => rectList[0]).Min().Center;
        }
      }

      /// <summary>
      /// Верхняя граница
      /// </summary>
      public readonly VectorDataGroup highBound;

      /// <summary>
      /// Нижняя граница
      /// </summary>
      public readonly VectorDataGroup lowBound;

      /// <summary>
      /// Определение масштабирования пространств и создание домена
      /// </summary>
      /// <param name="optFunc">Оптимизируемая функция</param>
      /// <param name="lowBound">Нижняя граница поиска</param>
      /// <param name="highBound">Верхняя граница поиска</param>
      public Domain(Func<IDataGroup, double> optFunc, in VectorDataGroup lowBound, in VectorDataGroup highBound)
      {
        VectorDataGroup center = VectorDataGroup.Ones(lowBound.Count) * UNIT_SIZE / 2;
        VectorDataGroup sizes = VectorDataGroup.Ones(lowBound.Count) * UNIT_SIZE;
        this.highBound = highBound; this.lowBound = lowBound;
        Func<VectorDataGroup, double> unitOptFunc = x => optFunc(this.UnitCubeTransfer(x));
        DividableRectangle startRect = new DividableRectangle(new FuncPoint(center, unitOptFunc), sizes, unitOptFunc);
        domain = new List<List<DividableRectangle>>
          { new List<DividableRectangle> { startRect } };
      }

      /// <summary>
      /// Масштабирование аргумента функции
      /// </summary>
      /// <param name="x">Аргумент функции</param>
      /// <returns>Масштабирование значение аргумента функции</returns>
      public VectorDataGroup UnitCubeTransfer(in VectorDataGroup x)
      {
        return (highBound - lowBound) / UNIT_SIZE * x + lowBound;
      }

      /// <summary>
      /// Распределение прямоугольников в домен
      /// </summary>
      /// <param name="rectangles">Множество прямоугольников</param>
      public void Distribute(in DividableRectangle[] rectangles)
      {
        foreach (DividableRectangle rectangle in rectangles)
        {
          int i = 0;
          while (i < domain.Count && rectangle.Diag > domain[i][0].Diag) i++;
          if (rectangle.Diag == domain[i][0].Diag)
          {
            int j = 0;
            while (j < domain[i].Count && rectangle.F > domain[i][j].F) j++;
            domain[i].Insert(j, rectangle);
          }
          else
            domain.Insert(i, new List<DividableRectangle>(new DividableRectangle[] { rectangle }));
        }
      }

      /// <summary>
      /// Удаление прямоугольника из домена
      /// </summary>
      /// <param name="rectangle">Прямоугольник к удалению</param>
      public void Delete(in DividableRectangle rectangle)
      {
        int i = 0;
        while (i < domain.Count && rectangle.Diag != domain[i][0].Diag) i++;
        domain[i].Remove(rectangle);
        if (domain[i].Count == 0) domain.RemoveAt(i);
      }

      /// <summary>
      /// Нахождение потенциальных прямоугольников по методу DIRECT
      /// </summary>
      /// <returns>Потенциальные прямоугольники</returns>
      public DividableRectangle[] GetPotentialRectangles()
      {
        List<DividableRectangle> result;
        double minF = double.MaxValue;
        int indexMin = -1;
        for (int i = 0; i < domain.Count; i++)
          if (minF >= domain[i][0].F)
          {
            minF = domain[i][0].F;
            indexMin = i;
          }
        result = new List<DividableRectangle>(domain.Count - indexMin + 1);
        for (int i = indexMin; i < domain.Count; i++)
          result.Add(domain[i][0]);

        return result.ToArray();
      }
    }
  }
}
