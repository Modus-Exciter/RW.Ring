using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Notung.Helm.Configuration;

namespace Notung.Helm.Dialogs
{
  public partial class SettingsDialog : Form
  {
    private readonly Dictionary<Type, TreeNode> m_nodes = new Dictionary<Type, TreeNode>();
    
    public SettingsDialog()
    {
      InitializeComponent();
    }

    private void SettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
        e.Cancel = !m_settings_controller.SaveAllSections();
    }

    private void m_errors_view_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0 || e.RowIndex >= m_settings_controller.Errors.Count)
        return;

      m_sections_tree.SelectedNode = m_nodes[m_settings_controller.Errors[e.RowIndex].SectionType];
    }

    private void m_sections_list_AfterSelect(object sender, TreeViewEventArgs e)
    {
      m_settings_controller.SelectPage(e.Node.Tag.GetType());
    }

    private void SettingsDialog_Load(object sender, EventArgs e)
    {
      m_settings_controller.LoadAllPages();

      foreach (var section in m_settings_controller.Pages)
      {
        var node = m_sections_tree.Nodes.Add(section.ToString());
        node.Tag = section;

        if (section.Image != null)
        {
          m_image_list.Images.Add(section.Image);
          node.ImageIndex = m_image_list.Images.Count - 1;
          node.SelectedImageIndex = node.ImageIndex;
        }

        m_nodes[section.GetType()] = node;
      }
    }

    private void m_button_apply_Click(object sender, EventArgs e)
    {
      m_settings_controller.SaveAllSections(true);
    }
  }
}
