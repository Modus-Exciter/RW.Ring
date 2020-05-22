using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConfiguratorGraphicalTest
{
  public partial class SelectFunctionDialog : Form
  {
    public SelectFunctionDialog(string[] items)
    {
      InitializeComponent();

      m_list_box.Items.AddRange(items);
    }

    public string SelectedItem
    {
      get { return m_list_box.SelectedItem as string; }
    }
  }
}
