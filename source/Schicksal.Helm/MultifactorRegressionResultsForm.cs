using Notung.Services;
using Schicksal.Regression;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Schicksal.Helm
{
  public partial class MultifactorRegressionResultsForm : Form
  {
    private readonly Color m_significat_color;
    public float Probability { get; set; }
    public string ResponseVariableName { get; set; }

    public MultifactorRegressionResultsForm()
    {
      InitializeComponent();
      m_significat_color = AppManager.Configurator.GetSection<Program.Preferences>().SignificatColor;
    }

    public void SetLinearRegressionResults(MultifactorRegressionResult dataSource, float probability)
    {
      if (dataSource == null) return;

      this.Probability = probability;
      m_linearEquationLabel.Text = this.GenerateEquationString(dataSource);
      m_linearCoefficientsBindingSource.DataSource = this.ConvertToCoefficientRowData(dataSource);
      m_linearModelBindingSource.DataSource = this.ConvertToModelRowData(dataSource);
    }

    public void SetParabolicRegressionResults(MultifactorRegressionResult dataSource, float probability)
    {
      if (dataSource == null) return;

      this.Probability = probability;
      m_parabolicEquationLabel.Text = this.GenerateEquationString(dataSource);
      m_parabolicCoefficientsBindingSource.DataSource = this.ConvertToCoefficientRowData(dataSource);
      m_parabolicModelBindingSource.DataSource = this.ConvertToModelRowData(dataSource);
    }

    private string GenerateEquationString(MultifactorRegressionResult result)
    {
      string responseName = string.IsNullOrEmpty(this.ResponseVariableName) ? "Отклик" : this.ResponseVariableName;

      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0} = {1:F4}", responseName, result.Coefficients[0]);

      for (int i = 1; i < result.Coefficients.Length; i++)
      {
        double coeff = result.Coefficients[i];
        sb.Append(coeff >= 0 ? " + " : " - ");
        sb.AppendFormat("{0:F4} * {1}", Math.Abs(coeff), result.FactorNames[i]);
      }
      return sb.ToString();
    }

    private List<CoefficientRowData> ConvertToCoefficientRowData(MultifactorRegressionResult dataSource)
    {
      var rows = new List<CoefficientRowData>();
      for (int i = 0; i < dataSource.FactorNames.Length; i++)
      {
        rows.Add(new CoefficientRowData
        {
          Factor = dataSource.FactorNames[i],
          Coefficient = dataSource.Coefficients[i],
          StandardError = dataSource.StandardErrors[i],
          TStatistic = dataSource.TStatistics[i],
          PValue = dataSource.PValuesTStatistics[i],
        });
      }
      return rows;
    }

    private List<ModelRowData> ConvertToModelRowData(MultifactorRegressionResult dataSource)
    {
      var rows = new List<ModelRowData>();
      rows.Add(new ModelRowData
      {
        Model = "Модель",
        RSquared = dataSource.RSquared,
        AdjustedRSquared = dataSource.AdjustedRSquared,
        FStatistic = dataSource.FStatistic,
        PValueFStatistic = dataSource.PValueFStatistic
      });
      return rows;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      bool hasLinearResults = m_linearCoefficientsBindingSource.Count > 0;
      bool hasParabolicResults = m_parabolicCoefficientsBindingSource.Count > 0;

      if (!hasLinearResults)
      {
        if (m_tabControl.TabPages.Contains(m_tabPageLinear))
          m_tabControl.TabPages.Remove(m_tabPageLinear);
      }
      if (!hasParabolicResults)
      {
        if (m_tabControl.TabPages.Contains(m_tabPageParabolic))
          m_tabControl.TabPages.Remove(m_tabPageParabolic);
      }
    }

    private void m_Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
      var grid = sender as DataGridView;
      if (grid == null) return;

      if (grid.Columns[e.ColumnIndex].DataPropertyName == nameof(CoefficientRowData.PValue) ||
          grid.Columns[e.ColumnIndex].DataPropertyName == nameof(ModelRowData.PValueFStatistic))
      {
        // Проверяем тип данных строки
        if (grid.Rows[e.RowIndex].DataBoundItem is CoefficientRowData coeffRowData &&
            grid.Columns[e.ColumnIndex].DataPropertyName == nameof(CoefficientRowData.PValue))
        {
          if (coeffRowData.PValue != null && coeffRowData.PValue.Value <= this.Probability)
          {
            e.CellStyle.ForeColor = m_significat_color;
          }
        }
        else if (grid.Rows[e.RowIndex].DataBoundItem is ModelRowData modelRowData &&
                 grid.Columns[e.ColumnIndex].DataPropertyName == nameof(ModelRowData.PValueFStatistic))
        {
          if (modelRowData.PValueFStatistic != null && modelRowData.PValueFStatistic.Value <= this.Probability)
          {
            e.CellStyle.ForeColor = m_significat_color;
          }
        }
      }
    }

    public class CoefficientRowData
    {
      public string Factor { get; set; }
      public double? Coefficient { get; set; }
      public double? StandardError { get; set; }
      public double? TStatistic { get; set; }
      public double? PValue { get; set; }
    }

    public class ModelRowData
    {
      public string Model { get; set; }
      public double? RSquared { get; set; }
      public double? AdjustedRSquared { get; set; }
      public double? FStatistic { get; set; }
      public double? PValueFStatistic { get; set; }
    }
  }
}