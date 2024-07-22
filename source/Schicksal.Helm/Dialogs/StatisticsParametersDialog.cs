using Notung;
using Notung.Services;
using Schicksal.Helm.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Schicksal.Helm.Dialogs
{
  public partial class StatisticsParametersDialog : Form
  {
    private Type m_options_type;

    public StatisticsParametersDialog()
    {
      this.InitializeComponent();
      m_probability_edit.Items.AddRange(new object[] { 0.05, 0.01, 0.001 });
    }

    public StatisticsParameters DataSource
    {
      get { return m_binding_source.DataSource as StatisticsParameters; }
      set { m_binding_source.DataSource = (object)value ?? typeof(StatisticsParameters); }
    }

    public Type OptionsType
    {
      get { return m_options_type; }
      set 
      {
        m_options_type = value;
        m_options_button.Visible = value != null && typeof(IAnalysisOptions).IsAssignableFrom(value)
          && value.GetConstructor(Type.EmptyTypes) != null;
      }
    }

    private void Button_left_Click(object sender, EventArgs e)
    {
      this.DataSource.AddPredictor(m_list_total.Text);
    }

    private void Button_right_Click(object sender, EventArgs e)
    {
      this.DataSource.RemovePredictor(m_list_selected.Text);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);

      if (this.DialogResult == DialogResult.OK)
      {
        var buffer = new InfoBuffer();

        if (!this.DataSource.Validate(buffer))
        {
          AppManager.Notificator.Show(buffer);
          e.Cancel = true;
        }
        else if (buffer.Count > 0)
          e.Cancel = !AppManager.Notificator.Confirm(buffer);
      }
    }

    private void Options_button_Click(object sender, EventArgs e)
    {
      var options = (IAnalysisOptions)Activator.CreateInstance(m_options_type);

      options.Load(this.DataSource);

      if (options.ShowDialog())
        this.DataSource.OptionsXML = options.Save();
    }
  }

  public interface IAnalysisOptions
  {
    void Load(StatisticsParameters context);

    string Save();

    bool ShowDialog();
  }

  public class StatisticsParameters : IValidator, INotifyPropertyChanged
  {
    private readonly BindingList<string> m_total_columns = new BindingList<string>();
    private readonly BindingList<string> m_predictors = new BindingList<string>();
    private readonly BindingList<string> m_calculatable = new BindingList<string>();
    private readonly HashSet<string> m_total_calculatable = new HashSet<string>();
    private readonly string m_hash;
    private string m_result;
    private string m_filter;
    private float m_probability;

    public event PropertyChangedEventHandler PropertyChanged;

    public StatisticsParameters(DataTable table, Dictionary<string, string[]> settings)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      m_calculatable.Add(string.Empty);

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

      string[] array;

      if (settings.TryGetValue(m_hash, out array) && array.Length > 2)
      {
        this.Result = array[0];
        this.Filter = array[1];

        float p;

        if (float.TryParse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture, out p))
          this.Probability = p;

        for (int i = 3; i < array.Length - 1; i++)
          this.AddPredictor(array[i]);

        if (m_total_columns.Contains(array[array.Length - 1]))
          this.AddPredictor(array[array.Length - 1]);
        else if (array[array.Length - 1].StartsWith("<"))
          this.OptionsXML = array[array.Length - 1];
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
      if (value == this.Result)
        this.Result = string.Empty;

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

    public void Save(Dictionary<string, string[]> settings)
    {
      var array = new string[m_predictors.Count + (string.IsNullOrEmpty(this.OptionsXML) ? 3 : 4)];
      array[0] = this.Result;
      array[1] = this.Filter;
      array[2] = this.Probability.ToString(CultureInfo.InvariantCulture);

      int i = 3;

      foreach (var item in m_predictors)
        array[i++] = item;

      if (!string.IsNullOrEmpty(this.OptionsXML))
        array[array.Length - 1] = this.OptionsXML;

      settings[m_hash] = array;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public string Result
    {
      get { return m_result; }
      set
      {
        m_result = value;
        this.OnPropertyChanged("Result");
      }
    }

    public string OptionsXML { get; set; }

    public string Filter
    {
      get { return m_filter; }
      set
      {
        m_filter = value;
        this.OnPropertyChanged("Filter");
      }
    }

    public float Probability
    {
      get { return m_probability; }
      set
      {
        m_probability = value;
        this.OnPropertyChanged("Probability");
      }
    }

    public bool Validate(InfoBuffer buffer)
    {
      if (string.IsNullOrEmpty(this.Result))
        buffer.Add(Resources.NO_RESULT_COUMN, InfoLevel.Warning);

      if (m_predictors.Count == 0)
        buffer.Add(Resources.NO_FACTOR_COLUMNS, InfoLevel.Warning);

      return buffer.Count == 0;
    }
  }
}
