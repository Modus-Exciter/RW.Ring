using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using Notung.ComponentModel;
using Notung.Configuration;

namespace Notung.Helm
{
  [DataContract]
  public class LogSection : ConfigurationSection
  {
    /// <summary>
    /// Цвет отладочных сообщенй
    /// </summary>
    [DataMember]
    [DefaultValue(typeof(Color), "Silver")]
    [DisplayNameRes("DebugColor", typeof(LogSection))]
    public Color DebugColor { get; set; }

    /// <summary>
    /// Цвет информационных сообщений
    /// </summary>
    [DataMember]
    [DefaultValue(typeof(Color), "Blue")]
    [DisplayNameRes("InfoColor", typeof(LogSection))]
    public Color InfoColor { get; set; }

    /// <summary>
    /// Цвет предупреждений
    /// </summary>
    [DataMember]
    [DefaultValue(typeof(Color), "Orange")]
    [DisplayNameRes("WarningColor", typeof(LogSection))]
    public Color WarningColor { get; set; }

    /// <summary>
    /// Цвет сообщений об ошибке
    /// </summary>
    [DataMember]
    [DefaultValue(typeof(Color), "Red")]
    [DisplayNameRes("ErrorColor", typeof(LogSection))]
    public Color ErrorColor { get; set; }

    /// <summary>
    /// Минимальный уровень протоколирования, перехватываемый в пользовательском интерфейсе
    /// </summary>
    [DataMember]
    [DefaultValue(InfoLevel.Info)]
    [DisplayNameRes("Treshold", typeof(LogSection))]
    public InfoLevel Treshold { get; set; }

    /// <summary>
    /// Максимальное количество сообщений в таблице
    /// </summary>
    [DataMember]
    [DefaultValue(100)]
    [DisplayNameRes("MaxLogBufferSize", typeof(LogSection))]
    public int MaxLogBufferSize { get; set; }

    /// <summary>
    /// Максимальное количество сообщений, которые пишутся в один файл
    /// </summary>
    [DataMember]
    [DefaultValue(100)]
    [DisplayNameRes("MaxLogFileSize", typeof(LogSection))]
    public int MaxLogFileSize { get; set; }

    /// <summary>
    /// Максимальное количество файлов лога
    /// </summary>
    [DataMember]
    [DefaultValue(100)]
    [DisplayNameRes("MaxLogFileCount", typeof(LogSection))]
    public int MaxLogFileCount { get; set; }

    /// <summary>
    /// Путь к нулевому файлу лога
    /// </summary>
    [DataMember]
    [DefaultValue("log.log")]
    [DisplayNameRes("FileRoot", typeof(LogSection))]
    public string FileRoot { get; set; }

    /// <summary>
    /// Разделитель записей лога
    /// </summary>
    [DataMember]
    [DefaultValue("/*========================")]
    [DisplayNameRes("OuterSplitterStart", typeof(LogSection))]
    public string OuterSplitterStart { get; set; }

    /// <summary>
    /// Разделитель записей лога
    /// </summary>
    [DataMember]
    [DefaultValue("========================*/")]
    [DisplayNameRes("OuterSplitterEnd", typeof(LogSection))]
    public string OuterSplitterEnd { get; set; }

    /// <summary>
    /// Разделитель внутри записей лога
    /// </summary>
    [DataMember]
    [DefaultValue("--------------------------")]
    [DisplayNameRes("InnerSplitter", typeof(LogSection))]
    public string InnerSplitter { get; set; }
  }
}
