using Notung.Logging;
using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using static Schicksal.Optimization.MathOptimization;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    /// <summary>
    /// Абсолютный интервальный оптимизационный алгоритм DIRECT
    /// </summary>
    /// <param name="optFunc">Оптимизируемая функция</param>
    /// <param name="lowBoundary">Нижняя граница поиска (по всем измерениям)</param>
    /// <param name="highBoundary">Верхняя граница поиска</param>
    /// <param name="options">Параметры оптимизации: точность, кол-во итераций</param>
    /// <returns>Минимум функции</returns>
    public static VectorDataGroup DIRECTSearch(Func<IDataGroup, double> optFunc, in VectorDataGroup lowBoundary, in VectorDataGroup highBoundary, OptimizationOptions options = null)
    {
      options = options ?? OptimizationOptions.Default;
      //Вспомогательные переменные
      int iterCount = 0;
      int dim = lowBoundary.Count;
      double deltaY = double.MaxValue;
      double deltaX = double.MaxValue;
      //Инициализация домена
      Domain domain = new Domain(optFunc, lowBoundary, highBoundary);
      //Точки для расчета критерия остановка
      FuncPoint minPoint = domain.MinPoint; 
      FuncPoint newMinPoint = domain.MinPoint;
      //Основной цикл алгоритма
      while (iterCount < options.m_maxIter)
      {
        //Расчет потенциальных прямоугольников
        DividableRectangle[] potentialRect = domain.GetPotentialRectangles();
        //Разделение потенциальных прямоугольников
        foreach (DividableRectangle rectangle in potentialRect)
        {
          domain.Distribute(rectangle.Divide());
          domain.Delete(rectangle);
        }
        //Расчет количественных критерий остановка
        newMinPoint = domain.MinPoint;
        deltaX = Math.Abs((newMinPoint.x - minPoint.x).Length()/dim);
        deltaY = Math.Abs(newMinPoint.y - minPoint.y);
        minPoint = newMinPoint;
        iterCount++;
      }

      return domain.MinPointReal.x;
    }
  }
}

