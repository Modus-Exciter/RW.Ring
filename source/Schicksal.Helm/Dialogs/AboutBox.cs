using System;
using System.Windows.Forms;
using Notung;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm.Dialogs
{
  partial class AboutBox : Form
  {
    public AboutBox()
    {
      this.InitializeComponent(); 

      this.Text = string.Format("{0} {1}", Resources.ABOUT, ApplicationInfo.Instance.Product);
      labelProductName.Text = ApplicationInfo.Instance.Product;
      labelVersion.Text = string.Format("Version {0}", ApplicationInfo.Instance.Version);
      labelCopyright.Text = ApplicationInfo.Instance.Copyright;
      labelCompanyName.Text = ApplicationInfo.Instance.Company;
      textBoxDescription.Text = ApplicationInfo.Instance.Description;
    }
  }
}
