﻿using System;
using System.Windows.Controls;

namespace LogAnalyzer
{
  /// <summary>
  /// Interaction logic for LogDisplay.xaml
  /// </summary>
  public partial class LogDisplay : UserControl
  {
    private static readonly DataGridLength _star_length = new DataGridLength(1, DataGridLengthUnitType.Star);

    public LogDisplay()
    {
      this.InitializeComponent();
    }

    private void DataGrid_AutoGeneratedColumns(object sender, EventArgs e)
    {
      var grid = sender as DataGrid;

      if (grid == null)
        return;

      if (grid.Columns.Count > 0)
        grid.Columns[grid.Columns.Count - 1].Width = _star_length;
      if (grid.Columns.Count > 1)
        grid.Columns[grid.Columns.Count - 2].Width = _star_length;
    }
  }
}
