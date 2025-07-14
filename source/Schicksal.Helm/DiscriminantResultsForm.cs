using Schicksal.Discriminant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Schicksal.Helm
{
  public partial class DiscriminantResultsForm : Form
  {
    /// <summary>
    /// Результаты анализа
    /// </summary>
    public DiscriminantResult DataSource { get; set; }

      public DiscriminantResultsForm()
      {
      this.InitializeComponent();
      }
    /// <summary>
    /// Обработчик загрузки формы
    /// </summary>
    private void DiscriminantResultsForm_Load(object sender, EventArgs e)
      {
        if (this.DataSource == null) return;

        txtSummary.Text = this.DataSource.Summary();

        treeViewDecision.Nodes.Clear();
        TreeNode root = this.BuildTreeNode(this.DataSource.DecisionTree);
        if (root != null)
          treeViewDecision.Nodes.Add(root);

        treeViewDecision.ExpandAll();
      }
    /// <summary>
    /// Рекурсивно строит дерево для TreeView
    /// </summary>
    private TreeNode BuildTreeNode(DiscriminantTreeNode node)
      {
        if (node == null) return null;

        TreeNode treeNode = new TreeNode(node.ToString());

        if (!node.End)
        {
          treeNode.Nodes.Add(new TreeNode("Лево:") { Tag = "header" });
          var leftNode = this.BuildTreeNode(node.Left);
          if (leftNode != null)
            treeNode.Nodes.Add(leftNode);

          treeNode.Nodes.Add(new TreeNode("Право:") { Tag = "header" });
          var rightNode = this.BuildTreeNode(node.Right);
          if (rightNode != null)
            treeNode.Nodes.Add(rightNode);
        }

        return treeNode;
      }
  }
}
