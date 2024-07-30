using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Schicksal.Basic
{
  /// <summary>
  /// Набор значений колонок, идентифицирующий выборку в таблице
  /// </summary>
  public sealed class GroupKey : IEnumerable<KeyValuePair<string, object>>
  {
    private readonly Dictionary<string, object> m_data;
    private readonly string m_base_filter;
    private readonly string m_query;
    private readonly string m_response;

    private FactorInfo m_factor_info;

    private static readonly Func<KeyValuePair<string, object>, KeyValuePair<string, object>> _omit_nulls =
      kv => new KeyValuePair<string, object>(kv.Key, OmitNulls(kv.Value));

    /// <summary>
    /// Инициализация набора значений колонок
    /// </summary>
    /// <param name="parameters">Таблица с фильтром, из которой делается выборка</param>
    /// <param name="data">Конкретные значения колонок</param>
    public GroupKey(PredictedResponseParameters parameters, Dictionary<string, object> data)
    {
      if (parameters is null)
        throw new ArgumentNullException("parameters");

      if (data is null)
        throw new ArgumentNullException("data");

      foreach (var kv in data)
      {
        if (!parameters.Predictors.Contains(kv.Key))
          throw new ArgumentException(string.Format("Column {0} not found in the table", kv.Key));
      }

      m_base_filter = parameters.Filter;
      m_response = parameters.Response;
      m_data = data;
      m_query = this.GetQueryText();
    }

    private GroupKey(Dictionary<string, object> data, string baseFilter, string response)
    {
      m_data = data;
      m_base_filter = baseFilter;
      m_response = response;
      m_query = this.GetQueryText();
    }

    /// <summary>
    /// Значение конкретной колонки таблицы
    /// </summary>
    /// <param name="column">Имя колонки</param>
    /// <returns>Значение указанной колонки</returns>
    public object this[string column]
    {
      get { return OmitNulls(m_data[column]); }
    }

    /// <summary>
    /// Фильтр, применённый к таблице
    /// </summary>
    public string BaseFilter
    {
      get { return m_base_filter; }
    }

    /// <summary>
    /// Имя колонки, используемой для хранения отклика на воздействие факторов
    /// </summary>
    public string Response
    {
      get { return m_response; }
    }

    /// <summary>
    /// Текст запроса к таблице, чтобы получить выборку
    /// </summary>
    public string Query
    {
      get { return m_query; }
    }

    /// <summary>
    /// Количество значений колонки
    /// </summary>
    public int Count
    {
      get { return m_data.Count; }
    }

    /// <summary>
    /// Информация о списке предикторов в значениях колонок
    /// </summary>
    public FactorInfo FactorInfo
    {
      get
      {
        if (m_factor_info == null)
          m_factor_info = new FactorInfo(m_data.Keys);

        return m_factor_info;
      }
    }

    /// <summary>
    /// Возвращает итератор, позволяющий обойти значения колонок
    /// </summary>
    /// <returns>Итератор, позволяющий обойти значения колонок</returns>
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return m_data.Select(_omit_nulls).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    /// <summary>
    /// Сравнение наборов значений колонок на равенство
    /// </summary>
    /// <param name="obj">Другой объект, с которым делается сравнение</param>
    /// <returns>True, если наборы значений колонок совпадают. Иначе, False</returns>
    public override bool Equals(object obj)
    {
      var other = obj as GroupKey;

      if (other == null)
        return false;

      if (!object.Equals(m_base_filter, other.m_base_filter))
        return false;

      if (!object.Equals(m_response, other.m_response))
        return false;

      if (m_data.Count != other.m_data.Count)
        return false;

      foreach (var kv in m_data)
      {
        object value;

        if (!other.m_data.TryGetValue(kv.Key, out value))
          return false;

        if (!object.Equals(OmitNulls(kv.Value), OmitNulls(value)))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Хеш-функция для набора значений колонок
    /// </summary>
    /// <returns>Хеш функцию от колонки отклика, фильтра и всех значений колонок</returns>
    public override int GetHashCode()
    {
      var ret = m_base_filter == null ? m_response.GetHashCode() :
        m_response.GetHashCode() ^ m_base_filter.GetHashCode();

      foreach (var kv in m_data)
        ret ^= (kv.Key.GetHashCode() ^ OmitNulls(kv.Value).GetHashCode());

      return ret;
    }

    /// <summary>
    /// Возвращает строковое представление набора значений колонок
    /// </summary>
    /// <returns>Текст запроса для осуществления выборки из таблицы</returns>
    public override string ToString()
    {
      return m_query;
    }

    /// <summary>
    /// Получение подмножества значений колонок по списку кололонок
    /// </summary>
    /// <param name="factor">Список значений колонок, которые надо включить в подмножество</param>
    /// <returns>Подмножество значений указанных колонок</returns>
    public GroupKey GetSubKey(FactorInfo factor)
    {
      var dic = new Dictionary<string, object>();

      foreach (var p in factor)
        dic[p] = m_data[p];

      var new_key = new GroupKey(dic, m_base_filter, m_response);
      return new_key;
    }

    /// <summary>
    /// Получение строкового представления произвольного значения ячейки таблицы для запроса
    /// </summary>
    /// <param name="value">Значение ячейки таблицы</param>
    /// <returns>Текстовое представление значения ячейки</returns>
    public static string GetInvariant(object value)
    {
      var formattable = value as IFormattable;

      if (value is string || value is char)
        return string.Format("'{0}'", value.ToString().Replace("'", "''"));
      else if (value is DateTime)
        return string.Format("#{0}#", ((DateTime)value).ToString(CultureInfo.InvariantCulture));
      else if (value is DBNull)
        return "NULL";
      else if (formattable != null)
        return formattable.ToString(null, CultureInfo.InvariantCulture);
      else if (value != null)
        return value.ToString();
      else
        return "NULL";
    }

    /// <summary>
    /// Перепаковка выборки второго порядка по подмножеству колонок
    /// </summary>
    /// <param name="sample">Выборка, сгруппированная по значениям колонок для запроса</param>
    /// <param name="factor">Подмножество колонок для запроса</param>
    /// <returns>Выборка, сгруппированная по подмножеству колонок для запроса</returns>
    public static IDividedSample<GroupKey> Repack(IDividedSample<GroupKey> sample, FactorInfo factor)
    {
      if (factor is null)
        throw new ArgumentNullException("factor");

      if (sample is null)
        throw new ArgumentNullException("sample");

      var first_key = sample.GetKey(0);

      if (factor.Any(p => !first_key.m_data.ContainsKey(p)))
        throw new ArgumentException();

      if (sample.Count == 0)
        return sample;

      if (factor.Count == first_key.FactorInfo.Count)
        return sample;

      Dictionary<GroupKey, List<double>> tmp = CreateRepackData(sample, factor);

      GroupKey[] keys = new GroupKey[tmp.Count];
      double[][] values = new double[tmp.Count][];

      int index = 0;

      foreach (var kv in tmp)
      {
        keys[index] = kv.Key;
        values[index++] = kv.Value.ToArray();
      }

      return new ArrayDividedSample<GroupKey>(values, keys);
    }

    private static Dictionary<GroupKey, List<double>> CreateRepackData(IDividedSample<GroupKey> sample, FactorInfo factor)
    {
      var tmp = new Dictionary<GroupKey, List<double>>();

      for (int i = 0; i < sample.Count; i++)
      {
        GroupKey key = sample.GetKey(i);
        GroupKey new_key = key.GetSubKey(factor);
        List<double> list;

        if (tmp.TryGetValue(new_key, out list))
          list.AddRange(sample[i]);
        else
          tmp.Add(new_key, sample[i].ToList());
      }

      return tmp;
    }

    private string GetQueryText()
    {
      var sb = new StringBuilder();

      sb.AppendFormat("[{0}] IS NOT NULL", m_response);

      foreach (var kv in m_data)
      {
        if (OmitNulls(kv.Value) is DBNull)
          sb.AppendFormat(" AND [{0}] IS NULL", kv.Key);
        else
          sb.AppendFormat(" AND [{0}] = {1}", kv.Key, GetInvariant(kv.Value));
      }

      if (!string.IsNullOrWhiteSpace(m_base_filter))
        sb.AppendFormat(" AND {0}", m_base_filter);

      return sb.ToString();
    }

    private static object OmitNulls(object value)
    {
      return value == null ? Convert.DBNull : value;
    }
  }
}