﻿using System.Diagnostics;

namespace Schicksal.Basic
{
  /// <summary>
  /// Извлечение заданного объекта из некоторых выборок
  /// </summary>
  /// <typeparam name="T">Тип объекта, который можно извлечь из некоторых выборок</typeparam>
  public interface ISamplePropertyExtractor<T>
  {
    /// <summary>
    /// Проверка возможности извлечения объекта из конкретной выборки
    /// </summary>
    /// <param name="sample">Выборка, из которой пытаемся извлечь объект</param>
    /// <returns>True, если возможно извлечь нужный объект из выборки. Иначе, False</returns>
    bool HasProperty(IPlainSample sample);

    /// <summary>
    /// Извлечение объекта из выборки
    /// </summary>
    /// <param name="sample">Выборка, из которой извлекается объект</param>
    /// <returns>Объект, ассоциированный с выборкой</returns>
    T Extract(IPlainSample sample);

    /// <summary>
    /// Извлечение объекта из выборки второго порядка
    /// </summary>
    /// <param name="sample">Выборка второго порядка, из которой извлекается объект</param>
    /// <returns>Объект, ассоциированный с выборкой второго порядка</returns>
    T Extract(IDividedSample sample);

    /// <summary>
    /// Извлечение объекта из выборки третьего порядка
    /// </summary>
    /// <param name="sample">Выборка третьего порядка, из которой извлекается объект</param>
    /// <returns>Объект, ассоциированный с выборкой третьего порядка</returns>
    T Extract(IComplexSample sample);
  }

  /// <summary>
  /// Вспомогательный класс с общей логикой получения денормированных данных
  /// </summary>
  public static class DenormalizationHelper
  {
    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="sample">Выборка нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    public static IDenormalizer GetDenormalizer(ISample sample, IDenormalizerFactory factory)
    {
      Debug.Assert(sample != null);
      Debug.Assert(factory != null);

      var plain = sample as IPlainSample;
      var divided = sample as IDividedSample;
      var complex = sample as IComplexSample;

      if (sample != null && factory.IsNormalized(plain))
        return factory.GetDenormalizer(plain);

      if (divided != null && divided.Count > 0 && factory.IsNormalized(divided[0]))
        return factory.GetDenormalizer(divided);

      if (complex != null && complex.Count > 0 && complex[0].Count > 0 && factory.IsNormalized(complex[0][0]))
        return factory.GetDenormalizer(complex);

      return DummyNormalizer.Denormalizer;
    }

    /// <summary>
    /// Обобщение логики денормирования на произвольное действие
    /// </summary>
    /// <typeparam name="T">Тип объекта, который можно извлечь из некоторых выборок</typeparam>
    /// <param name="sample">Выборка, из которой пытаемся извлечь</param>
    /// <param name="extractor">Конкретная реализация извлечения объекта</param>
    /// <param name="defaultValue">Объект по умолчанию, который из</param>
    /// <returns>Объект, извлечённый из выборки</returns>
    public static T ExtractProperty<T>(this ISamplePropertyExtractor<T> extractor, ISample sample) where T: class
    {
      Debug.Assert(sample != null);
      Debug.Assert(extractor != null);

      var plain = sample as IPlainSample;
      var divided = sample as IDividedSample;
      var complex = sample as IComplexSample;

      if (sample != null && extractor.HasProperty(plain))
        return extractor.Extract(plain);

      if (divided != null && divided.Count > 0 && extractor.HasProperty(divided[0]))
        return extractor.Extract(divided);

      if (complex != null && complex.Count > 0 && complex[0].Count > 0 && extractor.HasProperty(complex[0][0]))
        return extractor.Extract(complex);

      return null;
    }
  }
}
