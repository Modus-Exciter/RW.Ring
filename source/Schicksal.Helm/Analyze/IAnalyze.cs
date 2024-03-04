using Notung.Services;
using Notung;
using Schicksal.Helm.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Schicksal.Helm.Analyze
{
  public interface IAnalyze
  {
    Dictionary<string, string[]> GetSettings();
    Type OptionsType { get; }
    RunBase GetProcessor(DataTable table, StatisticsParameters data);
    LaunchParameters GetLaunchParameters();
    void BindTheResultForm(RunBase processor, object table_form, StatisticsParameters data);
  }
}
