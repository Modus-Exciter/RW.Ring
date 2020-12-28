using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Schicksal.Helm.Dialogs
{
  public partial class ComparisonFilterDialog : Form
  {
    public ComparisonFilterDialog()
    {
      InitializeComponent();
    }

    public bool OnlySignificat
    {
      get { return m_check_only_significat.Checked; }
      set { m_check_only_significat.Checked = value; }
    }

    public string Selection
    {
      get { return m_select_edit.Text; }
      set { m_select_edit.SelectedItem = value; }
    }

    public void SetSelectionList(IEnumerable<string> list)
    {
      m_select_edit.Items.Clear();
      m_select_edit.Items.AddRange(list.ToArray());
    }
  }
}
