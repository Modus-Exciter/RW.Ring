using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Data
{
  public class PrefixTree
  {
    private readonly PrefixTreeItem m_root = new PrefixTreeItem('\0');

    public void AddPrefix(string prefix)
    {
      if (string.IsNullOrEmpty(prefix))
        throw new ArgumentNullException("prefix");

      var item = m_root;

      for (int i = 0; i < prefix.Length; i++)
        item = item.AddChild(prefix[i]);
    }

    public bool MatchAny(string fullString)
    {
      var item = m_root;

      for (int i = 0; i < fullString.Length; i++)
      {
        if (item.TryGetChild(fullString[i], out item) && item.IsLeaf)
          return true;
      }

      return false;
    }

    public void Clear()
    {
      m_root.Clear();
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

      public PrefixTreeItem AddChild(char symbol)
      {
        PrefixTreeItem child;

        if (!m_next_symbols.TryGetValue(symbol, out child))
        {
          child = new PrefixTreeItem(symbol);
          child.m_parent = this;
          m_next_symbols.Add(symbol, child);
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
        StringBuilder sb = new StringBuilder();

        var item = this;

        while (item.m_parent != null)
        {
          sb.Append(item.m_symbol);
          item = item.m_parent;
        }

        return new string(sb.ToString().Reverse().ToArray());
      }
    }
  }
}
