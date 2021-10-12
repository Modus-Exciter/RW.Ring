using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Reflection;

namespace Notung.Helm.Tree
{
  class NestedPropertyTreeNode : TreeNode
  {
    private readonly PropertyDescriptor m_descriptor;

    public NestedPropertyTreeNode(PropertyDescriptor descriptor)
    {
      if (descriptor == null)
        throw new ArgumentNullException("descriptor");

      m_descriptor = descriptor;
    }

    public override ContextMenuStrip ContextMenuStrip
    {
      get
      {
        if (base.ContextMenuStrip == null)
        {
          base.ContextMenuStrip = new ContextMenuStrip();

          if (this.TreeView != null && this.TreeView.Container != null)
            this.TreeView.Container.Add(this.ContextMenuStrip);

          if (!TreeBindingController.GetControllersForTree(this.TreeView).Any(c => c.ShowMenu))
            return base.ContextMenuStrip;

          var value = m_descriptor.GetValue(this.Parent.Tag);

          if (value == null)
          {
            var types = this.GetKnownTypes(m_descriptor.PropertyType);
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
          }
          else
          {
            ToolStripMenuItem item = new ToolStripMenuItem(CoreResources.DELETE);

            item.Click += delegate
            {
              m_descriptor.SetValue(this.Parent.Tag, null);
              this.Nodes[0].ClearContent();
              this.Nodes[0].Remove();
              this.ClearMenu();
            };
            base.ContextMenuStrip.Items.Add(item);
          }
        }

        return base.ContextMenuStrip;
      }
      set { }
    }

    private void ClearMenu()
    {
      base.ContextMenuStrip = null;
    }

    private ToolStripItem CreateAddButton(Type itemType, string prefix)
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
        var item = Activator.CreateInstance(itemType);
        m_descriptor.SetValue(this.Parent.Tag, item);

        this.TreeView.SelectedNode = this.Nodes.DisplayEntry(item, 0);
        this.ClearMenu();
      };

      return ret;
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
  }
}
