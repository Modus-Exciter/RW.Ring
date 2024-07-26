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
    /// <param name="group">Выборка не нормированных данных</param>
    /// <returns>Выборка нормированных данных</returns>
    IDataGroup Normalize(IDataGroup group);

    /// <summary>
    /// Нормирование набора выборок
    /// </summary>
    /// <param name="group">Набор выборок не нормированных данных</param>
    /// <returns>Набор выборок нормированных данных</returns>
    IMultyDataGroup Normalize(IMultyDataGroup group);

    /// <summary>
    /// Нормирование нескольких наборов выборок
    /// </summary>
    /// <param name="group">Множество наборов выборок не нормированных данных</param>
    /// <returns>Множество наборов выборок нормированных данных</returns>
    ISetMultyDataGroup Normalize(ISetMultyDataGroup group);

    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="group">Выборка нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    IDenormalizer GetDenormalizer(IDataGroup group);

    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="group">Набор выборок нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    IDenormalizer GetDenormalizer(IMultyDataGroup group);

    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="group">Множество наборов выборок нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    IDenormalizer GetDenormalizer(ISetMultyDataGroup group);
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
  /// Заглушка для фиктивного нормирования данных
  /// </summary>
  public sealed class DummyNormalizer : INormalizer
  {
    private static readonly DummyNormalizer _instance = new DummyNormalizer();

    private DummyNormalizer() { }

    /// <summary>
    /// Экземпляр заглушки
    /// </summary>
    public static DummyNormalizer Instance
    {
      get { return _instance; }
    }

    /// <summary>
    /// Заглушка для фиктивного обратного преобразования нормированных данных
    /// </summary>
    public static IDenormalizer Denormalizer
    {
      get { return DummyDenormalizer.Instance; }
    }
    
    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="group">Выборка нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    public IDenormalizer GetDenormalizer(IDataGroup group)
    {
      return DummyDenormalizer.Instance;
    }

    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="group">Набор выборок нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    public IDenormalizer GetDenormalizer(IMultyDataGroup group)
    {
      return DummyDenormalizer.Instance;
    }

    /// <summary>
    /// Получение преобразователя для обратного нормирования данных
    /// </summary>
    /// <param name="group">Множество наборов выборок нормированных данных</param>
    /// <returns>Преобразователь нормированных значений в ненормированные</returns>
    public IDenormalizer GetDenormalizer(ISetMultyDataGroup group)
    {
      return DummyDenormalizer.Instance;
    }

    /// <summary>
    /// Нормирование одной выборки
    /// </summary>
    /// <param name="group">Выборка не нормированных данных</param>
    /// <returns>Выборка нормированных данных</returns>
    public IDataGroup Normalize(IDataGroup group)
    {
      return group;
    }

    /// <summary>
    /// Нормирование набора выборок
    /// </summary>
    /// <param name="group">Набор выборок не нормированных данных</param>
    /// <returns>Набор выборок нормированных данных</returns>
    public IMultyDataGroup Normalize(IMultyDataGroup group)
    {
      return group;
    }

    /// <summary>
    /// Нормирование нескольких наборов выборок
    /// </summary>
    /// <param name="group">Множество наборов выборок не нормированных данных</param>
    /// <returns>Множество наборов выборок нормированных данных</returns>
    public ISetMultyDataGroup Normalize(ISetMultyDataGroup group)
    {
      return group;
    }

    /// <summary>
    /// Строковое представление объекта
    /// </summary>
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
    public override int GetHashCode()
    {
      return this.GetType().GetHashCode();
    }

    private sealed class DummyDenormalizer : IDenormalizer
    {
      public static readonly DummyDenormalizer Instance = new DummyDenormalizer();

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