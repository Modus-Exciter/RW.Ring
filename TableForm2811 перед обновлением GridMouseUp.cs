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
            
            //m_grid.CellEndEdit += M_grid_CellEndEdit; // подписка на событие CellEndEdit
            m_grid.CellPainting += m_grid_CellPainting; // Подписка на событие CellPainting
            
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

        private void m_context_menu_Opening(object sender, CancelEventArgs e)
        {
            if (m_grid.ReadOnly)
                e.Cancel = true;
        }

       /* private void M_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
        }*/

        private void m_grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
            {
                e.PaintBackground(e.CellBounds, true);
                using (Pen pen = new Pen(SystemColors.Highlight, 3))
                {
                    e.Graphics.DrawRectangle(pen, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);
                }
                e.PaintContent(e.CellBounds);
                e.Handled = true;
            }
            else
            {
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
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
        private bool draggingFromHandle = false; // Флаг для перетаскивания за маркер в углу


        public bool IsDragging { get => isDragging; set => isDragging = value; }
        public int startColumnIndex1 { get => startColumnIndex; set => startColumnIndex = value; }
        public int startRowIndex1 { get => startRowIndex; set => startRowIndex = value; }

        public DragFillHelper(DataGridView grid)
        {
            this.grid = grid;
            grid.MouseDown += Grid_MouseDown;
            grid.MouseMove += Grid_MouseMove;
            grid.MouseUp += Grid_MouseUp;
            grid.CellPainting += Grid_CellPainting; // Добавлено для отрисовки маркера
        }
        public void CopyValue(int startCol, int startRow, int endCol, int endRow)
        {
            // Проверки на допустимые значения индексов
            if (startCol < 0 || startRow < 0 || endCol < 0 || endRow < 0 ||
                startRow >= grid.Rows.Count || endRow >= grid.Rows.Count ||
                startCol >= grid.Columns.Count || endCol >= grid.Columns.Count)
            {
                return; // Выходим, если индексы вне диапазона
            }

            object cellValue = grid.Rows[startRow].Cells[startCol].Value;
            if (cellValue == null) return; // Выходим, если значение null


            Type cellValueType = grid.Rows[startRow].Cells[startCol].ValueType;
            if (cellValueType == null) return; //Выходим если тип данных не определен

            int minRow = Math.Min(startRow, endRow);
            int maxRow = Math.Max(startRow, endRow);
            int minCol = Math.Min(startCol, endCol);
            int maxCol = Math.Max(startCol, endCol);


            for (int row = minRow; row <= maxRow; row++)
            {
                for (int col = minCol; col <= maxCol; col++)
                {
                    if (row != startRow || col != startCol)
                    {
                        try
                        {
                            //Проверка совместимости типов
                            if (grid.Rows[row].Cells[col].ValueType == cellValueType || grid.Rows[row].Cells[col].ValueType == typeof(object))
                            {
                                grid.Rows[row].Cells[col].Value = cellValue;
                            }
                            else
                            {
                                MessageBox.Show($"Несовместимые типы данных в ячейке ({row}, {col})!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при копировании значения в ячейку ({row}, {col}): {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }



        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Рисуем маленький квадрат в правом нижнем углу ячейки
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.State == DataGridViewElementStates.Selected) {
                Rectangle rect = e.CellBounds;
                rect.X = rect.Right - 15;
                rect.Y = rect.Bottom - 15;
                rect.Width = 15;
                rect.Height = 15;
                e.Graphics.FillRectangle(Brushes.Black, rect);
            }
        }

        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hit = grid.HitTest(e.X, e.Y);
            if (hit != null && !isDragging && e.Button == MouseButtons.Left && hit.Type == DataGridViewHitTestType.Cell)
            {
                grid.EndEdit(); // Завершаем редактирование, если оно активно
                startColumnIndex = hit.ColumnIndex;
                startRowIndex = hit.RowIndex;
              //startPoint = new Point(e.X, e.Y); // Сохраняем начальную точку
                startPoint = e.Location;
                Rectangle rect = grid.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, true);
                int tolerance = 5; // Допустимое отклонение в пикселях
                if (e.X > rect.Right - 15 && e.Y > rect.Bottom - 15) // Проверка на правый нижний угол
                {  
                  draggingFromHandle = true;
                  isDragging = true;
                  grid.Cursor = Cursors.SizeNWSE; // Меняем курсор на курсор изменения размера
                  Debug.WriteLine($"MouseDown: StartColumnIndex={startColumnIndex}, StartRowIndex={startRowIndex}");
                }
            }
        }
                
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (grid != null && grid.Enabled && isDragging && draggingFromHandle)
            {
                grid.Cursor = Cursors.Cross;
            }
            else
            {
                grid.Cursor = Cursors.Default;
            }
        }

        private void Grid_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging && draggingFromHandle)
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
                    draggingFromHandle = false;
                    grid.Cursor = Cursors.Default;
                    return; // Некорректная конечная точка
                }

                object cellValue = grid.Rows[startRowIndex].Cells[startColumnIndex].Value; // Значение начальной ячейки
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
                draggingFromHandle = false; // Сбрасываем флаг перетаскивания из правого угла ячейки
                grid.Cursor = Cursors.Default; // Возвращаем стандартный курсор
            }
        }
    }
}