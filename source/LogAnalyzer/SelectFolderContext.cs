using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using LogAnalyzer.Properties;
using Notung;
using Notung.ComponentModel;

namespace LogAnalyzer
{
  internal interface IDirectoryEntry
  {
    ReadOnlyCollection<DirectoryEntry> Children { get; }

    SelectFolderContext Root { get; }
  }

  public class SelectFolderContext : IDirectoryEntry
  {
    private readonly ReadOnlyCollection<DirectoryEntry> m_children;

    public SelectFolderContext()
    {
      m_children = new ReadOnlyCollection<DirectoryEntry>(new DirectoryEntry[]
      {
        new DirectoryEntry(Environment.GetFolderPath(Environment.SpecialFolder.MyComputer), this),
        new DirectoryEntry(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this),
        new DirectoryEntry(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), this),
        new DirectoryEntry(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), this)
      });
    }

    SelectFolderContext IDirectoryEntry.Root
    {
      get { return this; }
    }

    internal DirectoryEntry SelectedEntry { get; set; }

    public ReadOnlyCollection<DirectoryEntry> Children
    {
      get
      {
        var company = ApplicationInfo.Instance.Company;

        for (int i = 1; i < m_children.Count; i++)
        {
          if (m_children[i].Contains(company))
          {
            m_children[i].IsExpanded = true;
            m_children[i].Children.Single(c => c.ToString().Equals(company)).IsExpanded = true;
            break;
          }
        }

        return m_children;
      }
    }

    public string GetErrorText()
    {
      if (this.SelectedEntry == null)
        return "Выберите папку";

      var selectedPath = this.SelectedEntry.FullPath;

      if (string.IsNullOrEmpty(selectedPath))
        return "Выберите папку";

      if (!Directory.EnumerateFiles(selectedPath, "*.log").Any() &&
          Directory.Exists(Path.Combine(selectedPath, "Logs")))
        selectedPath = Path.Combine(selectedPath, "Logs");

      if (!Directory.EnumerateFiles(selectedPath, "*.log").Any())
        return "В указанной папке протоколы не найдены";

      return null;
    }

    private string GetLongTestString(int size)
    {
      byte[] bytes = new byte[size];
      char[] chars = "abcdefghijklmnoprstuvwxyz12345678990?<> \n\t".ToCharArray();
      var rnd = new Random();
      return new string(bytes.Select(b => chars[rnd.Next(chars.Length)]).ToArray());
    }
  }

  public class DirectoryEntry : ObservableObject, IDirectoryEntry
  {
    private readonly string m_path;
    private readonly string m_name;
    private readonly IDirectoryEntry m_parent;
    private ReadOnlyCollection<DirectoryEntry> m_children;
    private bool m_is_selected;
    private bool m_is_expanded;

    private static readonly Bitmap _monitor = Resources.Monitor;
    private static readonly Bitmap _disk_drive = Resources.DiskDrive;
    private static readonly Bitmap _folder = Resources.Folder;
    private static readonly Bitmap _folder_open = Resources.FolderOpen;

    internal DirectoryEntry(string path, IDirectoryEntry parent)
    {
      if (!string.IsNullOrEmpty(path) && (!Directory.Exists(path) || !Path.IsPathRooted(path)))
        throw new ArgumentException();

      if (parent == null)
        throw new ArgumentNullException("parent");

      m_parent = parent;
      m_path = path;
      m_name = Path.GetFileName(path);
    }

    public string FullPath
    {
      get { return m_path; }
    }

    public SelectFolderContext Root
    {
      get { return m_parent.Root; }
    }

    public bool IsSelected
    {
      get { return m_is_selected; }
      set
      {
        if (m_is_selected == value)
          return;

        m_is_selected = value;
        this.OnPropertyChanged("IsSelected");

        if (m_is_selected)
          m_parent.Root.SelectedEntry = this;
      }
    }

    public bool IsExpanded
    {
      get { return m_is_expanded; }
      set
      {
        if (m_is_expanded == value)
          return;

        m_is_expanded = value;

        this.OnPropertyChanged("IsExpanded");
        this.OnPropertyChanged("Image");

        if (m_is_expanded && !m_is_selected)
          this.IsSelected = true;
      }
    }

    public Bitmap Image
    {
      get
      {
        if (string.IsNullOrEmpty(m_path))
          return _monitor;

        if (string.IsNullOrEmpty(m_name))
          return _disk_drive;

        if (this.IsExpanded)
          return _folder_open;

        return _folder;
      }
    }

    public ReadOnlyCollection<DirectoryEntry> Children
    {
      get
      {
        if (m_children == null)
        {
          ReadOnlyCollection<DirectoryEntry> ret = null;

          if (string.IsNullOrEmpty(m_path))
          {
            var drives = Environment.GetLogicalDrives();
            var entries = new List<DirectoryEntry>(drives.Length);

            for (int i = 0; i < drives.Length; i++)
            {
              if (Directory.Exists(drives[i]))
                entries.Add(new DirectoryEntry(drives[i], this));
            }

            ret = new ReadOnlyCollection<DirectoryEntry>(entries);
          }
          else
          {
            var directories = Directory.GetDirectories(m_path);
            var list = new List<DirectoryEntry>(directories.Length);

            foreach (var folder in directories)
            {
              if (string.IsNullOrEmpty(m_name) && Path.GetFileName(folder) == "$RECYCLE.BIN")
                continue;

              if (CheckAccess(folder))
                list.Add(new DirectoryEntry(folder, this));
            }

            ret = new ReadOnlyCollection<DirectoryEntry>(list);
          }
          m_children = ret;
        }

        return m_children;
      }
    }

    public bool Contains(string name)
    {
      if (m_children != null)
        return m_children.Any(c => c.ToString().Equals(name));
      else
        return Directory.Exists(Path.Combine(m_path, name));
    }

    private static bool CheckAccess(string path)
    {
      try
      {
        var accessControlList = Directory.GetAccessControl(path);

        if (accessControlList == null)
          return false;

        bool allow = false;

        foreach (FileSystemAccessRule rule in accessControlList.GetAccessRules(
          true, true, typeof(System.Security.Principal.SecurityIdentifier)))
        {
          if ((rule.FileSystemRights & FileSystemRights.ListDirectory) == FileSystemRights.ListDirectory)
          {
            if (rule.AccessControlType == AccessControlType.Deny)
              return false;
            else if (rule.AccessControlType == AccessControlType.Allow)
              allow = true;
          }

          return allow;
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(m_path))
        return "Мой компьютер";

      if (string.IsNullOrEmpty(m_name))
        return m_path;

      return m_name;
    }
  }
}