using Notung;
using Notung.Services;
using Schicksal.Helm.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;

namespace Schicksal.Helm.Analyze
{
  /// <summary>
  /// Анализ данных с пользовательским интерфейсом
  /// </summary>
  public interface IAnalyze
  {
    /// <summary>
    /// В каком месте конфигурационного файла хранятся настройки анализа
    /// </summary>
    /// <returns>Ключом является конкатенация имён колонок, значением - массив значений параметров анализа</returns>
    Dictionary<string, string[]> GetSettings();

    /// <summary>
    /// Тип окна для дополнительных настроек анализа. Оно должно реализовывать интерфейс IAnalysisOptions
    /// </summary>
    Type OptionsType { get; }

    /// <summary>
    /// Получение настроенного экземпляра задачи на анализ
    /// </summary>
    /// <param name="table">Таблица с данными для анализа</param>
    /// <param name="data">Настройки анализа из стандартного диалога (+ дополнитеьные настройки, если есть)</param>
    /// <returns>Задача анализа указанных данных с указанными настройками</returns>
    IRunBase GetProcessor(DataTable table, StatisticsParameters data);

    /// <summary>
    /// Настройки диалога с индикатором прогресса (заголовок, картинка и т.п.)
    /// </summary>
    /// <returns>Настройки диалога с индикатором прогресса</returns>
    LaunchParameters GetLaunchParameters();

    /// <summary>
    /// Отображение результатов анализа
    /// </summary>
    /// <param name="processor">Выполненная задача на анализ данных</param>
    /// <param name="table_form">Окно, содержащее таблицу с анализируемыми данными</param>
    /// <param name="data">Настройки анализа из стандартного диалога (+ дополнитеьные настройки, если есть)</param>
    void BindTheResultForm(IRunBase processor, object table_form, StatisticsParameters data);
  }
}
