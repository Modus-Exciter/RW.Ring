﻿using Notung.Services;
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
    IRunBase GetProcessor(DataTable table, StatisticsParameters data);
    LaunchParameters GetLaunchParameters();
    void BindTheResultForm(IRunBase processor, object table_form, StatisticsParameters data);
  }
}
