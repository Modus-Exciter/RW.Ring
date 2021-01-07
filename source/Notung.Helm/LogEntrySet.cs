using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Notung.Logging;

namespace Notung.Helm
{
  public sealed class LogEntrySet : Component, IListSource
  {
    private readonly BindingList<LogEntry> m_entries = new BindingList<LogEntry>();
    private InfoLevel m_level = InfoLevel.Debug;

    public BindingList<LogEntry> Entries
    {
      get { return m_entries; }
    }

    public void Accept(string source, Info args)
    {
      LogEntry entry = new LogEntry();

      entry.Level = args.Level;
      entry.Source = source;
      entry.Message = args.Message;
      entry.Tag = args.Details;
      entry.EventDate = DateTime.Now;
      entry.InnerMessages = args.InnerMessages.ToArray();

      if (m_entries.Count > 0
        && m_entries.Count == AppManager.Configurator.GetSection<LogSection>().MaxLogBufferSize)
        m_entries.RemoveAt(0);

      m_entries.Add(entry);
    }

    public InfoLevel Accept(InfoBuffer buffer)
    {
      m_level = InfoLevel.Debug;
      int infoCounter = 0;
      try
      {
        foreach (var info in buffer)
        {
          this.CreateEntry(info, null, ref infoCounter);
        }
      }
      catch (Exception ex)
      {
        LogManager.GetLogger("Infolog").Error(ex.Message, ex);
      }

      return m_level;
    }

    private void CreateEntry(Info info, int? parentId, ref int infoCounter)
    {
      infoCounter++;

      if (infoCounter >= AppManager.Configurator.GetSection<LogSection>().MaxLogBufferSize)
      {
        var exc = new OverflowException(WinResources.BUFFER_OVER);
        LogEntry ent = new LogEntry();
        ent.Level = m_level = InfoLevel.Error;
        ent.Message = exc.Message;
        ent.ID = infoCounter;
        ent.Tag = exc;
        m_entries.Add(ent);
        throw exc;
      }

      LogEntry entry = new LogEntry();
      entry.Level = info.Level;
      entry.Message = info.Message;
      entry.Tag = info.Details;
      entry.ID = infoCounter;
      entry.ParentID = parentId;
      m_entries.Add(entry);

      if (m_level < info.Level)
        m_level = info.Level;

      foreach (var innerItem in info.InnerMessages)
      {
        this.CreateEntry(innerItem, entry.ID, ref infoCounter);
      }
    }

    #region IListSource Members

    bool IListSource.ContainsListCollection
    {
      get { return false; }
    }

    System.Collections.IList IListSource.GetList()
    {
      return m_entries;
    }

    #endregion
  }

  /// <summary>
  /// Класс для отображения записи протокола
  /// </summary>
  public class LogEntry
  {
    public DateTime EventDate { get; set; }

    public string Source { get; set; }

    public Image Icon
    {
      get
      {
        switch (this.Level)
        {
          case InfoLevel.Debug:
            return WinResources.p_16_debug;

          case InfoLevel.Info:
            return WinResources.p_16_info;

          case InfoLevel.Warning:
            return WinResources.p_16_warning;

          case InfoLevel.Error:
            return WinResources.p_16_error;
        }
        return null;

      }
    }

    public Image GetLargeIcon()
    {
      switch (this.Level)
      {
        case InfoLevel.Debug:
          return WinResources.p_48_debug;

        case InfoLevel.Info:
          return WinResources.p_48_info;

        case InfoLevel.Warning:
          return WinResources.p_48_warning;

        case InfoLevel.Error:
          return WinResources.p_48_error;

        default:
          return null;
      }
    }

    public Color GetColor()
    {
      switch (this.Level)
      {
        case InfoLevel.Debug:
          return AppManager.Configurator.GetSection<LogSection>().DebugColor;

        case InfoLevel.Info:
          return AppManager.Configurator.GetSection<LogSection>().InfoColor;

        case InfoLevel.Warning:
          return AppManager.Configurator.GetSection<LogSection>().WarningColor;

        case InfoLevel.Error:
          return AppManager.Configurator.GetSection<LogSection>().ErrorColor;
      }
      return Color.White;
    }

    public InfoBuffer CreateBuffer()
    {
      InfoBuffer buf = new InfoBuffer();
      Info root = buf.Add(this.Message, this.Level);
      root.Details = this.Tag;

      foreach (var info in this.InnerMessages)
        root.InnerMessages.Add(info);

      return buf;
    }

    public InfoLevel Level { get; set; }

    public string Message { get; set; }

    public object Tag { get; set; }

    public Info[] InnerMessages { get; set; }

    public int ID { get; set; }

    public int? ParentID { get; set; }
  }
}