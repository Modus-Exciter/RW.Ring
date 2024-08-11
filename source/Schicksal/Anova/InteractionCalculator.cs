using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

      var dic = new Dictionary<string, HashSet<object>>();

      foreach (var p in predictors)
      {
        var set = new HashSet<object>();

        for (int i = 0; i < source.Count; i++)
          set.Add(source.GetKey(i)[p]);

        dic.Add(p, set);
      }

      if (dic.Aggregate(1, (a, kv) => a * kv.Value.Count) == source.Count)
        return GroupKey.Repack(source, predictors);

      return PerformFilter(source, dic, predictors);
    }

    private static IDividedSample<GroupKey> PerformFilter(IDividedSample<GroupKey> source, Dictionary<string, HashSet<object>> dic, FactorInfo predictors)
    {
      string[] order = dic.OrderByDescending(kv => kv.Value.Count).Select(kv => kv.Key).ToArray();
      GroupKeyNode index = BuildIndex(source, order);
      List<object[]> data_list = CreateDataList(dic, order); // Градации первого фактора

      for (int p_idx = 1; p_idx < order.Length; p_idx++) // Обходим все оставшиеся факторы
      {
        // Ищем соответствие
        var wrong_indexes = new HashSet<int>();
        var wrong_values = new HashSet<object>();

        for (int i = 0; i < data_list.Count; i++) // Обходим уже сформированные
        {
          foreach (var value in dic[order[p_idx]]) // Обходим все градации нового фактора
          {
            data_list[i][p_idx] = value;

            if (!IndexContains(index, data_list[i], p_idx))
            {
              wrong_indexes.Add(i);
              wrong_values.Add(value);
            }
          }
        }

        var next_level = new List<object[]>(CalculateNextLevelCapacity(data_list.Count, 
          dic[order[p_idx]].Count, wrong_indexes.Count, wrong_values.Count));

        if (wrong_indexes.Count > 0)
          Sacrifice(dic[order[p_idx]], wrong_indexes, wrong_values, ref data_list, ref next_level);

        // Заменяем список
        data_list = Exchange(data_list, next_level, dic[order[p_idx]], p_idx);
      }

      // Формируем окончательный результат
      var twice = new TwiceGroupedSample(source, predictors);
      var list = new List<IPlainSample>();
      var keys = new List<GroupKey>();

      foreach (var values in data_list)
      {
        GroupKey key = GetKey(index, values);
        int idx = twice.GetIndex(key);

        if (idx >= 0)
        {
          list.AddRange(twice[idx]);

          for (int i = 0; i < twice[idx].Count; i++)
            keys.Add(((IDividedSample<GroupKey>)twice[idx]).GetKey(i));
        }
      }

      return new ArrayDividedSample<GroupKey>(list.ToArray(), i => keys[i]);
    }

    private static List<object[]> CreateDataList(Dictionary<string, HashSet<object>> dic, string[] order)
    {
      var data_list = new List<object[]>();

      foreach (var value in dic[order[0]]) // Обходим все градации самого первого фактора
      {
        data_list.Add(new object[dic.Count]);
        data_list[data_list.Count - 1][0] = value;
      }

      return data_list;
    }

    private static GroupKeyNode BuildIndex(IDividedSample<GroupKey> sample, string[] factorOrder)
    {
      var ret = new GroupKeyNode { Key = GroupKey.Empty };
      var factor_list = new List<string>(factorOrder.Length);
      var subs = new List<FactorInfo>(factorOrder.Length);

      foreach (var p in factorOrder)
      {
        factor_list.Add(p);
        subs.Add(new FactorInfo(factor_list));
      }

      for (int i = 0; i < sample.Count; i++)
      {
        var key = sample.GetKey(i);
        var node = ret;

        for (int j = 0; j < factorOrder.Length; j++)
        {
          var value = key[factorOrder[j]];

          GroupKeyNode sub_node;

          if (!node.Nodes.TryGetValue(value, out sub_node))
          {
            sub_node = new GroupKeyNode { Key = key.GetSubKey(subs[j]) };
            node.Nodes.Add(value, sub_node);
          }

          node = sub_node;
        }
      }

      return ret;
    }

    private static bool IndexContains(GroupKeyNode index, object[] values, int count)
    {
      for (int i = 0; i <= count; i++)
      {
        GroupKeyNode node;

        if (!index.Nodes.TryGetValue(values[i], out node))
          return false;

        index = node;
      }

      return true;
    }

    private static GroupKey GetKey(GroupKeyNode index, object[] values)
    {
      for (int i = 0; i < values.Length; i++)
      {
        GroupKeyNode node;

        if (!index.Nodes.TryGetValue(values[i], out node))
          throw new KeyNotFoundException();

        index = node;
      }

      return index.Key;
    }

    private static int CalculateNextLevelCapacity(int leftSize, int rightSize, int leftError, int rightError)
    {
      if ((float)leftError / leftSize < (float)rightError / rightSize)
        return (leftSize - leftError) * rightSize;
      else
        return leftSize * (rightSize - rightError);
    }

    private static void Sacrifice(HashSet<object> set, HashSet<int> wrong_indexes, HashSet<object> wrong_values, ref List<object[]> data_list, ref List<object[]> next_level)
    {
      next_level = new List<object[]>();

      // Выбираем, чем пожертвовать
      if ((float)wrong_indexes.Count / data_list.Count < (float)wrong_values.Count / set.Count)
      {
        for (int i = 0; i < data_list.Count; i++) // Жертвуем градациями уже обработанных факторов
        {
          if (!wrong_indexes.Contains(i))
            next_level.Add(data_list[i]);
        }

        var tmp = data_list;
        data_list = next_level;
        next_level = tmp;
        next_level.Clear();
      }
      else
        set.ExceptWith(wrong_values); // Жертвуем градациями нового фактора
    }

    private static List<object[]> Exchange(List<object[]> data_list, List<object[]> nextLevel, HashSet<object> set, int index)
    {
      bool one_was = false;

      foreach (var value in set)
      {
        for (int i = 0; i < data_list.Count; i++)
        {
          var array = data_list[i];

          if (one_was)
            array = array.ToArray();

          array[index] = value;
          nextLevel.Add(array);
        }

        one_was = true;
      }

      return nextLevel;
    }

    private class GroupKeyNode
    {
      private readonly Dictionary<object, GroupKeyNode> m_nodes = new Dictionary<object, GroupKeyNode>();

      public GroupKey Key { get; set; }

      public Dictionary<object, GroupKeyNode> Nodes
      {
        get { return m_nodes; }
      }

      public override string ToString()
      {
        return this.Key != null ? this.Key.ToString() : base.ToString();
      }
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