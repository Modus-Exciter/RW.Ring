using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Notung;

namespace Schicksal.Helm.Dialogs
{
  public partial class AnovaDialog : Form
  {
    public AnovaDialog()
    {
      InitializeComponent();
      m_probability_edit.Items.AddRange(new object[] { 0.05, 0.01, 0.001 });
    }

    public AnovaDialogData DataSource
    {
      get { return m_binding_source.DataSource as AnovaDialogData; }
      set { m_binding_source.DataSource = (object)value ?? typeof(AnovaDialogData); }
    }

    private void m_button_left_Click(object sender, EventArgs e)
    {
      this.DataSource.AddPredictor(m_list_total.Text);
    }

    private void m_button_right_Click(object sender, EventArgs e)
    {
      this.DataSource.RemovePredictor(m_list_selected.Text);
    }
  }

  public class AnovaDialogData : IValidator
  {
    private readonly BindingList<string> m_total_columns = new BindingList<string>();
    private readonly BindingList<string> m_predictors = new BindingList<string>();
    private readonly BindingList<string> m_calculatable = new BindingList<string>();
    private readonly HashSet<string> m_total_calculatable = new HashSet<string>();
    private readonly string m_hash;

    public AnovaDialogData(DataTable table)
    {
      if (table == null)
        throw new ArgumentNullException("table");
      
      foreach (DataColumn column in table.Columns)
      {
        m_total_columns.Add(column.ColumnName);

        if (column.DataType.IsPrimitive && column.DataType != typeof(bool)
          && column.DataType != typeof(char))
        {
          m_calculatable.Add(column.ColumnName);
          m_total_calculatable.Add(column.ColumnName);
        }
      }

      this.Probability = 0.05f;

      m_hash = string.Join(".", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).OrderBy(c => c));

      var settings = AppManager.Configurator.GetSection<Program.Preferences>().AnovaSettings;
      string[] array;

      if (settings.TryGetValue(m_hash, out array))
      {
        this.Result = array[0];
        this.Filter = array[1];
        this.Probability = float.Parse(array[2], CultureInfo.InvariantCulture);

        for (int i = 3; i < array.Length; i++)
          this.AddPredictor(array[i]);
      }
    }

    public BindingList<string> Total
    {
      get { return m_total_columns; }
    }

    public BindingList<string> Predictors
    {
      get { return m_predictors; }
    }

    public BindingList<string> Calculatable
    {
      get { return m_calculatable; }
    }

    public void AddPredictor(string value)
    {
      m_predictors.Add(value);
      m_total_columns.Remove(value);
      m_calculatable.Remove(value);
    }

    public void RemovePredictor(string value)
    {
      m_predictors.Remove(value);
      m_total_columns.Add(value);

      if (m_total_calculatable.Contains(value))
        m_calculatable.Add(value);
    }

    public void Save()
    {
      var settings = AppManager.Configurator.GetSection<Program.Preferences>().AnovaSettings;

      var array = new string[m_predictors.Count + 3];
      array[0] = this.Result;
      array[1] = this.Filter;
      array[2] = this.Probability.ToString(CultureInfo.InvariantCulture);

      int i = 3;

      foreach (var item in m_predictors)
        array[i++] = item;

      settings[m_hash] = array;
    }

    public string Result { get; set; }

    public string Filter { get; set; }

    public float Probability { get; set; }

    public bool Validate(InfoBuffer buffer)
    {
      return true;
    }
  }
}
