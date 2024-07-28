namespace Schicksal.Basic
{
  /// <summary>
  /// Объект для нормирования данных
  /// </summary>
  public interface INormalizer
  {
    /// <summary>
    /// Нормирование одной выборки
    /// </summary>
    /// <param name="sample">Выборка не нормированных данных</param>
    /// <returns>Выборка нормированных данных</returns>
    IPlainSample Normalize(IPlainSample sample);

    /// <summary>
    /// Нормирование набора выборок
    /// </summary>
    /// <param name="sample">Набор выборок не нормированных данных</param>
    /// <returns>Набор выборок нормированных данных</returns>
    IDividedSample Normalize(IDividedSample sample);

    /// <summary>
    /// Нормирование нескольких наборов выборок
    /// </summary>
    /// <param name="sample">Множество наборов выборок не нормированных данных</param>
    /// <returns>Множество наборов выборок нормированных данных</returns>
    IComplexSample Normalize(IComplexSample sample);

    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="sample">Выборка нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    IDenormalizer GetDenormalizer(ISample sample);
  }

  /// <summary>
  /// Преобразователь нормированных данных в не нормированные
  /// </summary>
  public interface IDenormalizer
  {
    /// <summary>
    /// Преобразование нормированного значения в ненормированное
    /// </summary>
    /// <param name="value">Нормированное значение</param>
    /// <returns>Не нормированное значение</returns>
    double Denormalize(double value);
  }

  /// <summary>
  /// Заглушка для фиктивного нормирования данных. Предназначена для алгоритмов,
  /// где используется нормирование, когда реальное нормирование не требуется
  /// </summary>
  public sealed class DummyNormalizer : INormalizer
  {
    private static readonly DummyNormalizer _instance = new DummyNormalizer();
    private static readonly DummyDenormalizer _denormalizer = new DummyDenormalizer();

    private DummyNormalizer() { }

    /// <summary>
    /// Экземпляр заглушки
    /// </summary>
    public static DummyNormalizer Instance
    {
      get { return _instance; }
    }

    /// <summary>
    /// Получение преобразователя нормированных значений в ненормированные
    /// </summary>
    /// <param name="sample">Выборка нормированных данных</param>
    /// <returns>Преобразователь, который никак не обрабатывает данные</returns>
    public IDenormalizer GetDenormalizer(ISample sample)
    {
      return _denormalizer;
    }

    /// <summary>
    /// Нормирование одной выборки
    /// </summary>
    /// <param name="sample">Выборка не нормированных данных</param>
    /// <returns>Выборка нормированных данных</returns>
    public IPlainSample Normalize(IPlainSample sample)
    {
      return sample;
    }

    /// <summary>
    /// Нормирование набора выборок
    /// </summary>
    /// <param name="sample">Набор выборок не нормированных данных</param>
    /// <returns>Набор выборок нормированных данных</returns>
    public IDividedSample Normalize(IDividedSample sample)
    {
      return sample;
    }

    /// <summary>
    /// Нормирование нескольких наборов выборок
    /// </summary>
    /// <param name="sample">Множество наборов выборок не нормированных данных</param>
    /// <returns>Множество наборов выборок нормированных данных</returns>
    public IComplexSample Normalize(IComplexSample sample)
    {
      return sample;
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

    private sealed class DummyDenormalizer : IDenormalizer
    {
      public double Denormalize(double value)
      {
        return value;
      }

      public override string ToString()
      {
        return "Dummy denormalizer(a => a)";
      }

      public override bool Equals(object obj)
      {
        return obj is DummyDenormalizer;
      }

      public override int GetHashCode()
      {
        return this.GetType().GetHashCode();
      }
    }
  }
}