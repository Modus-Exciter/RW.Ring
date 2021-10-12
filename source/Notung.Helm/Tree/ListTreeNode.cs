using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Notung.Helm.Tree
{
  class ListTreeNode : TreeNode
  {
    IBindingList m_list;
    Type m_item_type;

    public IBindingList List
    {
      get { return m_list; }
      set
      {
        if (ReferenceEquals(m_list, value))
          return;

        m_item_type = null;
        if (m_list != null)
        {
          m_list.ListChanged -= this.OnListChanged;
        }
        m_list = value;
        base.ContextMenuStrip = null;
        if (value != null)
        {
          foreach (MethodInfo mi in m_list.GetType().GetMethods())
          {
            var parms = mi.GetParameters();
            if (mi.Name == "Add" && parms.Length == 1)
            {
              m_item_type = parms[0].ParameterType;
              break;
            }
          }
          value.ListChanged += this.OnListChanged;
        }
      }
    }

    public override ContextMenuStrip ContextMenuStrip
    {
      get
      {
        this.CreateMenu();
        return base.ContextMenuStrip;
      }
      set
      {
        base.ContextMenuStrip = value;
      }
    }

    private void CreateMenu()
    {
      if (base.ContextMenuStrip != null)
        return;

      if (m_list == null || m_item_type == null)
        return;

      base.ContextMenuStrip = new ContextMenuStrip();

      if (!TreeBindingController.GetControllersForTree(this.TreeView).Any(c => c.ShowMenu))
        return;

      if (this.TreeView != null && this.TreeView.Container != null)
        this.TreeView.Container.Add(this.ContextMenuStrip);

      var types = this.GetKnownTypes(m_item_type);
      if (types.Count == 1)
      {
        base.ContextMenuStrip.Items.Add(CreateAddButton(types.First(), CoreResources.NEW + " "));
      }
      else if (types.Count > 1)
      {
        ToolStripMenuItem group = new ToolStripMenuItem(CoreResources.NEW);
        base.ContextMenuStrip.Items.Add(group);

        foreach (var type in types)
        {
          group.DropDownItems.Add(this.CreateAddButton(type, ""));
        }
      }
      base.ContextMenuStrip.Items.Add(this.CreateClearButton());

      ListTreeNode parentNode = this.Parent as ListTreeNode;
      if (parentNode != null && parentNode.List != null)
      {
        base.ContextMenuStrip.Items.Add(CoreResources.DELETE, null, delegate
        {
          parentNode.List.Remove(this.Tag);
        });
      }
    }

    private HashSet<Type> GetKnownTypes(Type baseType)
    {
      var ret = new HashSet<Type>();

      if (baseType != null && !baseType.IsAbstract
        && baseType.GetConstructor(Type.EmptyTypes) != null)
        ret.Add(baseType);

      foreach (KnownTypeAttribute att in baseType.GetCustomAttributes(typeof(KnownTypeAttribute), true))
      {
        if (att.Type != null && !att.Type.IsAbstract
          && att.Type.GetConstructor(Type.EmptyTypes) != null)
        {
          ret.Add(att.Type);
        }

        if (!string.IsNullOrEmpty(att.MethodName))
        {
          MethodInfo mi = baseType.GetMethod(att.MethodName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            Type.DefaultBinder, Type.EmptyTypes, new ParameterModifier[0]);

          if (mi != null && typeof(IEnumerable<Type>).IsAssignableFrom(mi.ReturnType))
          {
            foreach (var type in (IEnumerable<Type>)mi.Invoke(null, null))
            {
              if (!type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null)
                ret.Add(type);
            }
          }
        }
      }

      return ret;
    }

    private ToolStripMenuItem CreateClearButton()
    {
      ToolStripMenuItem ret = new ToolStripMenuItem(CoreResources.CLEAR);

      ret.Click += delegate
      {
        m_list.Clear();
      };

      return ret;
    }

    private ToolStripMenuItem CreateAddButton(Type itemType, string prefix)
    {
      string typeName = itemType.Name;

      if (itemType.IsDefined(typeof(DisplayNameAttribute), true))
      {
        typeName = (itemType.GetCustomAttributes(typeof(DisplayNameAttribute), true)[0]
          as DisplayNameAttribute).DisplayName;
      }

      ToolStripMenuItem ret = new ToolStripMenuItem(prefix + typeName);

      ret.Click += delegate
      {
        m_list.Add(Activator.CreateInstance(itemType));
      };

      return ret;
    }

    private void OnListChanged(object sender, ListChangedEventArgs e)
    {
      var oldMode = this.TreeView.DrawMode;
      this.TreeView.DrawMode = TreeViewDrawMode.Normal;

      switch (e.ListChangedType)
      {
        case ListChangedType.ItemAdded:
          this.AddItem(e.NewIndex);
          break;
        case ListChangedType.ItemChanged:
          this.ChangeItem(e.NewIndex, e.PropertyDescriptor);
          break;
        case ListChangedType.ItemDeleted:
          this.RemoveItem(e.NewIndex);
          break;
        case ListChangedType.Reset:
          this.Reset();
          break;
      }
      this.TreeView.DrawMode = oldMode;
    }

    private void ChangeItem(int index, PropertyDescriptor pd)
    {
      var changee = this.Nodes[index];

      if (!ReferenceEquals(changee.Tag, this.List[index]))
      {
        changee.ClearContent();
        changee.Remove();

        this.Nodes.DisplayEntry(m_list[index], index);
      }
      else
      {
        changee.Text = (changee.Tag ?? "").ToString();
      }
    }

    private void Reset()
    {
      object tag = this.Tag;
      IBindingList list = m_list;

      this.ClearContent();
      this.Nodes.Clear();

      this.List = list;
      this.ProcessChildren(tag, list);
    }

    private void AddItem(int index)
    {
      if (m_list == null || m_list.Count <= index || index < 0)
        return;

      object newItem = m_list[index];

      TreeNode newNode = this.Nodes.DisplayEntry(newItem, index);
      newNode.Expand();
      this.TreeView.SelectedNode = newNode;
    }

    private void RemoveItem(int index)
    {
      if (m_list == null || this.Nodes.Count <= index || index < 0)
        return;

      var removee = this.Nodes[index];

      removee.ClearContent();
      removee.Tag = null;
      removee.Remove();
    }
  }
}
