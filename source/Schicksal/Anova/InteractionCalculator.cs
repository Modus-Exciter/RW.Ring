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

      int totalObservations = source.Sum(g => g.Count);
      if (source.Count == 0 || totalObservations == 0)
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
      var result = new Dictionary<string, Dictionary<object, int>>();

      foreach (var factor in predictors)
      {
        var freqDict = new Dictionary<object, int>();
        bool factorFound = false;

        for (int groupIndex = 0; groupIndex < source.Count; groupIndex++)
        {
          var key = source.GetKey(groupIndex);

          if (!key.FactorInfo.Contains(factor))  //Проверяем наличие фактора в ключе
            continue;

          factorFound = true;
          object value = key[factor];
          int groupSize = source[groupIndex].Count;

          if (freqDict.ContainsKey(value))
            freqDict[value] += groupSize;
          else
            freqDict[value] = groupSize;
        }

        if (factorFound)
          result[factor] = freqDict;
      }

      return result;
    }

    /// <summary>
    /// Выбор для каждого фактора его самой редкой градаций
    /// </summary>
    private static Dictionary<string, KeyValuePair<object, int>> GetMinFrequencyCandidates(Dictionary<string, Dictionary<object, int>> frequencyInfo, FactorInfo predictors)
    {
      var result = new Dictionary<string, KeyValuePair<object, int>>();

      foreach (var factor in predictors)
      {
        if (!frequencyInfo.ContainsKey(factor) || frequencyInfo[factor].Count == 0)
          continue;

        int minFreq = frequencyInfo[factor].Values.Min();
        var minEntry = frequencyInfo[factor].First(kv => kv.Value == minFreq);

        result[factor] = new KeyValuePair<object, int>(minEntry.Key, minEntry.Value);
      }

      return result;
    }

    /// <summary>
    /// Создание копии датасета без конкретной градации конкретного фактора. Возвращает разницу количества элементов исходного датасета и нового. Новый датасет заполняется в result
    /// </summary>
    private static int CreateDatasetWithoutLevelPrediction(IDividedSample<GroupKey> source, string predict, object predictValue, out IDividedSample<GroupKey> result)
    {
      int removedObservations = 0;
      var newGroups = new List<IPlainSample>();
      var newKeys = new List<GroupKey>();

      for (int i = 0; i < source.Count; i++)
      {
        var key = source.GetKey(i);
        if (Equals(key[predict], predictValue))
        {
          removedObservations += source[i].Count;
          continue;
        }

        newGroups.Add(source[i]);
        newKeys.Add(key);
      }

      result = new ArrayDividedSample<GroupKey>(
        data: newGroups.ToArray(),
        keys: index => newKeys.ToArray()[index]
      );

      return removedObservations;
    }
    private static IDividedSample<GroupKey> PerformFilter(IDividedSample<GroupKey> source, Dictionary<string, HashSet<object>> uniqueValues, FactorInfo predictors)
    {
      var data = source;
      int previousCount = 0;

      while (true)
      {
        //Репак данных для текущего состояния
        var repacked = GroupKey.Repack(data, predictors);

        if (IsFull(repacked, predictors))
          return repacked;

        var frequencyInfo = GetFrequencyMap(repacked, predictors); //Считаем частоты

        var minFrequencyCandidates = GetMinFrequencyCandidates(frequencyInfo, predictors); //Выбираем самые редкие градации каждого фактора

        if (minFrequencyCandidates.Count == 0)
          break;

        int minDifference = int.MaxValue;
        int bestRemovedObservations = int.MaxValue;
        IDividedSample<GroupKey> bestCandidate = null;
        KeyValuePair<string, object> bestFactor = default;

        foreach (var factor in minFrequencyCandidates) //Обходим все минимальные градации каждого фактора
        {
          IDividedSample<GroupKey> newDataset; //Копия датасета без минимальной градаци
          int removedObservations = CreateDatasetWithoutLevelPrediction(repacked, factor.Key, factor.Value.Key, out newDataset);

          //Расчет новой полноты
          var newFrequencyInfo = GetFrequencyMap(newDataset, predictors);
          int newCartesianSize = newFrequencyInfo.Values.Aggregate(1, (acc, dict) => acc * dict.Keys.Count);
          int difference = Math.Abs(newCartesianSize - newDataset.Count);

          //Если нашли полное декартово произведение - это результат
          if (difference == 0 && IsFull(newDataset, predictors))
            return newDataset;

          if (difference < minDifference || (difference == minDifference && removedObservations < bestRemovedObservations))
          {
            minDifference = difference;
            bestRemovedObservations = removedObservations;
            bestCandidate = newDataset;
            bestFactor = new KeyValuePair<string, object>(factor.Key, factor.Value.Key);
          }
        }

        if (bestCandidate == null)
          break;

        data = bestCandidate;  //Обновление данных для следующей итерации
        if (minDifference == 0 || data.Count == previousCount)
          return data;

        previousCount = data.Count;
      }
      return GroupKey.Repack(data, predictors);
    }

    // Проверка полноты Декартова произведения
    public static bool IsFull(IDividedSample<GroupKey> source, FactorInfo predictors)
    {
      if (source.Count == 0) return true;

      //Вычисляем ожидаемое количество комбинаций
      int expectedCount = 1;
      var uniqueValues = new Dictionary<string, HashSet<object>>();

      foreach (var p in predictors)
      {
        var set = new HashSet<object>();
        for (int i = 0; i < source.Count; i++)
        {
          set.Add(source.GetKey(i)[p]);
        }
        uniqueValues[p] = set;
        expectedCount *= set.Count;
      }
      return source.Count == expectedCount;
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