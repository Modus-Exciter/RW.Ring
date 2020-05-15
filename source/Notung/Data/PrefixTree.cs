using System;
using System.Collections.Generic;
using System.Linq;

namespace Notung.Data
{
  /// <summary>
  /// Класс для быстрого поиска префиксов строки
  /// </summary>
  public class PrefixTree
  {
    private readonly PrefixTreeItem m_root = new PrefixTreeItem('\0');
    private int m_count;

    /// <summary>
    /// Добавление префикса для поиска
    /// </summary>
    /// <param name="prefix"></param>
    public void AddPrefix(string prefix)
    {
      if (string.IsNullOrEmpty(prefix))
        throw new ArgumentNullException("prefix");

      var item = m_root;
      var addedNew = false;
      for (int i = 0; i < prefix.Length; i++)
        item = item.AddChild(prefix[i], out addedNew);

      if (addedNew)
        m_count++;
    }

    /// <summary>
    /// Проверяет, что переданная строка начинается с одного из заведённых префиксов
    /// </summary>
    /// <param name="fullString">Строка для проверки</param>
    /// <returns>Если строка начинается хотя бы с одного из префиксов, то true. Иначе, false</returns>
    public bool MatchAny(string fullString)
    {
      if (string.IsNullOrEmpty(fullString))
        throw new ArgumentNullException("fullString");
      
      var item = m_root;

      for (int i = 0; i < fullString.Length; i++)
      {
        if (item.TryGetChild(fullString[i], out item) && item.IsLeaf)
          return true;
        else if (item == null)
          break;
      }

      return false;
    }

    /// <summary>
    /// Очищает список префиксов
    /// </summary>
    public void Clear()
    {
      m_root.Clear();
      m_count = 0;
    }

    /// <summary>
    /// Получение массива всех префиксов, которые есть
    /// </summary>
    /// <returns>Все префиксы, ранее добавленные методом Add</returns>
    public string[] GetAllPrefixes()
    {
      LinkedList<PrefixTreeItem> list = new LinkedList<PrefixTreeItem>();
      Collect(m_root, list);

      var ret = new string[list.Count];

      int i = 0;

      foreach (var item in list)
        ret[i++] = item.ToString();

      return ret;
    }

    /// <summary>
    /// Общее количество префиксов
    /// </summary>
    public int Count
    {
      get { return m_count; }
    }

    private void Collect(PrefixTreeItem item, LinkedList<PrefixTreeItem> list)
    {
      foreach (var child in item.Children)
      {
        if (child.IsLeaf)
          list.AddLast(child);
        else
          Collect(child, list);
      }
    }

    private class PrefixTreeItem
    {
      private readonly char m_symbol;
      private readonly Dictionary<char, PrefixTreeItem> m_next_symbols = new Dictionary<char,PrefixTreeItem>();
      private PrefixTreeItem m_parent;

      public PrefixTreeItem(char symbol)
      {
        m_symbol = symbol;
      }

      public PrefixTreeItem AddChild(char symbol, out bool addedNew)
      {
        PrefixTreeItem child;

        addedNew = false;

        if (!m_next_symbols.TryGetValue(symbol, out child))
        {
          child = new PrefixTreeItem(symbol);
          child.m_parent = this;
          m_next_symbols.Add(symbol, child);
          addedNew = true;
        }

        return child;
      }

      public void Clear()
      {
        m_next_symbols.Clear();
      }

      public bool Contains(char symbol)
      {
        return m_next_symbols.ContainsKey(symbol);
      }

      public bool TryGetChild(char symbol, out PrefixTreeItem item)
      {
        return m_next_symbols.TryGetValue(symbol, out item);
      }

      public ICollection<PrefixTreeItem> Children
      {
        get { return m_next_symbols.Values; }
      }

      public PrefixTreeItem Parent
      {
        get { return m_parent; }
      }

      public PrefixTreeItem Root
      {
        get
        {
          var item = this;

          while (item.m_parent != null)
            item = item.m_parent;

          return item;
        }
      }

      public bool IsLeaf
      {
        get { return m_next_symbols.Count == 0; }
      }

      public override string ToString()
      {
        var list = new LinkedList<char>();
        var item = this;

        while (item.m_parent != null)
        {
          list.AddFirst(item.m_symbol);
          item = item.m_parent;
        }

        var ret = new char[list.Count];

        int i = 0;

        foreach (var symbol in list)
          ret[i++] = symbol;

        return new string(ret);
      }
    }
  }
}
