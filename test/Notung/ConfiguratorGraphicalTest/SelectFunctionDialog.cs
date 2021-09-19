using System.Windows.Forms;

namespace ConfiguratorGraphicalTest
{
  public partial class SelectFunctionDialog : Form
  {
    public SelectFunctionDialog(string[] items)
    {
      this.InitializeComponent();

      m_list_box.Items.AddRange(items);
    }

    public string SelectedItem
    {
      get { return m_list_box.SelectedItem as string; }
    }
  }
}
