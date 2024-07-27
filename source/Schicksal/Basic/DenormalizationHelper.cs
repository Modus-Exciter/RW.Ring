using System.Diagnostics;

namespace Schicksal.Basic
{
  /// <summary>
  /// Вспомогательный класс с общей логикой получения денормированных данных
  /// </summary>
  public class DenormalizationHelper
  {
    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="sample">Выборка нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    public static IDenormalizer GetDenormalizer(ISample sample, IDenormalizerFactory factory)
    {
      Debug.Assert(sample != null && factory != null);

      var plain = sample as IPlainSample;
      var divided = sample as IDividedSample;
      var complex = sample as IComplexSample;

      if (sample != null && factory.IsNormalized(plain))
        return factory.GetDenormalizer(plain);

      if (divided != null && divided.Count > 0 && factory.IsNormalized(divided[0]))
        return factory.GetDenormalizer(divided);

      if (complex != null && complex.Count > 0 && complex[0].Count > 0 && factory.IsNormalized(complex[0][0]))
        return factory.GetDenormalizer(complex);

      return DummyNormalizer.Instance.GetDenormalizer(sample);
    }
  }
}
