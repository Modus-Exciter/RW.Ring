using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Schicksal.Basic;

namespace Schicksal.Helm.Dialogs
{
  public partial class DiscriminantOptionsDialog : Form, IAnalysisOptions
  {
    public DiscriminantOptionsDialog()
    {
      this.InitializeComponent();
      this.StartPosition = FormStartPosition.Manual;
      this.Location = Cursor.Position;
    }

    string IAnalysisOptions.Save()
    {
      string criterion = "None";
      if (m_radio_entropy.Checked)
        criterion = "Entropy";
      else if (m_radio_gini.Checked)
        criterion = "Gini";

      return $"<DiscriminantParameters Criterion=\"{criterion}\" />";
    }

    void IAnalysisOptions.Load(StatisticsParameters context)
    {
      if (string.IsNullOrWhiteSpace(context.OptionsXML))
        return;

      var doc = new XmlDocument();
      doc.LoadXml(context.OptionsXML);
      var attr = doc.DocumentElement?.Attributes["Criterion"];

      if (attr != null)
      {
        switch (attr.Value)
        {
          case "Entropy":
            m_radio_entropy.Checked = true;
            break;
          case "Gini":
            m_radio_gini.Checked = true;
            break;
          default:
            m_radio_none.Checked = true;
            break;
        }
      }
    }

    bool IAnalysisOptions.ShowDialog()
    {
      return this.ShowDialog() == DialogResult.OK;
    }
  }
}
