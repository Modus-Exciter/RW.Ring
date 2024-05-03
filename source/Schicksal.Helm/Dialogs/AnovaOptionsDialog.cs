using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace Schicksal.Helm.Dialogs
{
  public partial class AnovaOptionsDialog : Form, IAnalysisOptions
  {
    public AnovaOptionsDialog()
    {
      this.InitializeComponent();
    }

    public string Save()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("<AnovaParameters Normalization=\"");
      if (m_btn_no_norm.Checked)
        sb.Append("None");
      else if (m_btn_non_parametric.Checked)
        sb.Append("NonParametric");
      else if (m_btn_box_cox.Checked)
        sb.Append("BoxCox");

      sb.Append("\"/>");

      return sb.ToString();
    }

    void IAnalysisOptions.Load(string xml)
    {
      if (string.IsNullOrWhiteSpace(xml))
        return;

      XmlDocument doc = new XmlDocument();
      doc.LoadXml(xml);
      switch (doc.DocumentElement.Attributes["Normalization"].Value)
      {
        case "NonParametric":
          m_btn_non_parametric.Checked = true;
          break;

        case "BoxCox":
          m_btn_box_cox.Checked = true;
          break;
      }
    }

    bool IAnalysisOptions.ShowDialog()
    {
      return this.ShowDialog() == DialogResult.OK;
    }
  }
}
