using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Schicksal.Basic
{
  /// <summary>
  /// Объект для нормирования данных
  /// </summary>
  public interface INormalizer
  {
    /// <summary>
    /// Настройка преобразователя для нормирования данных
    /// </summary>
    /// <param name="sample">Выборка нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    IValueTransform Prepare(ISample sample);
  }

  /// <summary>
  /// Преобразователь ненормированных данных в нормированные и обратно
  /// </summary>
  public interface IValueTransform
  {
    /// <summary>
    /// Преобразование ненормированного значения в нормированное
    /// </summary>
    /// <param name="value">Не нормированное значение</param>
    /// <returns>Нормированное значение</returns>
    double Normalize(double value);

    /// <summary>
    /// Преобразование нормированного значения в ненормированное
    /// </summary>
    /// <param name="value">Нормированное значение</param>
    /// <returns>Не нормированное значение</returns>
    double Denormalize(double value);
  }

  /// <summary>
  /// Расширяющие методы для упрощённого создания нормированных выборок
  /// </summary>
  public static class NormalizerExtensions
  {
    /// <summary>
    /// Преобразование выборки в нормированную выборку
    /// </summary>
    /// <param name="normalizer">Объект для нормирования данных</param>
    /// <param name="sample">Выборка</param>
    /// <returns>Выборка нормированных данных</returns>
    public static IPlainSample Normalize(this INormalizer normalizer, IPlainSample sample)
    {
      return Prepare(normalizer, sample).Normalize(sample);
    }

    /// <summary>
    /// Преобразование выборки в нормированную выборку
    /// </summary>
    /// <param name="normalizer">Объект для нормирования данных</param>
    /// <param name="sample">Выборка</param>
    /// <returns>Выборка нормированных данных</returns>
    public static IDividedSample Normalize(this INormalizer normalizer, IDividedSample sample)
    {
      return Prepare(normalizer, sample).Normalize(sample);
    }

    /// <summary>
    /// Преобразование выборки в нормированную выборку
    /// </summary>
    /// <param name="normalizer">Объект для нормирования данных</param>
    /// <param name="sample">Выборка</param>
    /// <returns>Выборка нормированных данных</returns>
    public static IComplexSample Normalize(this INormalizer normalizer, IComplexSample sample)
    {
      return Prepare(normalizer, sample).Normalize(sample);
    }

    /// <summary>
    /// Преобразование выборки в нормированную выборку
    /// </summary>
    /// <param name="transform">Объект для нормирования данных</param>
    /// <param name="sample">Выборка</param>
    /// <returns>Выборка нормированных данных</returns>
    public static IPlainSample Normalize(this IValueTransform transform, IPlainSample sample)
    {
      CheckParameters(transform, sample);

      if (transform.Equals(DummyNormalizer.Transform))
        return sample;

      var ns = sample as NormalizedSample;

      if (ns != null && ns.ValueTransform.Equals(transform))
        return sample;

      return new NormalizedSample(sample, transform);
    }

    /// <summary>
    /// Преобразование выборки в нормированную выборку
    /// </summary>
    /// <param name="transform">Объект для нормирования данных</param>
    /// <param name="sample">Выборка</param>
    /// <returns>Выборка нормированных данных</returns>
    public static IDividedSample Normalize(this IValueTransform transform, IDividedSample sample)
    {
      CheckParameters(transform, sample);

      if (transform.Equals(DummyNormalizer.Transform))
        return sample;

      if (RecreateRequired(sample, transform))
      {
        var samples = new IPlainSample[sample.Count];

        for (int i = 0; i < samples.Length; i++)
          samples[i] = new NormalizedSample(sample[i], transform);

        return new ArrayDividedSample(samples);
      }

      return sample;
    }

    /// <summary>
    /// Преобразование выборки в нормированную выборку
    /// </summary>
    /// <param name="transform">Объект для нормирования данных</param>
    /// <param name="sample">Выборка</param>
    /// <returns>Выборка нормированных данных</returns>
    public static IComplexSample Normalize(this IValueTransform transform, IComplexSample sample)
    {
      CheckParameters(transform, sample);

      if (transform.Equals(DummyNormalizer.Transform))
        return sample;

      if (RecreateRequired(sample, transform))
      {
        var array = new IDividedSample[sample.Count];

        for (int i = 0; i < array.Length; i++)
        {
          var samples = new IPlainSample[sample[i].Count];

          for (int j = 0; j < samples.Length; j++)
            samples[j] = new NormalizedSample(sample[i][j], transform);

          array[i] = new ArrayDividedSample(samples);
        }

        return new ArrayComplexSample(array);
      }

      return sample;
    }

    #region Implementation ------------------------------------------------------------------------

    private static bool RecreateRequired(IDividedSample sample, IValueTransform transform)
    {
      for (int i = 0; i < sample.Count; i++)
      {
        var ns = sample[i] as NormalizedSample;

        if (ns == null || !ns.ValueTransform.Equals(transform))
          return true;
      }

      return false;
    }

    private static bool RecreateRequired(IComplexSample sample, IValueTransform transform)
    {
      for (int i = 0; i < sample.Count; i++)
      {
        for (int j = 0; j < sample[i].Count; j++)
        {
          var ns = sample[i][j] as NormalizedSample;

          if (ns == null || !ns.ValueTransform.Equals(transform))
            return true;
        }
      }

      return false;
    }

    private static IValueTransform Prepare(INormalizer normalizer, ISample sample)
    {
      if (normalizer == null)
        throw new ArgumentNullException("normalizer");

      if (sample == null)
        throw new ArgumentNullException("sample");

      if (sample.Count == 0)
        return DummyNormalizer.Transform;

      return normalizer.Prepare(sample);
    }

    private static void CheckParameters(IValueTransform transform, ISample sample)
    {
      if (transform == null)
        throw new ArgumentNullException("transform");

      if (sample == null)
        throw new ArgumentNullException("sample");
    }

    #endregion
  }

  /// <summary>
  /// Нормированная выборка
  /// </summary>
  public sealed class NormalizedSample : IPlainSample
  {
    private readonly IPlainSample m_sample;
    private readonly IValueTransform m_transform;
    private Func<double, double> m_handler;

    /// <summary>
    /// Инициализация нормированной выборки
    /// </summary>
    /// <param name="sample">Исходная выборка</param>
    /// <param name="transform">Преобразователь ненормированных данных в нормированные</param>
    public NormalizedSample(IPlainSample sample, IValueTransform transform)
    {
      if (sample is null)
        throw new ArgumentNullException("sample");

      if (transform is null)
        throw new ArgumentNullException("transform");

      m_sample = sample;
      m_transform = transform;
    }

    /// <summary>
    /// Обращение к элементу выборки по номеру
    /// </summary>
    /// <param name="index">Порядковый номер элемента выборки</param>
    /// <returns>Нормированное значение элемента выборки</returns>
    public double this[int index]
    {
      get { return m_transform.Normalize(m_sample[index]); }
    }

    /// <summary>
    /// Объём выборки
    /// </summary>
    public int Count
    {
      get { return m_sample.Count; }
    }

    /// <summary>
    /// Преобразователь ненормированных данных в нормированные и обратно
    /// </summary>
    public IValueTransform ValueTransform
    {
      get { return m_transform; }
    }

    /// <summary>
    /// Исходные (не нормированные) данные
    /// </summary>
    public IPlainSample Source
    {
      get { return m_sample; }
    }

    /// <summary>
    /// Строковое представление объекта
    /// </summary>
    /// <returns>Информация о выборке и её трансформации</returns>
    public override string ToString()
    {
      return string.Format("{0}, transform: {1}", m_sample, m_transform);
    }

    /// <summary>
    /// Сравнение нормированной выборки с другим объектом
    /// </summary>
    /// <param name="obj">Другой объект</param>
    /// <returns>True, если другой объект - это такая же нормированная выборка. Иначе, False</returns>
    public override bool Equals(object obj)
    {
      var other = obj as NormalizedSample;

      if (other == null)
        return false;

      return m_sample.Equals(other.m_sample) && m_transform.Equals(m_transform);
    }

    /// <summary>
    /// Получение хеш-кода для нормированной выборки
    /// </summary>
    /// <returns>Хеш-код типа данных</returns>
    public override int GetHashCode()
    {
      return m_sample.GetHashCode() ^ m_transform.GetHashCode();
    }

    /// <summary>
    /// Возвращает итератор, выполняющий перебор нормированных значений в выборке
    /// </summary>
    /// <returns>Итератор, который можно использовать для обхода выборки</returns>
    public IEnumerator<double> GetEnumerator()
    {
      if (m_handler == null)
        m_handler = m_transform.Normalize;

      return m_sample.Select(m_handler).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }

  /// <summary>
  /// Заглушка для фиктивного нормирования данных. Предназначена для алгоритмов,
  /// где используется нормирование, когда реальное нормирование не требуется
  /// </summary>
  public sealed class DummyNormalizer : INormalizer
  {
    private static readonly DummyNormalizer _instance = new DummyNormalizer();
    private static readonly DummyTransform _denormalizer = new DummyTransform();

    private DummyNormalizer() { }

    /// <summary>
    /// Экземпляр заглушки
    /// </summary>
    public static DummyNormalizer Instance
    {
      get { return _instance; }
    } 
    
    /// <summary>
    /// Экземпляр преобразователя-заглушки
    /// </summary>
    public static IValueTransform Transform
    {
      get { return _denormalizer; }
    }

    /// <summary>
    /// Получение преобразователя нормированных значений в ненормированные
    /// </summary>
    /// <param name="sample">Выборка нормированных данных</param>
    /// <returns>Преобразователь, который никак не обрабатывает данные</returns>
    public IValueTransform Prepare(ISample sample)
    {
      return _denormalizer;
    }

    /// <summary>
    /// Строковое представление объекта
    /// </summary>
    /// <returns>Dummy normalizer(a => a)</returns>
    public override string ToString()
    {
      return "Dummy normalizer(a => a)";
    }

    /// <summary>
    /// Сравнение преобразователя с другим объектом
    /// </summary>
    /// <param name="obj">Другой объект</param>
    /// <returns>True, если другой объект - это тоже DummyNormalizer. Иначе, False</returns>
    public override bool Equals(object obj)
    {
      return obj is DummyNormalizer;
    }

    /// <summary>
    /// Получение хеш-кода для преобразователя
    /// </summary>
    /// <returns>Хеш-код типа данных</returns>
    public override int GetHashCode()
    {
      return this.GetType().GetHashCode();
    }

    #region Implementation ------------------------------------------------------------------------

    private sealed class DummyTransform : IValueTransform
    {
      public double Normalize(double value)
      {
        return value;
      }

      public double Denormalize(double value)
      {
        return value;
      }

      public override string ToString()
      {
        return "Dummy transform(a => a)";
      }

      public override bool Equals(object obj)
      {
        return obj is DummyTransform;
      }

      public override int GetHashCode()
      {
        return this.GetType().GetHashCode();
      }
    }

    #endregion
  }
}