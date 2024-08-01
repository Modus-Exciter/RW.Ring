using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Набор выборок одинакового размера
  /// </summary>
  public interface IEqualSubSamples : ISample
  {
    /// <summary>
    /// Размер всех выборок
    /// </summary>
    int SubSampleSize { get; }
  }
  
  /// <summary>
  /// Вспомогательный класс для объединения нескольких выборок в одну 
  /// </summary>
  [ImmutableObject(true)]
  public sealed class JoinedSample : IPlainSample
  {
    private readonly IDividedSample m_sample;
    private readonly int m_total_count;

    public JoinedSample(IDividedSample sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      m_sample = sample;
      m_total_count = m_sample.Sum(g => g.Count);
    }

    public double this[int index]
    {
      get
      {
        int part_index = 0;
        var sub = m_sample as IEqualSubSamples;

        if (sub == null)
        {
          while (index >= m_sample[part_index].Count)
            index -= m_sample[part_index++].Count;
        }
        else
        {
          part_index = index / sub.SubSampleSize;
          index %= sub.SubSampleSize;
        }

        return m_sample[part_index][index];
      }
    }

    public int Count
    {
      get { return m_total_count; }
    }

    public IEnumerator<double> GetEnumerator()
    {
      return m_sample.SelectMany(g => g).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public override string ToString()
    {
      return string.Format("Joined {0}", m_sample);
    }

    public override bool Equals(object obj)
    {
      var other = obj as JoinedSample;

      if (other == null)
        return false;

      return m_sample.Equals(other.m_sample);
    }

    public override int GetHashCode()
    {
      return m_sample.GetHashCode();
    }
  }

  /// <summary>
  /// Вспомогательный класс для объединения нескольких наборов выборок в один 
  /// </summary>
  [ImmutableObject(true)]
  public sealed class PartiallyJoinedSample : IDividedSample
  {
    private readonly IComplexSample m_sample;
    private readonly int m_total_count;

    public PartiallyJoinedSample(IComplexSample sample)
    {
      if (sample == null)
        throw new ArgumentNullException("sample");

      m_sample = sample;
      m_total_count = m_sample.Sum(g => g.Count);
    }

    public IPlainSample this[int index]
    {
      get
      {
        int part_index = 0;
        var sub = m_sample as IEqualSubSamples;

        if (sub == null)
        {
          while (index >= m_sample[part_index].Count)
            index -= m_sample[part_index++].Count;
        }
        else
        {
          part_index = index / sub.SubSampleSize;
          index %= sub.SubSampleSize;
        }

        return m_sample[part_index][index];
      }
    }

    public int Count 
    {
      get { return m_total_count; } 
    }

    public IEnumerator<IPlainSample> GetEnumerator()
    {
      return m_sample.SelectMany(g => g).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public override string ToString()
    {
      return string.Format("Joined {0}", m_sample);
    }

    public override bool Equals(object obj)
    {
      var other = obj as PartiallyJoinedSample;

      if (other == null)
        return false;

      return m_sample.Equals(other.m_sample);
    }

    public override int GetHashCode()
    {
      return m_sample.GetHashCode();
    }
  }
}