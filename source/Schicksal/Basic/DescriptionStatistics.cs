﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Notung;

namespace Schicksal.Basic
{
  /// <summary>
  /// Описательные статистики для выборок
  /// </summary>
  public static class DescriptionStatistics
  {
    /// <summary>
    /// Среднее арифметическое
    /// </summary>
    public static double Mean(IDataGroup group)
    {
      Debug.Assert(group != null);

      return group.Count > 0 ? group.Average() : double.NaN;
    }

    /// <summary>
    /// Медиана
    /// </summary>
    public static double Median(IDataGroup group)
    {
      var ordered = OrderedGroup.Construct(group);

      if (group.Count % 2 == 0)
        return (ordered[group.Count / 2 - 1] + ordered[group.Count / 2]) / 2;
      else
        return ordered[group.Count / 2];
    }

    /// <summary>
    /// Сумма квадратов отклонений
    /// </summary>
    public static double SquareDerivation(IDataGroup group)
    {
      Debug.Assert(group != null);

      var mean = Mean(group);
      double sum = 0;

      foreach (var value in group)
      {
        var derivation = value - mean;
        sum += derivation * derivation;
      }

      return sum;
    }

    /// <summary>
    /// Сумма модулей отклонений
    /// </summary>
    public static double PlainDerivation(IDataGroup group)
    {
      Debug.Assert(group != null);

      var mean = Mean(group);
      double sum = 0;

      foreach (var value in group)
        sum += Math.Abs(value - mean);

      return sum;
    }

    /// <summary>
    /// Выборочная дисперсия
    /// </summary>
    public static double Dispresion(IDataGroup group)
    {
      Debug.Assert(group != null);
      Debug.Assert(group.Count > 1);

      return SquareDerivation(group) / (group.Count - 1);
    }
    /// <summary>
    /// Упрощенная выборочная дисперсия
    /// </summary>
    public static double PlainDispersion(IDataGroup group)
    {
      Debug.Assert(group != null);
      Debug.Assert(group.Count > 1);

      return PlainDerivation(group) / Math.Sqrt(group.Count - 1);
    }

    public static IDataGroup Wrap(IDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      if (group is ArrayDataGroup)
        return group;

      var array = new double[group.Count];
      var i = 0;

      foreach (var value in group)
        array[i++] = value;

      return new ArrayDataGroup(array);
    }

    public static IMultyDataGroup Wrap(IMultyDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      if (group.All(g => g is ArrayDataGroup))
      {
        if (group is MultiArrayDataGroup)
          return group;

        var groups = new IDataGroup[group.Count];
        var i = 0;

        foreach (var value in group)
          groups[i++] = value;

        return new MultiArrayDataGroup(groups);
      }
      else
      {
        var groups = new IDataGroup[group.Count];
        var i = 0;

        foreach (var value in group)
          groups[i++] = Wrap(value);

        return new MultiArrayDataGroup(groups);
      }
    }

    public static ISetMultyDataGroup Wrap(ISetMultyDataGroup group)
    {
      Debug.Assert(group != null, "group cannot be null");

      if (group.SelectMany(g => g).All(g => g is ArrayDataGroup))
      {
        if (group is SetMultiArrayDataGroup && group.All(g => g is MultiArrayDataGroup))
          return group;
      }

      var groups = new IMultyDataGroup[group.Count];
      var i = 0;

      foreach (var value in group)
        groups[i++] = Wrap(value);

      return new SetMultiArrayDataGroup(groups);
    }
  }

  public class DescriptionStatisticsCalculator : RunBase
  {
    private readonly DataTable m_table;
    private readonly string m_result;
    private readonly string[] m_factors;
    private readonly string m_filter;
    private readonly double m_probability;

    public DescriptionStatisticsCalculator(DataTable table, string[] factors, string result, string filter, double probability)
    {
      m_table = table;
      m_factors = factors;
      m_result = result;
      m_filter = filter;
      m_probability = probability;
    }

    public DescriptionStatisticsEntry[] Result { get; private set; }

    public string[] Factors
    {
      get { return m_factors; }
    }

    public override void Run()
    {
      using (var group = new TableMultyDataGroup(m_table, m_factors, m_result, m_filter))
      {
        var res = new DescriptionStatisticsEntry[group.Count];

        for (int i = 0; i < res.Length; i++)
        {
          var name = group.GetKey(i);

          if (name.Contains(" AND "))
            name = name.Substring(name.IndexOf(" AND ") + 5);

          if (!string.IsNullOrEmpty(m_filter))
            name = name.Replace(" AND " + m_filter, "");

          res[i] = new DescriptionStatisticsEntry
          {
            Description = name.Replace(" AND ", ", ").Replace("[", "").Replace("]", ""),
            Mean = DescriptionStatistics.Mean(group[i]),
            Median = DescriptionStatistics.Median(group[i]),
            Min = group[i].Min(),
            Max = group[i].Max(),
            Count = group[i].Count
          };

          if (res[i].Count > 1)
          {
            res[i].StdError = Math.Sqrt(DescriptionStatistics.Dispresion(group[i]));
            res[i].ConfidenceInterval = res[i].StdError / Math.Sqrt(group[i].Count) *
              SpecialFunctions.invstudenttdistribution(group[i].Count - 1, 1 - m_probability / 2);
          }
          else
          {
            res[i].StdError = double.NaN;
            res[i].ConfidenceInterval = double.NaN;
          }
        }

        this.Result = res;
      }
    }
  }


  public class DescriptionStatisticsEntry
  {
    internal DescriptionStatisticsEntry() { }

    public string Description { get; internal set; }

    public double Mean { get; internal set; }

    public double Median { get; internal set; }

    public double Min { get; internal set; }

    public double Max { get; internal set; }

    public int Count { get; internal set; }

    public double StdError { get; internal set; }

    public double ConfidenceInterval { get; internal set; }
  }
}