using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Notung.Data;
using Schicksal.Basic;

namespace Schicksal.Anova
{
  public class InteractionCalculator
  {
    private readonly ConcurrentDictionary<EffectKey, SampleVariance> m_variance_cache;

    public InteractionCalculator(IEnumerable<FactorVariance> data)
    {
      if (data is null) 
        throw new ArgumentNullException(nameof(data));

      // Добавили в словарь все главные эффекты
      m_variance_cache = new ConcurrentDictionary<EffectKey, SampleVariance>();
    }

    public SampleVariance GetInteraction(IDividedSample<GroupKey> source, FactorInfo predictors)
    {
      source = Filter(source, predictors);

      Debug.Assert(IsFull(source, predictors));

      if (source.Count == 0)
        return default(SampleVariance);

      var splitted = predictors.Split(false).ToArray();
      var keys = new KeyedArray<FactorInfo>(splitted);
      var list = new List<FactorVariance>();
      var ei = new EffectKey { Factor = predictors, GradationCount = source.Sum(g => g.Count) };

      SampleVariance ret = m_variance_cache.GetOrAdd(ei, 
        e => FisherTest.MSb(GroupKey.Repack(source, predictors)));

      foreach (var p in splitted)
      {
        var ep = new EffectKey { Factor = p, GradationCount = source.Sum(g => g.Count) };
        list.Add(new FactorVariance(p, m_variance_cache.GetOrAdd(ep, 
          e => FisherTest.MSb(GroupKey.Repack(source, p)))));
      }

      foreach (int index in GetFactorOrder(splitted))
      {
        FactorVariance result = list[index];

        if (result.Factor.Count == 1)
          continue;

        foreach (var p in splitted[index].Split(false))
        {
          if (!keys.Contains(p))
            continue;

          FactorVariance res = list[keys.GetIndex(p)];
          result.Variance.DegreesOfFreedom -= res.Variance.DegreesOfFreedom;
          result.Variance.SumOfSquares -= res.Variance.SumOfSquares;

          if (result.Variance.DegreesOfFreedom < 0)
            result.Variance.DegreesOfFreedom = 0;

          if (result.Variance.SumOfSquares < 0)
            result.Variance.SumOfSquares = 0;

          list[index] = result;
        }
      }

      foreach (var f in list)
      {
        ret.DegreesOfFreedom -= f.Variance.DegreesOfFreedom;
        ret.SumOfSquares -= f.Variance.SumOfSquares;

        if (ret.DegreesOfFreedom < 0)
          ret.DegreesOfFreedom = 0;

        if (ret.SumOfSquares < 0)
          ret.SumOfSquares = 0;
      }

      return ret;
    }

    private static int[] GetFactorOrder(FactorInfo[] splitted)
    {
      var graph = new UnweightedListGraph(splitted.Length, true);
      var keys = new KeyedArray<FactorInfo>(splitted);

      for (int i = 0; i < splitted.Length; i++)
      {
        if (splitted[i].Count == 1)
          continue;

        foreach (var p in splitted[i].Split(false))
        {
          if (keys.Contains(p))
            graph.AddArc(keys.GetIndex(p), i);
        }
      }

      return TopologicalSort.Kahn(graph);
    }

    /// <summary>
    /// Удаление градаций факторов, нарушающих полноту факторного эксперимента
    /// </summary>
    /// <param name="source">Выброрка, разделённая на группы</param>
    /// <param name="predictors">Факторы, градации которых требуется проанализировать</param>
    /// <returns>Отфильтрованная выборка</returns>
    public static IDividedSample<GroupKey> Filter(IDividedSample<GroupKey> source, FactorInfo predictors)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      if (predictors is null)
        throw new ArgumentNullException(nameof(predictors));

      if (predictors.Count < 2 || source.Count == 0)
        return source;

      var uniqueValues = new Dictionary<string, HashSet<object>>(); // Ключ - фактор, значение - множество всех его уникальных значений во всех группах

      foreach (var p in predictors)
      {
        var set = new HashSet<object>();

        for (int i = 0; i < source.Count; i++)
          set.Add(source.GetKey(i)[p]);

        uniqueValues.Add(p, set);
      }

      // Если Декартово произведение и так полное, незачем фильтровать
      if (uniqueValues.Aggregate(1, (a, kv) => a * kv.Value.Count) == source.Count)
        return GroupKey.Repack(source, predictors);
       
      return PerformFilter(source, uniqueValues, predictors);
    }

    /// <summary>
    /// Получения словаря с частотами встречаемости всех уникальных значений всех факторов
    /// </summary>
    private static Dictionary<string, Dictionary<object, int>> GetFrequencyMap(IDividedSample<GroupKey> source, FactorInfo predictors)
    {
      var result = new Dictionary<string, Dictionary<object, int>>(); //Ключ - фактор, значение - словарь: градация-количество элементов

      foreach (var factor in predictors) //Обходим все факторы
      {
        var freqDict = new Dictionary<object, int>(); 

        for (int groupIndex = 0; groupIndex < source.Count; groupIndex++)
        {
          object value = source.GetKey(groupIndex)[factor]; //Текущая градация (значение фактора)
          int groupSize = source[groupIndex].Count; 

          if (freqDict.TryGetValue(value, out int currentCount))
            freqDict[value] = currentCount + groupSize; //Прибавляем к счетчку частоты размер 
          else
            freqDict[value] = groupSize;
        }
        result[factor] = freqDict;
      }
      return result;
    }

    private static int GetMinFrequency(Dictionary<string, Dictionary<object, int>> frequencyInfo)
    {
      int result = int.MaxValue;
      foreach (var factorDict in frequencyInfo.Values)
      {
        foreach (int freq in factorDict.Values)
        {
          if (freq < result)
            result = freq;
        }
      }
      return result;
    }

    private static IDividedSample<GroupKey> PerformFilter(IDividedSample<GroupKey> source, Dictionary<string, HashSet<object>> uniqueValues, FactorInfo predictors)
    {
      int maxIterations = 100;
      var data = source;

      for (int iter = 0; iter < maxIterations; iter++)
      {
        var frequency = GetFrequencyMap(data, predictors); //Считаем частоты

        //Проверка полноты
        int cartesianSize = frequency.Values.Aggregate(1, (acc, dict) => acc * dict.Keys.Count);
        if (cartesianSize == data.Count)
          return GroupKey.Repack(data, predictors);

        int globalMinFreq = GetMinFrequency(frequency); //Поиск минимальной частоты

        //Сбор кандидатов в словарь
        var candidates = new Dictionary<string, List<object>>(); 
        foreach (var factor in predictors)
        {
          var factorDict = frequency[factor];
          foreach (var kv in factorDict)
          {
            if (kv.Value == globalMinFreq)
            {
              if (!candidates.ContainsKey(factor))
                candidates[factor] = new List<object>();

              candidates[factor].Add(kv.Key);
            }
          }
        }

        if (candidates.Count == 0)
          break;

        //Подсчёт общего числа наблюдений
        int totalObservations = 0;
        for (int i = 0; i < data.Count; i++)
          totalObservations += data[i].Count;

        //Оценка кандидатов
        string bestFactor = null;
        object bestValue = null;
        int bestDelta = int.MaxValue;
        int bestRemovedObservations = int.MaxValue;
        IDividedSample<GroupKey> bestData = null;

        foreach (var factor in candidates.Keys)
        {
          foreach (object value in candidates[factor])
          {
            //Фильтрация данных
            var newData = new List<IPlainSample>();
            var newKeys = new List<GroupKey>();

            for (int i = 0; i < data.Count; i++)
            {
              object currentValue = data.GetKey(i)[factor];
              if (!Equals(currentValue, value))
              {
                newData.Add(data[i]);
                newKeys.Add(data.GetKey(i));
              }
            }

            if (newData.Count == 0)
              continue;

            var newSample = new ArrayDividedSample<GroupKey>(newData.ToArray(), i => newKeys[i]);

            //Подсчёт удалённых наблюдений
            int newTotalObservations = 0;
            for (int i = 0; i < newSample.Count; i++)
              newTotalObservations += newSample[i].Count;

            int removedObservations = totalObservations - newTotalObservations;

            //Пересчёт декартового произведения
            var newFrequency = GetFrequencyMap(newSample, predictors);
            int newCartesianSize = 1;
            foreach (var dict in newFrequency.Values)
            {
              newCartesianSize *= dict.Count;
            }
            int delta = newCartesianSize - newSample.Count;

            //Выбор лучшего кандидата
            if (delta < bestDelta || (delta == bestDelta && removedObservations < bestRemovedObservations))
            {
              bestDelta = delta;
              bestRemovedObservations = removedObservations;
              bestFactor = factor;
              bestValue = value;
              bestData = newSample;
            }
          }
        }

        if (bestData == null)
          break;

        data = bestData;
      }

      return GroupKey.Repack(data, predictors);
    }

    // Проверка полноты Декартова произведения
    private static bool IsFull(IDividedSample<GroupKey> source, FactorInfo predictors)
    {
      var uniqueValues = new Dictionary<string, HashSet<object>>();

      if (source.Count == 0)
        return true;

      var bf = source.GetKey(0).BaseFilter;
      var rs = source.GetKey(0).Response;

      foreach (var p in predictors)
      {
        var set = new HashSet<object>();

        for (int i = 0; i < source.Count; i++)
          set.Add(source.GetKey(i)[p]);

        uniqueValues.Add(p, set);
      }

      var cm = new CartesianMultiplier<string, object>(
        uniqueValues.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()));

      foreach (var mul in cm)
      {
        bool ok = false;
        var g = new GroupKey(mul, bf, rs);

        for (int i = 0; i < source.Count; i++)
        {
          var key = source.GetKey(i);

          if (source.GetKey(i).GetSubKey(predictors).Equals(g))
          {
            ok = true;
            break;
          }
        }

        if (!ok)
          return false;
      }

      return true;
    }

    private struct EffectKey
    {
      public FactorInfo Factor;

      public int GradationCount;

      public override string ToString()
      {
        return string.Format("{0} -> {1}", this.Factor ?? FactorInfo.Empty, this.GradationCount);
      }

      public override bool Equals(object obj)
      {
        if (!(obj is EffectKey other))
          return false;

        return this.GradationCount == other.GradationCount 
          && object.Equals(this.Factor, other.Factor);
      }

      public override int GetHashCode()
      {
        return this.GradationCount ^ (this.Factor != null ? this.Factor.GetHashCode() : 0);
      }
    }
  }
}