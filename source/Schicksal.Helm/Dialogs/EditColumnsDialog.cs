using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Notung;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm.Dialogs
{
  public partial class EditColumnsDialog : Form
  {
    public EditColumnsDialog()
    {
      InitializeComponent();

      var dic = new Dictionary<Type, string>();

      dic[typeof(byte)] = string.Format(SchicksalResources.UINT, byte.MaxValue);
      dic[typeof(sbyte)] = string.Format(SchicksalResources.INT, sbyte.MinValue, sbyte.MaxValue);
      dic[typeof(ushort)] = string.Format(SchicksalResources.UINT, ushort.MaxValue);
      dic[typeof(short)] = string.Format(SchicksalResources.INT, short.MinValue, short.MaxValue);
      dic[typeof(uint)] = string.Format(SchicksalResources.UINT, uint.MaxValue);
      dic[typeof(int)] = string.Format(SchicksalResources.INT, int.MinValue, int.MaxValue);
      dic[typeof(ulong)] = string.Format(SchicksalResources.UINT, "1*10^13");
      dic[typeof(long)] = string.Format(SchicksalResources.INT, "1*10^-13", "1*10^13");
      dic[typeof(float)] = SchicksalResources.FLOAT;
      dic[typeof(double)] = SchicksalResources.DOUBLE;
      dic[typeof(decimal)] = SchicksalResources.DECIMAL;
      dic[typeof(char)] = SchicksalResources.SYMBOL;
      dic[typeof(string)] = SchicksalResources.TEXT;
      dic[typeof(bool)] = SchicksalResources.BOOL;
      dic[typeof(DateTime)] = SchicksalResources.DATE;
      dic[typeof(TimeSpan)] = SchicksalResources.TIME;

      columnTypeDataGridViewTextBoxColumn.DataSource = dic.ToArray();
      columnTypeDataGridViewTextBoxColumn.ValueMember = "Key";
      columnTypeDataGridViewTextBoxColumn.DisplayMember = "Value";

      m_binding_source.DataSource = new BindingList<TableColumnInfo>();
    }

    public BindingList<TableColumnInfo> Columns
    {
      get { return m_binding_source.DataSource as BindingList<TableColumnInfo>; }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);

      if (this.DialogResult != System.Windows.Forms.DialogResult.OK)
        return;

      InfoBuffer buffer = new InfoBuffer();
      HashSet<string> unique = new HashSet<string>();

      if (this.Columns.Count == 0)
        buffer.Add(Resources.NO_COLUMNS, InfoLevel.Warning);

      foreach (var col in this.Columns)
      {
        if (string.IsNullOrEmpty(col.ColumnName))
          buffer.Add(Resources.EMPTY_COLUMNS, InfoLevel.Warning);
        else if (col.ColumnName.Contains('[') || col.ColumnName.Contains(']')
          || col.ColumnName.Contains('+') || col.ColumnName.Contains(','))
          buffer.Add(Resources.WRONG_COLUMN_NAME, InfoLevel.Warning);
        else if (!unique.Add(col.ColumnName))
          buffer.Add(string.Format(Resources.DUPLICATE_COLUMN, col.ColumnName), InfoLevel.Warning);
      }

      if (buffer.Count > 0)
      {
        AppManager.Notificator.Show(buffer);
        e.Cancel = true;
      }
    }
  }

  public class TableColumnInfo
  {
    public TableColumnInfo()
    {
      this.ColumnType = typeof(double);
    }

    public string ColumnName { get; set; }

    public Type ColumnType { get; set; }

    public static DataTable CreateTable(IList<TableColumnInfo> columns)
    {
      if (columns == null)
        throw new ArgumentNullException("columns");

      DataTable table = new DataTable();

      foreach (var col in columns)
      {
        var column = table.Columns.Add(col.ColumnName, col.ColumnType);

        if (column.DataType == typeof(string))
        {
          column.AllowDBNull = false;
          column.DefaultValue = string.Empty;
        }
      }
      return table;
    }
  }
}