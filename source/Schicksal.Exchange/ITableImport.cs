using System;
using System.Data;

namespace Schicksal.Exchange
{
  /// <summary>
  /// Интерфейс для импорта таблиц данных в программу Schicksal
  /// </summary>
  public interface ITableImport
  {
    /// <summary>
    /// Выполнение импорта
    /// </summary>
    /// <param name="context">Ссылка на главное окно программы</param>
    /// <returns>Результат импорта. Если импорт не удался, метод возвращает null</returns>
    ImportResult Import(object context);
  }

  /// <summary>
  /// Результат импорта данных в программу Schicksal
  /// </summary>
  [Serializable]
  public sealed class ImportResult
  {
    /// <summary>
    /// Импортированная таблица
    /// </summary>
    public DataTable Table { get; set; }

    /// <summary>
    /// Описание набора данных для импорта
    /// </summary>
    public string Description { get; set; }
  }
}
