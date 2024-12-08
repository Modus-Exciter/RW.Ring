using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Notung;
using Notung.Services;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;
using System.Diagnostics;

namespace Schicksal.Helm
{
  public partial class TableForm : Form
  {
    private DragFillHelper dragFillHelper;

    public TableForm()
    {
      this.InitializeComponent();

      dragFillHelper = new DragFillHelper(m_grid);
      // подписка на событие CellEndEdit
      m_grid.CellEndEdit += M_grid_CellEndEdit;
    }

    public DataTable DataSource
    {
      get { return m_grid.DataSource as DataTable; }
      set
      {
        if (ReferenceEquals(value, m_grid.DataSource))
          return;

        m_grid.DataSource = value;

        if (value != null)
        {
          value.AcceptChanges();

          if (m_grid.ReadOnly)
          {
            foreach (DataGridViewColumn col in m_grid.Columns)
            {
              if (col.ValueType == typeof(double) || col.ValueType == typeof(float))
                col.DefaultCellStyle.Format = "0.000";
            }
          }
        }
      }
    }

    public string FileName { get; set; }

    public void MarkAsReadOnly()
    {
      m_grid.ReadOnly = true;
      m_grid.AllowUserToAddRows = false;
      m_grid.AllowUserToDeleteRows = false;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.Icon = this.Icon.Clone() as Icon;
    }

    protected override void OnShown(System.EventArgs e)
    {
      base.OnShown(e);

      if (m_grid.Rows.Count < (1 << 10))
        m_grid.AutoResizeColumns();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      if (this.DataSource != null && this.DataSource.GetChanges() != null)
      {
        var res = AppManager.Notificator.ConfirmOrCancel(string.Format(
          Resources.SAVE_FILE_CONFIRM, this.Text.Substring(0, this.Text.Length - 1)), InfoLevel.Info);

        if (res == null)
          e.Cancel = true;
        else if (res == true)
        {
          this.Save();

          var parent = this.MdiParent as MainForm;

          if (parent != null)
            parent.FillLastFilesMenu();
        }
      }
    }

    private void SaveFile(string fileName, DataTable table)
    {
      if (File.Exists(fileName))
        File.Delete(fileName);

      table.AcceptChanges();

      using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        DataTableSaver.WriteDataTable(table, fs);
      }

      AppManager.Configurator.GetSection<Program.Preferences>().LastFiles[fileName] = DateTime.Now;

      this.FileName = Path.GetFileName(this.FileName);
      this.Text = Path.GetFileName(this.FileName);
    }

    public void Save()
    {
      if (string.IsNullOrEmpty(this.FileName))
        this.SaveAs();
      else
        this.SaveFile(this.FileName, this.DataSource);
    }

    public void SaveAs()
    {
      using (var dlg = new SaveFileDialog())
      {
        dlg.Filter = "Schicksal data files|*.sks";

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          this.FileName = dlg.FileName;
          this.SaveFile(this.FileName, this.DataSource);
        }
      }
    }

    private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      AppManager.Notificator.Show(e.Exception.Message, InfoLevel.Error);
    }

    private void Switcher_LanguageChanged(object sender, Notung.ComponentModel.LanguageEventArgs e)
    {
      m_cmd_tbedit.Text = Resources.TABLE_EDIT;
    }

    private void m_cmd_tbedit_Click(object sender, EventArgs e)
    {
      using (var dlg = new EditColumnsDialog())
      {
        dlg.Text = Resources.TABLE_EDIT;
        TableColumnInfo.FillColumnInfo(dlg.Columns, this.DataSource);

        if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
        {
          m_grid.DataSource = TableColumnInfo.CreateUpdatedTable(dlg.Columns, this.DataSource);

          if (!this.Text.EndsWith("*"))
            this.Text += "*";
        }
      }
    }

    private void m_grid_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
      m_grid.BeginEdit(true);
    }

    private void m_grid_MouseClick(object sender, MouseEventArgs e)
    {
      var info = m_grid.HitTest(e.X, e.Y);
      DataGridViewCell cell = null;

      if (info != null && info.ColumnIndex >= 0 && info.RowIndex >= 0)
        cell = m_grid.Rows[info.RowIndex].Cells[info.ColumnIndex];

      if (m_grid.CurrentCell != null && m_grid.CurrentCell != cell)
      {
        if (m_grid.CurrentCell.IsInEditMode)
          m_grid.EndEdit();
        else
          m_grid.BeginEdit(false);
      }
    }

    private void m_grid_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
    {
      if (!this.Text.EndsWith("*"))
        this.Text += "*";
    }

    private void m_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      var row = m_grid.Rows[e.RowIndex].DataBoundItem as DataRowView;

      if (row == null)
        return;

      if (row.Row.RowState != DataRowState.Unchanged && !this.Text.EndsWith("*"))
        this.Text += "*";
    }

    private void m_context_menu_Opening(object sender, CancelEventArgs e)
    {
      if (m_grid.ReadOnly)
        e.Cancel = true;
    }

    private void M_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      if (dragFillHelper.IsDragging == true && e.RowIndex >= 0 && e.ColumnIndex >= 0)
      {
        try
        {
          int endColumnIndex = e.ColumnIndex;
          int endRowIndex = e.RowIndex;
          var cellValue = m_grid.Rows[dragFillHelper.startRowIndex].Cells[dragFillHelper.startColumnIndex].Value;

          if (cellValue != null)
          {
            int minRow = Math.Min(dragFillHelper.startRowIndex, endRowIndex);
            int maxRow = Math.Max(dragFillHelper.startRowIndex, endRowIndex);
            int minCol = Math.Min(dragFillHelper.startColumnIndex, endColumnIndex);
            int maxCol = Math.Max(dragFillHelper.startColumnIndex, endColumnIndex);

            for (int row = minRow; row <= maxRow; row++)
            {
              if (row < m_grid.Rows.Count)
              {
                for (int col = minCol; col <= maxCol; col++)
                {
                  if (col < m_grid.Columns.Count)  // Добавлена проверка на количество столбцов
                  {
                    try
                    {
                      m_grid.Rows[row].Cells[col].Value = cellValue;
                    }
                    catch (Exception ex)
                    {
                      Debug.WriteLine($"Ошибка при записи значения: {ex.Message}");
                      Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                    }
                  }
                }
              }
            }
          }
          dragFillHelper.IsDragging = false;
          m_grid.Cursor = Cursors.Default;

        }
        catch (ArgumentOutOfRangeException ex)
        {
          Debug.WriteLine($"ArgumentOutOfRangeException in m_grid_CellEndEdit: {ex.Message}");
          dragFillHelper.IsDragging = false;
          m_grid.Cursor = Cursors.Default;
        }
        catch (Exception ex) // Добавлена общая обработка исключений
        {
          Debug.WriteLine($"Непредвиденная ошибка в m_grid_CellEndEdit: {ex.Message}");
          Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
          dragFillHelper.IsDragging = false;
          m_grid.Cursor = Cursors.Default;
        }
      }
    }

    private void m_grid_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Escape)
      {
        if (m_grid.CurrentCell != null && m_grid.CurrentCell.IsInEditMode)
        {
          m_grid.CancelEdit();
        }
      }
    }
  }

  public class DragFillHelper
  {
    private DataGridView grid;
    internal int startColumnIndex;
    internal int startRowIndex;
    private bool isDragging = false;
    private Point startPoint; // Добавлена переменная startPoint

    public bool IsDragging { get => isDragging; set => isDragging = value; }
    public int startColumnIndex1 { get => startColumnIndex; set => startColumnIndex = value; }
    public int startRowIndex1 { get => startRowIndex; set => startRowIndex = value; }

    public DragFillHelper(DataGridView grid)
    {
      this.grid = grid;
      grid.MouseDown += Grid_MouseDown;
      grid.MouseMove += Grid_MouseMove;
      grid.MouseUp += Grid_MouseUp;
    }

    private void Grid_MouseDown(object sender, MouseEventArgs e)
    {
      DataGridView.HitTestInfo hit = grid.HitTest(e.X, e.Y);
      if (e.Button == MouseButtons.Left && hit.Type == DataGridViewHitTestType.Cell)
      {
        grid.EndEdit(); // Завершаем редактирование, если оно активно
        startColumnIndex = hit.ColumnIndex;
        startRowIndex = hit.RowIndex;
        startPoint = new Point(e.X, e.Y); // Сохраняем начальную точку
        isDragging = true;
        grid.Cursor = Cursors.SizeNWSE; // Меняем курсор на курсор изменения размера
        Debug.WriteLine($"MouseDown: StartColumnIndex={startColumnIndex}, StartRowIndex={startRowIndex}");
      }
    }

    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
      if (isDragging && grid.Rows.Count > 0)
      {
        try
        {
          // Обновление курсора на основе направления перетаскивания.
          if (Math.Abs(e.X - startPoint.X) > Math.Abs(e.Y - startPoint.Y))
          {
            grid.Cursor = Cursors.SizeWE;
          }
          else
          {
            grid.Cursor = Cursors.SizeNS;
          }
        }
        catch (ArgumentOutOfRangeException ex)
        {
          Debug.WriteLine($"ArgumentOutOfRangeException in Grid_MouseMove: {ex.Message}");
          isDragging = false;
          grid.Cursor = Cursors.Default;
        }
      }
    }

    private void Grid_MouseUp(object sender, MouseEventArgs e)
    {
      if (isDragging)
      {
        DataGridView.HitTestInfo hit = grid.HitTest(e.X, e.Y);
        int endColumnIndex = hit.ColumnIndex; // Индекс столбца конечной ячейки
        int endRowIndex = hit.RowIndex;     // Индекс строки конечной ячейки

        Debug.WriteLine($"MouseUp: EndColumnIndex={endColumnIndex}, EndRowIndex={endRowIndex}");

        if (endColumnIndex < 0 || endRowIndex < 0 || startColumnIndex < 0 || startRowIndex < 0 ||
            endRowIndex >= grid.Rows.Count || startRowIndex >= grid.Rows.Count ||
            endColumnIndex >= grid.Columns.Count || startColumnIndex >= grid.Columns.Count)
        {
          isDragging = false;
          grid.Cursor = Cursors.Default;
          return; // Некорректная конечная точка
        }

        var cellValue = grid.Rows[startRowIndex].Cells[startColumnIndex].Value; // Значение начальной ячейки

        if (cellValue != null)
        {
          int minRow = Math.Min(startRowIndex, endRowIndex); // Минимальный индекс строки
          int maxRow = Math.Max(startRowIndex, endRowIndex); // Максимальный индекс строки
          int minCol = Math.Min(startColumnIndex, endColumnIndex); // Минимальный индекс столбца
          int maxCol = Math.Max(startColumnIndex, endColumnIndex); // Максимальный индекс столбца

          Debug.WriteLine($"Copying value from ({startRowIndex}, {startColumnIndex}) to range ({minRow}, {minCol}) - ({maxRow}, {maxCol})");

          for (int row = minRow; row <= maxRow; row++)
          {
            if (row >= 0 && row < grid.Rows.Count)
            {
              for (int col = minCol; col <= maxCol; col++)
              {
                if (col >= 0 && col < grid.Columns.Count)
                {
                  // Проверяем, что текущая ячейка не является начальной ячейкой
                  if (row != startRowIndex || col != startColumnIndex)
                  {
                    grid.Rows[row].Cells[col].Value = cellValue; // Копируем значение в ячейки
                    Debug.WriteLine($"Set value at ({row}, {col}) to {cellValue}");
                  }
                }
              }
            }
          }
        }

        isDragging = false; // Сбрасываем флаг перетаскивания
        grid.Cursor = Cursors.Default; // Возвращаем стандартный курсор
      }
    }
  }
}