using Notung.Services;
using Notung;
using Schicksal.Helm.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Schicksal.Helm.IAnalyze
{
  public interface IAnalyze
  {
    string ToString();
    Dictionary<string, string[]> GetSettings();
    RunBase GetProcessor(DataTable table, AnovaDialogData data);
    LaunchParameters GetLaunchParameters();
    void BindTheResultForm(RunBase processor, object table_form, AnovaDialogData data);
  }
}
