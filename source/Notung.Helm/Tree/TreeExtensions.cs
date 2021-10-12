using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using Notung.ComponentModel;

namespace Notung.Helm.Tree
{
  static class TreeExtensions
{
  static readonly Dictionary<INotifyPropertyChanged, TreeNode> _change_handlers
    = new Dictionary<INotifyPropertyChanged, TreeNode>();

  public static TreeNode DisplayEntry(this TreeNodeCollection nodes, object component, int index)
  {
    if (component == null)
      return null;

    IBindingList list = component as IBindingList;

    if (list == null)
    {
      IListSource source = component as IListSource;

      if (source != null)
        list = source.GetList() as IBindingList;
    }

    var ret = list != null ? new ListTreeNode() : new TreeNode();
    nodes.Insert(index, ret);

    ret.Text = component.ToString();
    ret.Tag = component;

    ret.SetImage(component);

    if (list != null)
    {
      ((ListTreeNode)ret).List = list;
    }

    ret.ProcessChildren(component, list);

    ListTreeNode parentNode = ret.Parent as ListTreeNode;

    if (TreeBindingController.GetControllersForTree(ret.TreeView).Any(c => c.ShowMenu))
    {
      if (parentNode != null && parentNode.List != null && ret.ContextMenuStrip == null)
      {
        ret.ContextMenuStrip = new ContextMenuStrip();
        ret.ContextMenuStrip.Items.Add(CoreResources.DELETE, null, delegate
        {
          parentNode.List.Remove(component);
        });
      }
    }

    ret.AddPropertyHandler(component);
    return ret;
  }

  internal static void ProcessChildren(this TreeNode node, object component, IBindingList list)
  {
    var pds = (from PropertyDescriptor pd in TypeDescriptor.GetProperties(component)
               where typeof(IBindingList).IsAssignableFrom(pd.PropertyType)
               && pd.Attributes[typeof(NoBindAttribute)] == null
               select new { Name = pd.DisplayName, Value = (IBindingList)pd.GetValue(component) }).ToList();

    foreach (var current in pds)
    {
      ListTreeNode listNode = ReferenceEquals(current.Value, list) ?
      (ListTreeNode)node : new ListTreeNode();

      if (!ReferenceEquals(listNode, node))
      {
        node.Nodes.Add(listNode);
        listNode.Text = current.Name;
        listNode.Tag = current.Value;
        listNode.List = current.Value;
        listNode.SetImage(current.Value);
      }

      for (int i = 0; i < current.Value.Count; i++)
      {
        listNode.Nodes.DisplayEntry(current.Value[i], i);
      }
    }

    if (list == null)
    {
      foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(component))
      {
        if (pd.IsReadOnly || pd.PropertyType == typeof(object)
          || !pd.PropertyType.IsClass
          || typeof(IEnumerable).IsAssignableFrom(pd.PropertyType)
          || pd.Attributes[typeof(NoBindAttribute)] != null
          || pd.Converter.CanConvertFrom(typeof(string)))
          continue;

        NestedPropertyTreeNode nested = new NestedPropertyTreeNode(pd);
        nested.Text = pd.DisplayName;

        node.Nodes.Add(nested);
        nested.SetImage(null);

        var value = pd.GetValue(component);

        if (value != null)
          nested.Nodes.DisplayEntry(value, 0);
      }
    }
  }

  internal static void SetImage(this TreeNode ret, object component)
  {
    ImageIdxEventArgs e = new ImageIdxEventArgs(component);
    foreach (var binder in TreeBindingController.GetControllersForTree(ret.TreeView))
    {
      binder.SelectImage(e);

      if (e.ImageIdx >= 0)
        break;
    }

    ret.SelectedImageIndex = e.ImageIdx;
    ret.ImageIndex = e.ImageIdx;
  }

  internal static void AddPropertyHandler(this TreeNode ret, object component)
  {
    INotifyPropertyChanged pc = component as INotifyPropertyChanged;
    if (pc != null)
      lock (_change_handlers)
      {
        if (!_change_handlers.ContainsKey(pc))
        {
          _change_handlers.Add(pc, ret);
          pc.PropertyChanged += PropertyChangedHandler;
        }
      }
  }

  private static void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
  {
    INotifyPropertyChanged pc = sender as INotifyPropertyChanged;

    if (pc == null)
      return;

    lock (_change_handlers)
    {
      TreeNode node;

      if (_change_handlers.TryGetValue(pc, out node))
      {
        node.Text = pc.ToString();
      }
    }
  }

  public static void ClearContent(this TreeNode node)
  {
    foreach (TreeNode child in node.Nodes)
    {
      child.ClearContent();
    }

    ListTreeNode ltn = node as ListTreeNode;

    if (ltn != null)
    {
      ltn.List = null;
    }

    INotifyPropertyChanged pc = node.Tag as INotifyPropertyChanged;
    if (pc != null)
    {
      lock (_change_handlers)
      {
        _change_handlers.Remove(pc);
        pc.PropertyChanged -= PropertyChangedHandler;
      }
    }
  }
}

}
