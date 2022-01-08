using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Notung.ComponentModel;

namespace Notung.Helm.Tree
{
  public class TreeBindingController : Component
  {
    #region Fields

    object m_data_source;

    static readonly Dictionary<TreeBindingController, TreeView> _trees
      = new Dictionary<TreeBindingController, TreeView>();

    #endregion

    #region Constructors

    /// <summary>
    /// Инициализирует новый экземпляр контроллера дерева
    /// </summary>
    public TreeBindingController()
    {
      lock (_trees)
        _trees.Add(this, null);

      this.ShowMenu = true;
    }

    /// <summary>
    /// Инициализирует новый экземпляр контроллера дерева
    /// </summary>
    /// <param name="container">Контейнер, в который добавляется компонент</param>
    public TreeBindingController(IContainer container)
      : this()
    {
      container.Add(this);
    }

    #endregion

    #region Events

    /// <summary>
    /// Происходит при выборе изображения для создаваемого узла
    /// </summary>
    public event EventHandler<ImageIdxEventArgs> ImageSelecting;

    /// <summary>
    /// Происходит при проверке допустимости перемещения узла
    /// </summary>
    public event EventHandler<TreeNodeMoveEventArgs> DragEnterChecking;

    /// <summary>
    /// Производит перемещение узла
    /// </summary>
    public event EventHandler<TreeNodeMoveEventArgs> DragDropPerforming;

    #endregion

    #region Properties

    /// <summary>
    /// Дерево, которое требуется отобразить
    /// </summary>
    public TreeView Tree
    {
      get
      {
        lock (_trees)
          return _trees[this];
      }
      set
      {
        lock (_trees)
        {
          TreeView old = _trees[this];

          if (old != null)
          {
            old.NodeMouseClick -= this.Tree_NodeMouseClick;
            old.KeyDown -= this.Tree_KeyDown;
            old.ItemDrag -= this.Tree_ItemDrag;
            old.DragOver -= this.Tree_DragOver;
            old.DragDrop -= this.Tree_DragDrop;
          }
          if (value != null)
          {
            value.NodeMouseClick += this.Tree_NodeMouseClick;
            value.KeyDown += this.Tree_KeyDown;
            value.ItemDrag += this.Tree_ItemDrag;
            value.DragOver += this.Tree_DragOver;
            value.DragDrop += this.Tree_DragDrop;
          }
          _trees[this] = value;
        }
      }
    }

    [DefaultValue(true)]
    public bool ShowMenu { get; set; }

    /// <summary>
    /// Объект, отображаемый в дереве
    /// </summary>
    public object DataSource
    {
      get { return m_data_source; }
      set
      {
        if (this.Tree == null)
          return;

        foreach (TreeNode node in this.Tree.Nodes)
          node.ClearContent();

        this.Tree.Nodes.Clear();
        m_data_source = value;

        if (value == null)
          return;

        var retNode = this.Tree.Nodes.DisplayEntry(value, 0);

        if (retNode != null)
        {
          retNode.Expand();
        }
      }
    }

    #endregion

    #region Tree handlers

    private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      TreeView tree = sender as TreeView;

      if (tree != null)
        tree.SelectedNode = e.Node;
    }

    private void Tree_KeyDown(object sender, KeyEventArgs e)
    {
      TreeView tree = sender as TreeView;

      if (tree == null)
        return;

      if (e.KeyCode == Keys.Delete)
      {
        ListTreeNode node = tree.SelectedNode.Parent as ListTreeNode;

        if (node != null && node.List != null && tree.SelectedNode.Tag != null)
        {
          node.List.Remove(tree.SelectedNode.Tag);
        }
      }
      else if (e.KeyCode == Keys.Insert)
      {
        ListTreeNode node = tree.SelectedNode as ListTreeNode;

        if (node != null && node.List != null)
        {
          try
          {
            node.List.AddNew();
          }
          catch { }
        }
      }
      else if (e.KeyCode == Keys.F2)
      {
        if (tree.SelectedNode != null && tree.LabelEdit)
          tree.SelectedNode.BeginEdit();
      }
    }

    private void Tree_ItemDrag(object sender, ItemDragEventArgs e)
    {
      TreeView tree = sender as TreeView;

      if (tree != null)
        tree.DoDragDrop(e.Item, DragDropEffects.Move);
    }

    private void Tree_DragOver(object sender, DragEventArgs e)
    {
      TreeView tree = sender as TreeView;

      if (tree != null)
        this.PrepareDrag(tree, e);
    }

    private void Tree_DragDrop(object sender, DragEventArgs drgevent)
    {
      TreeView tree = sender as TreeView;

      if (tree != null && drgevent.Effect == DragDropEffects.Move)
      {
        TreeNode source = drgevent.Data.GetData(typeof(TreeNode)) as TreeNode;
        if (source == null)
        {
          source = drgevent.Data.GetData(typeof(ListTreeNode)) as TreeNode;
        }
        TreeNode destination = tree.GetNodeAt(tree.PointToClient(new Point(drgevent.X, drgevent.Y)));

        TreeNodeMoveEventArgs moveArgs = new TreeNodeMoveEventArgs(source, destination);

        INodeMoveProvider mover = m_data_source as INodeMoveProvider;
        if (mover != null)
        {
          IList list = null;
          if (destination is ListTreeNode)
          {
            list = (destination as ListTreeNode).List;
          }
          mover.Perform(source.Tag, destination.Tag, list);
        }

        if (this.DragDropPerforming != null)
        {
          this.DragDropPerforming(this, moveArgs);
        }
      }
    }

    private void PrepareDrag(TreeView tree, DragEventArgs drgevent)
    {
      TreeNode source = drgevent.Data.GetData(typeof(TreeNode)) as TreeNode;
      if (source == null)
      {
        source = drgevent.Data.GetData(typeof(ListTreeNode)) as TreeNode;
      }
      TreeNode destination = tree.GetNodeAt(tree.PointToClient(new Point(drgevent.X, drgevent.Y)));

      TreeNodeMoveEventArgs moveArgs = new TreeNodeMoveEventArgs(source, destination);

      INodeMoveProvider mover = m_data_source as INodeMoveProvider;
      if (mover != null)
      {
        IList list = null;
        if (destination is ListTreeNode)
        {
          list = (destination as ListTreeNode).List;
        }
        moveArgs.Cancel = !mover.Check(source.Tag, destination.Tag, list);
      }

      if (this.DragEnterChecking != null)
      {
        this.DragEnterChecking(this, moveArgs);
      }

      drgevent.Effect = moveArgs.Cancel ? DragDropEffects.None : DragDropEffects.Move;

      tree.SelectedNode = destination ?? tree.SelectedNode;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Очищает ресурсы и отвязывает дерево от контроллера
    /// </summary>
    /// <param name="disposing">Нужно ли освобождать управляемые ресурсы</param>
    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      lock (_trees)
        _trees.Remove(this);
    }

    internal void SelectImage(ImageIdxEventArgs e)
    {
      if (this.ImageSelecting != null)
        this.ImageSelecting(this, e);
    }

    internal static IEnumerable<TreeBindingController> GetControllersForTree(TreeView tree)
    {
      lock (_trees)
        return _trees.Where(kv => ReferenceEquals(kv.Value, tree)).Select(kv => kv.Key);
    }

    #endregion
  }
}
