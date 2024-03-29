﻿using Schicksal.Basic;
using Schicksal.VectorField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    private struct DividableRectangle : IComparable<DividableRectangle>
    {
      /// <summary>
      /// Пробы функции в "левом" и "правом" центре относительно середины
      /// </summary>
      private struct Sample : IComparable<Sample>
      {
        private readonly FuncPoint m_left;
        private readonly FuncPoint m_right;
        private readonly int m_dim_index;

        public Sample(in FuncPoint left, in FuncPoint right, int dimIndex)
        {
          m_left = left;
          m_right = right;
          m_dim_index = dimIndex;
        }

        public int Dim
        {
          get { return m_dim_index; }
        }

        public FuncPoint Left
        {
          get { return m_left; }
        }

        public FuncPoint Right
        {
          get { return m_right; }
        }

        public FuncPoint Min
        {
          get { return m_left.y < m_right.y ? m_left : m_right; }
        }

        public int CompareTo(Sample point)
        {
          return this.Min.CompareTo(point.Min);
        }
      }

      const int DIVIDE_COUNT = 3;

      //Центр треугольника с значением фуннции в нем. Значение функции - одно из состовляющих критерия оптимальности
      private readonly FuncPoint m_center;
      
      //Размеры прямоугольника
      private readonly VectorDataGroup m_sizes;
      
      //Диагональ - одно из состовляющих критерия оптимальности
      private readonly double m_diag;
      
      //Функция, по которой осуществляется оптимизационный расчет
      private readonly Func<VectorDataGroup, double> m_func;

      /// <summary>
      /// Размерность гиперпрямоугольника
      /// </summary>
      public int Dim
      {
        get { return m_sizes.Count; }
      }

      /// <summary>
      /// Длина диагонали (критерий оптимальности)
      /// </summary>
      public double Diag
      {
        get { return m_diag; }
      }
      
      /// <summary>
      /// Значение функции в центре (критерий оптимальности)
      /// </summary>
      public double F
      {
        get { return m_center.y; }
      }
      
      /// <summary>
      /// Положение центра в R^n пространстве
      /// </summary>
      public VectorDataGroup X
      {
        get { return m_center.x; }
      }

      /// <summary>
      /// Совокупность значения функции в центре и положения центра
      /// </summary>
      public FuncPoint Center
      {
        get { return m_center; }
      }

      /// <summary>
      /// Строка для отладки
      /// </summary>
      /// <returns> Полную информацию о существенных свойствах прямоугольника</returns>
      public override string ToString()
      {
        return String.Format("x:{0}   f:{1}     d:{3}   sizes:{2}", 
          this.X.ToString(),
          NumberConvert.Do(this.F),
          this.m_sizes.ToString(),
          NumberConvert.Do(this.Diag));
      }

      /// <summary>
      /// Инициализация прямоугольника
      /// </summary>
      /// <param name="center">Центр прямоугольника и значения функции в нем</param>
      /// <param name="sizes">Размеры сторон прямоугольника</param>
      /// <param name="func">Расчетная функция</param>
      public DividableRectangle(in FuncPoint center, in VectorDataGroup sizes, Func<VectorDataGroup, double> func)
      {
        this.m_center = center;
        this.m_sizes = sizes;
        this.m_func = func;
        m_diag = sizes.Length();
      }

      /// <summary>
      /// Разделение прямоугольника вдоль максимальных длин
      /// </summary>
      /// <returns>Составляющие прямоугольники</returns>
      public DividableRectangle[] Divide()
      {
        Sample[] samples = this.Sampling(this.GetSplitDimensions());
        DividableRectangle[] rectangles = new DividableRectangle[2 * samples.Length + 1];
        DividableRectangle centerRect = this;

        Array.Sort(samples);
        for (int i = 0; i < samples.Length; i++)
        {
          DividableRectangle[] temp = centerRect.Split(samples[i]);
          rectangles[2 * i] = temp[0];
          rectangles[2 * i + 1] = temp[1];
          centerRect = temp[2];
        }
        rectangles[rectangles.Length - 1] = centerRect;

        return rectangles;
      }

      /// <summary>
      /// Разделение прямоугольника по пробам
      /// </summary>
      /// <param name="sample">Пробы в будущих центрах</param>
      /// <returns>Составляющие прямоугольники</returns>
      private DividableRectangle[] Split(in Sample sample)
      {
        DividableRectangle[] result = new DividableRectangle[DIVIDE_COUNT];

        VectorDataGroup sizes = this.SplitSize(sample.Dim);

        result[0] = new DividableRectangle(sample.Left, sizes, m_func);
        result[1] = new DividableRectangle(sample.Right, sizes, m_func);
        result[2] = new DividableRectangle(m_center, sizes, m_func);

        return result;
      }
      
      /// <summary>
      /// Расчет проб внутри прямоугольника
      /// </summary>
      /// <param name="dimIndex">Индексы измерений для пробы</param>
      /// <returns>Пробы будущих центров</returns>
      private Sample[] Sampling(int[] dimIndex)
      {
        Sample[] samples = new Sample[dimIndex.Length];
        FuncPoint left, right;
        double delta;

        for (int i = 0; i < dimIndex.Length; i++)
        {
          delta = m_sizes[dimIndex[i]] / DIVIDE_COUNT;
          left = new FuncPoint(m_center.x - this.UnitVector(dimIndex[i]) * delta, m_func);
          right = new FuncPoint(m_center.x + this.UnitVector(dimIndex[i]) * delta, m_func);
          samples[i] = new Sample(left, right, dimIndex[i]);
        }

        return samples;
      }

      /// <summary>
      /// Расчет измерений с максимальной длиной
      /// </summary>
      /// <returns>Индексы измерений с максимальной длиной</returns>
      private int[] GetSplitDimensions()
      {
        List<int> res = new List<int>(this.Dim);
        double max = m_sizes.Max();

        for (int i = 0; i < this.Dim; i++)
          if (m_sizes[i] == max)
            res.Add(i); 

        return res.ToArray();
      }

      /// <summary>
      /// Расчет сторон подпрямоугольника
      /// </summary>
      /// <param name="dimIndex">Измерение, в котором производится деление</param>
      /// <returns></returns>
      private VectorDataGroup SplitSize(int dimIndex)
      {
        double[] newSizes = m_sizes.ToArray();
        newSizes[dimIndex] /= DIVIDE_COUNT;
        return new VectorDataGroup(newSizes);
      }

      private VectorDataGroup UnitVector(int dimIndex)
      {
        return VectorDataGroup.Unit(this.Dim, dimIndex);
      }

      public int CompareTo(DividableRectangle other)
      {
        if (this.F < other.F) return -1;
        if (this.F > other.F) return 1;
        return 0;
      }
    }

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
      { get 
        {
          FuncPoint temp = domain.Select(rectList => rectList[0]).Min().Center;
          return new FuncPoint(this.UnitCubeTransfer(temp.x), temp.y);
        } 
      }
      
      /// <summary>
      /// Точка с минимальным значением функции без обратного преобразования
      /// </summary>
      public FuncPoint MinPoint 
      { get 
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
