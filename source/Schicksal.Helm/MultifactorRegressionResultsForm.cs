using Notung.Services;
using Schicksal.Regression;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Schicksal.Helm
{
  public partial class MultifactorRegressionResultsForm : Form
  {
    private readonly Color m_significat_color;
    public float Probability { get; set; }

    public MultifactorRegressionResultsForm()
    {
      this.InitializeComponent();
      m_significat_color = AppManager.Configurator.GetSection<Program.Preferences>().SignificatColor;
    }

    public void SetLinearRegressionResults(MultifactorRegressionResult dataSource, float probability)
    {
      this.Probability = probability;
      m_linearBindingSource.DataSource = this.ConvertToRegressionRowData(dataSource, probability);
    }

    public void SetParabolicRegressionResults(MultifactorRegressionResult dataSource, float probability)
    {
      this.Probability = probability;
      m_parabolicBindingSource.DataSource = this.ConvertToRegressionRowData(dataSource, probability);
    }

    private List<RegressionRowData> ConvertToRegressionRowData(MultifactorRegressionResult dataSource, float probability)
    {
      List<RegressionRowData> rows = new List<RegressionRowData>();

      if (dataSource == null) return rows;

      for (int i = 0; i < dataSource.FactorNames.Length; i++)
      {
        rows.Add(new RegressionRowData
        {
          Factor = dataSource.FactorNames[i],
          Coefficient = dataSource.Coefficients[i],
          StandardError = dataSource.StandardErrors[i],
          TStatistic = dataSource.TStatistics[i],
          PValue = dataSource.PValuesTStatistics[i],
          IsCoefficientSignificant = dataSource.PValuesTStatistics[i] <= probability,

          RSquared = null,
          AdjustedRSquared = null,
          FStatistic = null,
          PValueFStatistic = null,
          DegreesOfFreedomResidual = null,
          MsResidual = null
        });
      }

      double? msResidual = null;
      if (dataSource.DegreesOfFreedomResidual > 0)
      {
        msResidual = dataSource.ResidualSumOfSquares / dataSource.DegreesOfFreedomResidual;
      }

      rows.Add(new RegressionRowData
      {
        Factor = "Модель",
        Coefficient = null,
        StandardError = null,
        TStatistic = null,
        PValue = null,
        IsCoefficientSignificant = false,

        RSquared = dataSource.RSquared,
        AdjustedRSquared = dataSource.AdjustedRSquared,
        FStatistic = dataSource.FStatistic,
        PValueFStatistic = dataSource.PValueFStatistic,
        DegreesOfFreedomResidual = dataSource.DegreesOfFreedomResidual,
        MsResidual = msResidual
      });

      return rows;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      bool hasLinearResults = (m_linearBindingSource.DataSource as List<RegressionRowData>)?.Any() == true;
      bool hasParabolicResults = (m_parabolicBindingSource.DataSource as List<RegressionRowData>)?.Any() == true;

      if (!hasLinearResults)
      {
        m_tabControl.TabPages.Remove(m_tabPageLinear);
      }
      if (!hasParabolicResults)
      {
        m_tabControl.TabPages.Remove(m_tabPageParabolic);
      }

      if (m_tabControl.TabPages.Count == 0)
      {
        MessageBox.Show("Нет результатов для отображения.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
      }

      this.ApplyColumnFormatting(m_grid);
      this.ApplyColumnFormatting(m_gridParabolic);

      m_grid.Refresh();
      m_gridParabolic.Refresh();
    }

    private void ApplyColumnFormatting(DataGridView grid)
    {
      if (grid == null) return;
      foreach (DataGridViewColumn col in grid.Columns)
      {
        if (col.DataPropertyName == nameof(RegressionRowData.PValue) ||
            col.DataPropertyName == nameof(RegressionRowData.PValueFStatistic))
        {
          col.DefaultCellStyle.Format = "0.0000";
        }
        else if (col.ValueType == typeof(double) || col.ValueType == typeof(double?) ||
                 col.ValueType == typeof(float) || col.ValueType == typeof(float?) ||
                 col.ValueType == typeof(decimal) || col.ValueType == typeof(decimal?))
        {
          col.DefaultCellStyle.Format = "0.0000";
        }
        else if (col.DataPropertyName == nameof(RegressionRowData.DegreesOfFreedomResidual) ||
                 col.ValueType == typeof(int) || col.ValueType == typeof(int?))
        {
          col.DefaultCellStyle.Format = "0";
        }
      }
      grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
    }

    private void m_grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      this.HighlightSignificantPValues(e, m_grid);
    }

    private void m_gridParabolic_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      this.HighlightSignificantPValues(e, m_gridParabolic);
    }

    private void HighlightSignificantPValues(DataGridViewCellPaintingEventArgs e, DataGridView grid)
    {
      if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

      var rowData = grid.Rows[e.RowIndex].DataBoundItem as RegressionRowData;

      if (rowData == null) return;

      string dataPropertyName = grid.Columns[e.ColumnIndex].DataPropertyName;

      if (dataPropertyName == nameof(RegressionRowData.PValue))
      {
        if (rowData.IsCoefficientSignificant)
        {
          e.CellStyle.ForeColor = m_significat_color;
        }
      }
      // Подсветка PValueFStatistic для строки модели
      else if (dataPropertyName == nameof(RegressionRowData.PValueFStatistic))
      {
        if (rowData.PValueFStatistic.HasValue && rowData.PValueFStatistic.Value <= this.Probability)
        {
          e.CellStyle.ForeColor = m_significat_color;
        }
      }
    }

    private void m_tabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
  }


  public class RegressionRowData
  {
    public string Factor { get; set; }
    public double? Coefficient { get; set; }
    public double? StandardError { get; set; }
    public double? TStatistic { get; set; }
    public double? PValue { get; set; }
    public bool IsCoefficientSignificant { get; set; }

    public double? RSquared { get; set; }
    public double? AdjustedRSquared { get; set; }
    public double? FStatistic { get; set; }
    public double? PValueFStatistic { get; set; }
    public int? DegreesOfFreedomResidual { get; set; }
    public double? MsResidual { get; set; }
  }

}