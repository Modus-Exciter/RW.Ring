using System;
using System.Reflection;
using System.Windows.Forms;
using Notung;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm.Dialogs
{
  partial class AboutBox : Form
  {
    public AboutBox()
    {
      InitializeComponent();
      this.Text = String.Format("{0} {1}", Resources.ABOUT, ApplicationInfo.Instance.Product);
      this.labelProductName.Text = ApplicationInfo.Instance.Product;
      this.labelVersion.Text = String.Format("Version {0}", ApplicationInfo.Instance.Version);
      this.labelCopyright.Text = ApplicationInfo.Instance.Copyright;
      this.labelCompanyName.Text = ApplicationInfo.Instance.Company;
      this.textBoxDescription.Text = ApplicationInfo.Instance.Description;
    }
  }
}
