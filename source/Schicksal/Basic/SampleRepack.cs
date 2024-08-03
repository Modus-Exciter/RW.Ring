using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Перепаковка выборок в базовые выборки на основе массивов
  /// </summary>
  public static class SampleRepack
  {
    /// <summary>
    /// Перепаковка выборки
    /// </summary>
    /// <param name="sample">Исходная выборка</param>
    /// <returns>Выборка на основе массива</returns>
    public static IPlainSample Wrap(IPlainSample sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      return WrapIfNeeded(sample);
    }

    /// <summary>
    /// Перепаковка выборки
    /// </summary>
    /// <param name="sample">Исходная выборка второго порядка</param>
    /// <returns>Выборка второго порядка на основе массивов</returns>
    public static IDividedSample Wrap(IDividedSample sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      var check_result = Check(sample);

      if (!check_result.Repack)
        return sample;

      if (check_result.EqualPlainSamples)
        return new TwoDimensionsMultiDataSample(PrepareArray(sample));

      var samples = new IPlainSample[sample.Count];

      for (int i = 0; i < samples.Length; i++)
        samples[i] = WrapIfNeeded(sample[i]);

      return new ArrayDividedSample(samples);
    }

    /// <summary>
    /// Перепаковка выборки
    /// </summary>
    /// <typeparam name="T">Тип ключа подвыборки в выборке</typeparam>
    /// <param name="sample">Исходная выборка второго порядка с ключами групп</param>
    /// <returns>Выборка второго порядка на основе массивов с ключами групп</returns>
    public static IDividedSample<T> Wrap<T>(IDividedSample<T> sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      var check_result = Check(sample);

      if (!check_result.Repack)
        return sample;

      if (check_result.EqualPlainSamples)
        return new TwoDimensionsMultiDataSample<T>(PrepareArray(sample), sample.GetKey);

      var samples = new IPlainSample[sample.Count];

      for (int i = 0; i < samples.Length; i++)
        samples[i] = WrapIfNeeded(sample[i]);

      return new ArrayDividedSample<T>(new ArrayDividedSample(samples), sample.GetKey);
    }

    /// <summary>
    /// Перепаковка выборки
    /// </summary>
    /// <param name="sample">Исходная выборка третьего порядка</param>
    /// <returns>Выборка третьего порядка на основе массивов</returns>
    public static IComplexSample Wrap(IComplexSample sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      var check_result = Check(sample);

      if (!check_result.Repack)
        return sample;

      if (check_result.EqualPlainSamples && check_result.EqualDividedSamples)
        return RepackRectangleSetMultiDataSample(sample);

      var samples = new IDividedSample[sample.Count];

      for (int i = 0; i < sample.Count; i++)
      {
        if (check_result.EqualPlainSamples)
        {
          samples[i] = new TwoDimensionsMultiDataSample(PrepareArray(sample[i]));
        }
        else
        {
          var array = new IPlainSample[sample[i].Count];

          for (int j = 0; j < array.Length; j++)
            array[j] = WrapIfNeeded(sample[i][j]);

          samples[i] = new ArrayDividedSample(array);
        }
      }

      return new ArrayComplexSample(samples);
    }

    #region Implementation ------------------------------------------------------------------------

    private struct DimensionCheckResult
    {
      public bool Repack;
      public bool EqualPlainSamples;
      public bool EqualDividedSamples;

      public DimensionCheckResult(bool repack, bool equalPlain, bool equalDivided)
      {
        this.Repack = repack;
        this.EqualPlainSamples = equalPlain;
        this.EqualDividedSamples = equalDivided;
      }
    }

    private static IPlainSample WrapIfNeeded(IPlainSample sample)
    {
      if (sample is ArrayPlainSample)
        return sample as ArrayPlainSample;

      double[] array = new double[sample.Count];

      for (int i = 0; i < array.Length; i++)
        array[i] = sample[i];

      return new ArrayPlainSample(array);
    }

    private static double[,] PrepareArray(IDividedSample sample)
    {
      int count = sample.Count > 0 ? sample[0].Count : 0;

      double[,] array = new double[sample.Count, count];

      for (int i = 0; i < sample.Count; i++)
      {
        for (int j = 0; j < count; j++)
          array[i, j] = sample[i][j];
      }

      return array;
    }

    private static IComplexSample RepackRectangleSetMultiDataSample(IComplexSample sample)
    {
      int count1 = sample.Count > 0 ? sample[0].Count : 0;
      int count2 = count1 > 0 ? sample[0][0].Count : 0;

      double[,,] array = new double[sample.Count, count1, count2];

      for (int i = 0; i < sample.Count; i++)
      {
        for (int j = 0; j < sample[i].Count; j++)
        {
          for (int k = 0; k < sample[i][j].Count; k++)
            array[i, j, k] = sample[i][j][k];
        }
      }

      return new ThreeDimensionsSetDataSample(array);
    }

    private static DimensionCheckResult Check(IDividedSample sample)
    {
      bool first = true;
      bool repack = false;
      bool equal = true;
      int dimension = 0;

      if (sample is TwoDimensionsMultiDataSample)
        return new DimensionCheckResult(false, true, true);

      for (int i = 0; i < sample.Count; i++)
      {
        repack |= !(sample[i] is ArrayPlainSample);

        if (first)
        {
          dimension = sample[i].Count;
          first = false;
        }
        else
        {
          equal &= sample[i].Count == dimension;

          if (!equal && repack)
            break;
        }
      }

      return new DimensionCheckResult(repack, equal, true);
    }

    private static DimensionCheckResult Check(IComplexSample sample)
    {
      bool first = true;
      bool repack = false;
      bool equal1 = true;
      bool equal2 = true;
      int dimension = 0;

      for (int i = 0; i < sample.Count; i++)
      {
        var inner_check = Check(sample[i]);

        repack |= inner_check.Repack;
        equal1 &= inner_check.EqualPlainSamples;

        if (first)
        {
          dimension = sample[i].Count;
          first = false;
        }
        else
        {
          equal2 &= sample[i].Count == dimension;

          if (!equal2 && repack)
            break;
        }
      }

      return new DimensionCheckResult(repack, equal1, equal2);
    }

    private class TwoDimensionsMultiDataSample : IDividedSample, IEqualSubSamples
    {
      private readonly TwoDimensionsDataSample[] m_array;

      public TwoDimensionsMultiDataSample(double[,] array)
      {
        m_array = new TwoDimensionsDataSample[array.GetLength(0)];

        for (int i = 0; i < m_array.Length; i++)
          m_array[i] = new TwoDimensionsDataSample(array, i);
      }

      public IPlainSample this[int index]
      {
        get { return m_array[index]; }
      }

      public int Count
      {
        get { return m_array.Length; }
      }

      public int SubSampleSize
      {
        get { return m_array.Length > 0 ? m_array[0].Count : 0; }
      }

      public IEnumerator<IPlainSample> GetEnumerator()
      {
        return (m_array as IList<IPlainSample>).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return m_array.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Number sequence set, count={0}", m_array.Length);
      }

      public override bool Equals(object obj)
      {
        var other = obj as TwoDimensionsMultiDataSample;

        if (other == null || ReferenceEquals(this, obj))
          return other != null;

        if (m_array.Length == 0 && other.m_array.Length == 0)
          return true;

        return m_array[0].Array == other.m_array[0].Array;
      }

      public override int GetHashCode()
      {
        if (m_array.Length == 0)
          return 0;
        else
          return m_array[0].Array.GetHashCode();
      }
    }

    private sealed class TwoDimensionsDataSample : IPlainSample
    {
      public readonly double[,] Array;
      private readonly int m_index;

      public TwoDimensionsDataSample(double[,] array, int index)
      {
        this.Array = array;
        m_index = index;
      }

      public double this[int index]
      {
        get { return this.Array[m_index, index]; }
      }

      public int Count
      {
        get { return this.Array.GetLength(1); }
      }

      public IEnumerator<double> GetEnumerator()
      {
        int size = this.Array.GetLength(1);

        for (int i = 0; i < size; i++)
          yield return this.Array[m_index, i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Number sequence, count={0}", this.Array.GetLength(1));
      }

      public override bool Equals(object obj)
      {
        var other = obj as TwoDimensionsDataSample;

        if (other == null || ReferenceEquals(this, obj))
          return other != null;

        return this.Array == other.Array && m_index == other.m_index;
      }

      public override int GetHashCode()
      {
        return (m_index + 1) ^ this.Array.GetHashCode();
      }
    }

    private sealed class TwoDimensionsMultiDataSample<T> : TwoDimensionsMultiDataSample, IDividedSample<T>
    {
      private readonly Dictionary<T, int> m_indexes;
      private readonly T[] m_keys;

      public TwoDimensionsMultiDataSample(double[,] array, Func<int, T> keys) : base(array)
      {
        m_indexes = new Dictionary<T, int>();
        m_keys = new T[base.Count];

        for (int i = 0; i < base.Count; i++)
        {
          var key = keys(i);

          m_indexes.Add(key, i);
          m_keys[i] = key;
        }
      }

      public IPlainSample this[T key]
      {
        get { return base[m_indexes[key]]; }
      }

      public int GetIndex(T key)
      {
        return m_indexes[key];
      }

      public T GetKey(int index)
      {
        return m_keys[index];
      }
    }

    private sealed class ThreeDimensionsSetDataSample : IComplexSample, IEqualSubSamples
    {
      private readonly ThreeDimensionsMultiDataSample[] m_samples;
      private readonly double[,,] m_data;

      public ThreeDimensionsSetDataSample(double[,,] data)
      {
        m_data = data;
        m_samples = new ThreeDimensionsMultiDataSample[data.GetLength(0)];

        for (int i = 0; i < m_samples.Length; i++)
          m_samples[i] = new ThreeDimensionsMultiDataSample(data, i);
      }

      public IDividedSample this[int index]
      {
        get { return m_samples[index]; }
      }

      public int Count
      {
        get { return m_samples.Length; }
      }

      public int SubSampleSize
      {
        get { return m_data.GetLength(1); }
      }

      public IEnumerator<IDividedSample> GetEnumerator()
      {
        return (m_samples as IList<IDividedSample>).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Number sequence complex, count={0}", m_data.GetLength(0));
      }

      public override bool Equals(object obj)
      {
        var other = obj as ThreeDimensionsSetDataSample;

        if (other == null || ReferenceEquals(this, obj))
          return other != null;

        return Equals(m_data, other.m_data);
      }

      public override int GetHashCode()
      {
        return m_data.GetHashCode();
      }
    }

    private sealed class ThreeDimensionsMultiDataSample : IDividedSample, IEqualSubSamples
    {
      private readonly ThreeDimensionsDataSample[] m_data;

      public ThreeDimensionsMultiDataSample(double[,,] data, int index)
      {
        m_data = new ThreeDimensionsDataSample[data.GetLength(1)];

        for (int i = 0; i < m_data.Length; i++)
          m_data[i] = new ThreeDimensionsDataSample(data, index, i);
      }

      public IPlainSample this[int index]
      {
        get { return m_data[index]; }
      }

      public int Count
      {
        get { return m_data.Length; }
      }

      public int SubSampleSize
      {
        get { return m_data.Length > 0 ? m_data[0].Array.GetLength(2) : 0; }
      }

      public IEnumerator<IPlainSample> GetEnumerator()
      {
        return (m_data as IList<IPlainSample>).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Number sequence set, count={0}", m_data.Length);
      }

      public override bool Equals(object obj)
      {
        var other = obj as ThreeDimensionsMultiDataSample;

        if (other == null || ReferenceEquals(this, obj))
          return other != null;

        if (other == null)
          return false;

        if (m_data.Length == 0 && other.m_data.Length == 0)
          return true;

        return m_data[0].Array == other.m_data[0].Array;
      }

      public override int GetHashCode()
      {
        if (m_data.Length == 0)
          return 0;
        else
          return m_data[0].Array.GetHashCode() ^ m_data[0].Index1;
      }
    }

    private sealed class ThreeDimensionsDataSample : IPlainSample
    {
      public readonly double[,,] Array;
      private readonly int m_index1;
      private readonly int m_index2;

      public ThreeDimensionsDataSample(double[,,] array, int index1, int index2)
      {
        this.Array = array;
        m_index1 = index1;
        m_index2 = index2;
      }

      public int Index1
      {
        get { return m_index1; }
      }

      public double this[int index]
      {
        get { return this.Array[m_index1, m_index2, index]; }
      }

      public int Count
      {
        get { return this.Array.GetLength(2); }
      }

      public IEnumerator<double> GetEnumerator()
      {
        var size = this.Array.GetLength(2);

        for (int i = 0; i < size; i++)
          yield return this.Array[m_index1, m_index2, i];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      public override string ToString()
      {
        return string.Format("Number sequence, count={0}", this.Array.GetLength(2));
      }

      public override bool Equals(object obj)
      {
        var other = obj as ThreeDimensionsDataSample;

        if (other == null || ReferenceEquals(this, obj))
          return other != null;

        return this.Array == other.Array && m_index1 == other.m_index1 && m_index2 == other.m_index2;
      }

      public override int GetHashCode()
      {
        return (m_index1 + 1) ^ (m_index2 + 3) ^ this.Array.GetHashCode();
      }
    }

    #endregion
  }
}