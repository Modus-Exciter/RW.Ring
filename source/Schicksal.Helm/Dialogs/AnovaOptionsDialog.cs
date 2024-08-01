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

    string IAnalysisOptions.Save()
    {
      var sb = new StringBuilder();
      sb.Append("<AnovaParameters Normalization=\"");
      if (m_btn_no_norm.Checked)
        sb.Append("None");
      else if (m_btn_kruskal_wallis.Checked)
        sb.Append("NonParametric");
      else if (m_btn_box_cox.Checked)
        sb.Append("BoxCox");

      if (!string.IsNullOrEmpty(m_cb_conjugate.SelectedItem as string))
        sb.AppendFormat("\"\nConjugate=\"{0}", m_cb_conjugate.SelectedItem);

      sb.AppendFormat("\" Individual=\"{0}\"/>", m_check_individual_error.Checked);

      return sb.ToString();
    }

    void IAnalysisOptions.Load(StatisticsParameters context)
    {
      m_cb_conjugate.DataSource = new string[] { string.Empty }.Union((context.Total).Except(
        new string[] { context.Result })).ToList();

      if (string.IsNullOrWhiteSpace(context.OptionsXML))
        return;

      var doc = new XmlDocument();
      doc.LoadXml(context.OptionsXML);
      switch (doc.DocumentElement.Attributes["Normalization"].Value)
      {
        case "NonParametric":
          m_btn_kruskal_wallis.Checked = true;
          break;

        case "BoxCox":
          m_btn_box_cox.Checked = true;
          break;
      }

      m_cb_conjugate.SelectedItem = doc.DocumentElement.HasAttribute("Conjugate") ?
        doc.DocumentElement.Attributes["Conjugate"].Value : string.Empty;

      m_check_individual_error.Checked = doc.DocumentElement.HasAttribute("Individual") 
        && bool.Parse(doc.DocumentElement.Attributes["Individual"].Value);
    }

    bool IAnalysisOptions.ShowDialog()
    {
      return this.ShowDialog() == DialogResult.OK;
    }
  }
}
