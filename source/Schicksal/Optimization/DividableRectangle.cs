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
    //TODO: выделить сравнение в интерйфейс IComparer<> для того, чтобы избежать боксинга!
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
          left = new FuncPoint(m_center.x - VectorDataGroup.Unit(this.Dim, dimIndex[i]) * delta, m_func);
          right = new FuncPoint(m_center.x + VectorDataGroup.Unit(this.Dim, dimIndex[i]) * delta, m_func);
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
  }
}
