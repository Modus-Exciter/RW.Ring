using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Schicksal.Helm.Dialogs
{
  public partial class AnovaOptionsDialog : Form, IAnalysisOptions
  {
    public AnovaOptionsDialog()
    {
      InitializeComponent();
    }

    public string Save()
    {
       return null;//throw new NotImplementedException();
    }

    void IAnalysisOptions.Load(string xml)
    {
      //throw new NotImplementedException();
    }

    bool IAnalysisOptions.ShowDialog()
    {
      return this.ShowDialog() == DialogResult.OK;
    }
  }
}
