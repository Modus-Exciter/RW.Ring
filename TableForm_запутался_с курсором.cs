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

            m_grid.CellPainting += m_grid_CellPainting; // Подписка на событие CellPainting
            m_grid.CellMouseMove += m_grid_CellMouseMove; // Подписка на событие CellMouseMove
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

        private void m_grid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Проверяем, что курсор находится внутри DataGridView и над действительной ячейкой
            if (e.ColumnIndex < 0 || e.RowIndex < 0) 
            {
                return; // Если курсор вне ячейки, выходим из метода
            }

            // Получаем прямоугольник, описывающий ячейку
            Rectangle rect = m_grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

            // Вычисляем размер "ручки" в правом нижнем углу ячейки. 
            // Используем минимум из ширины и высоты ячейки, чтобы ручка не была больше самой ячейки.
            // Гарантируем минимальный размер ручки в 5 пикселей.
            int handleSize = Math.Max(Math.Min(rect.Width, rect.Height) / 5, 5); 
            int tolerance = 2; // Добавляем небольшой допуск для более удобного попадания по ручке

            // Проверяем, находится ли курсор над областью "ручки"
            bool overHandle = (e.X > rect.Right - handleSize - tolerance) && (e.Y > rect.Bottom - handleSize - tolerance);

            // Меняем курсор в зависимости от того, находится ли курсор над ручкой или нет.
            // Только если перетаскивание НЕ началось (!isDragging)
            if (!isDragging) 
            {
                m_grid.Cursor = overHandle ? Cursors.Cross : Cursors.Default; 
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
            //grid.MouseMove += Grid_MouseMove;
            grid.MouseUp += Grid_MouseUp;
            grid.CellPainting += Grid_CellPainting; // Добавлено для отрисовки маркера
        }

        private void CopyValue(int startCol, int startRow, int endCol, int endRow, object cellValue, bool copyRowsOnly, bool copyColumnsOnly)
        {
            if (grid.ReadOnly) return;

            // Вывод информации в отладчик о диапазоне копирования
            string copyDirection = copyRowsOnly ? "строке" : "столбце";
            int endRowValue = copyRowsOnly ? startRow : endRow;
            int endColValue = copyRowsOnly ? endCol : endCol;

            Debug.WriteLine($"Копирование значения '{cellValue}' из ячейки ({startRow}, {startCol}) в {copyDirection} до ячейки ({endRowValue}, {endColValue})");

            if (copyRowsOnly)
            {
                CopyRow(startRow, startCol, endCol, cellValue);
            }
            else if (copyColumnsOnly)
            {
                CopyColumn(startCol, startRow, endRow, cellValue);
            }
        }

        private void CopyRow(int row, int startCol, int endCol, object cellValue)
        {
            // Определяем направление копирования по столбцам
            int step = startCol <= endCol ? 1 : -1; // 1 - если копируем вправо, -1 - если влево

            for (int col = startCol; step > 0 ? col <= endCol : col >= endCol; col += step)
            {
                DataGridViewCell cell = grid.Rows[row]?.Cells[col];
                if (cell != null)
                {
                    SetCellValue(cell, cellValue);
                }
                else
                {
                    Debug.WriteLine($"Ошибка: ячейка ({row}, {col}) не найдена.");
                }
            }
        }


        private void CopyColumn(int col, int startRow, int endRow, object cellValue)
        {
            // Определяем направление копирования по строкам
            int step = startRow <= endRow ? 1 : -1; // 1 - если копируем вниз, -1 - если вверх

            for (int row = startRow; step > 0 ? row <= endRow : row >= endRow; row += step)
            {
                DataGridViewCell cell = grid.Rows[row]?.Cells[col];
                if (cell != null)
                {
                    SetCellValue(cell, cellValue);
                }
                else
                {
                    Debug.WriteLine($"Ошибка: ячейка ({row}, {col}) не найдена.");
                }
            }
        }

        private void SetCellValue(DataGridViewCell cell, object cellValue)
        {
            try
            {
                // Если значение null, просто присваиваем null
                if (cellValue == null || cellValue == DBNull.Value)
                {
                    cell.Value = null;
                    return;
                }
                
                Type cellValueType = cell.ValueType;
                Type valueType = cellValue.GetType();

                if (cellValueType == null)
                {
                    cell.Value = cellValue;
                    return;
                }
                // Если тип значения совместим с типом ячейки, присваиваем значение
                else if (cell.ValueType.IsAssignableFrom(cellValue.GetType()))
                {
                    cell.Value = cellValue;
                    return;
                }
                else if (cellValueType == typeof(string))
                {
                    cell.Value = cellValue.ToString();
                }
                else if (cellValueType == typeof(object))
                {
                    cell.Value = cellValue;
                }
                // Если типы не совместимы, пытаемся конвертировать значение
                else
                {
                    try
                    {
                        cell.Value = Convert.ChangeType(cellValue, cellValueType);
                    }
                    catch (InvalidCastException)
                    {
                        MessageBox.Show($"Невозможно преобразовать значение '{cellValue}' типа '{valueType}' к типу '{cellValueType}'", "Ошибка преобразования типа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show($"Неверный формат значения '{cellValue}' для типа '{cellValueType}'", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка при преобразовании: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            // Обработка любых других исключений
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при копировании значения в ячейку ({cell.RowIndex}, {cell.ColumnIndex}): {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Рисуем маленький квадрат в правом нижнем углу ячейки
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.State == DataGridViewElementStates.Selected)
            {
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
            // Проверяем, что DataGridView существует, перетаскивание не началось и нажата левая кнопка мыши
            if (m_grid != null && !isDragging && e.Button == MouseButtons.Left)
            {
                // Получаем информацию о ячейке, на которую нажал пользователь
                DataGridView.HitTestInfo hit = m_grid.HitTest(e.X, e.Y);

                // Проверяем, что нажатие произошло на ячейку
                if (hit != null && hit.Type == DataGridViewHitTestType.Cell)
                {
                    m_grid.EndEdit(); // Завершаем редактирование, если оно активно

                    // Сохраняем начальные координаты
                    startColumnIndex = hit.ColumnIndex;
                    startRowIndex = hit.RowIndex;
                    startPoint = e.Location;

                    // Получаем прямоугольник, описывающий ячейку
                    Rectangle rect = m_grid.GetCellDisplayRectangle(hit.ColumnIndex, hit.RowIndex, true);

                    // Расчет handleSize с минимальным размером и проверкой на 0
                    int handleSize = Math.Max(Math.Min(rect.Width, rect.Height) / 5, 5); // Минимальный размер 5 пикселей
                    int tolerance = 2;

                    Debug.WriteLine($"handleSize: {handleSize}, rect.Width: {rect.Width}, rect.Height: {rect.Height}");

                    // Проверяем, находится ли курсор в области "ручки"
                    bool isHandleClicked = (handleSize > 0) && (e.X > rect.Right - handleSize - tolerance) && (e.Y > rect.Bottom - handleSize - tolerance);

                    draggingFromHandle = isHandleClicked;
                    isDragging = true; // Перетаскивание началось

                    m_grid.Cursor = Cursors.SizeAll; // Устанавливаем курсор SizeAll при начале перетаскивания

                    Debug.WriteLine($"MouseDown: StartColumnIndex={startColumnIndex}, StartRowIndex={startRowIndex}, draggingFromHandle={draggingFromHandle}, handleSize={handleSize}");
                }
            }
}

        /*private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_grid != null && m_grid.Enabled && isDragging && draggingFromHandle)
            {
                m_grid.Cursor = Cursors.Cross;
            }
            else
            {
                m_grid.Cursor = Cursors.Default;
            }
        }*/

        private void Grid_MouseUp(object sender, MouseEventArgs e)
        {
            // Проверяем, что перетаскивание началось и осуществляется за "ручку"
            if (isDragging && draggingFromHandle)
            {
                // Получаем информацию о ячейке, на которой пользователь отпустил кнопку мыши
                DataGridView.HitTestInfo hit = m_grid.HitTest(e.X, e.Y);
                if (hit == null || hit.ColumnIndex < 0 || hit.RowIndex < 0)
                {
                    ResetDragging();
                    return;
                }
                // Получаем индексы столбца и строки конечной ячейки
                int endColumnIndex = hit.ColumnIndex;
                int endRowIndex = hit.RowIndex;

                Debug.WriteLine($"MouseUp: EndColumnIndex={endColumnIndex}, EndRowIndex={endRowIndex}");
                // Получаем строку и ячейку, с которых началось перетаскивание
                DataGridViewRow startRow = m_grid.Rows[startRowIndex];
                DataGridViewCell startCell = startRow?.Cells[startColumnIndex];

                // Проверка границ и null-проверок в одно условие
                if (startRow == null || startCell == null || endRowIndex >= m_grid.Rows.Count || endColumnIndex >= m_grid.Columns.Count)
                {
                    ResetDragging();
                    return;
                }
                // Получаем значение исходной ячейки
                object cellValue = startCell.Value;
                if (cellValue == null)
                {
                    ResetDragging();
                    return; // Значение исходной ячейки равно null
                }

                // Определяем, копируем ли только строки или столбцы
                bool copyRowsOnly = endRowIndex == startRowIndex && endColumnIndex != startColumnIndex; // Копирование только по строке, если индекс строки тот же, а индекс столбца другой
                bool copyColumnsOnly = endColumnIndex == startColumnIndex && endRowIndex != startRowIndex; // Копирование только по столбцу, если индекс столбца тот же, а индекс строки другой
                // Проверяем, что пользователь перетащил курсор хотя бы на одну ячейку
                if (endRowIndex != startRowIndex || endColumnIndex != startColumnIndex)
                {
                    // Если копируется только строка или только столбец
                    if (copyRowsOnly || copyColumnsOnly)
                    {
                        try
                        {
                            // Вызываем метод копирования значений
                            CopyValue(startColumnIndex, startRowIndex, endColumnIndex, endRowIndex, cellValue, copyRowsOnly, copyColumnsOnly);
                        }
                        // Обработка любых исключений, возникающих при копировании
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при копировании: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Debug.WriteLine($"Exception in Grid_MouseUp: {ex}");
                        }
                        finally
                        {
                            // Сбрасываем состояние перетаскивания
                            ResetDragging();
                        }
                    }
                }
                else
                {
                    ResetDragging(); // Сбрасываем, если не было движения
                }

            }
        }

        public void ResetDragging()
        {
            isDragging = false;
            draggingFromHandle = false;
            if (grid != null)
            { // проверка на null, на случай если grid не был инициализирован
                grid.Cursor = Cursors.Default;
            }
        }
    }
}
