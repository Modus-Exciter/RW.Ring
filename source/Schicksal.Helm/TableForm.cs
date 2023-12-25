using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Notung;
using Notung.Services;
using Schicksal.Helm.Dialogs;
using Schicksal.Helm.Properties;

namespace Schicksal.Helm
{
    public partial class TableForm : Form
    {
        TextBox[] filterTextBoxes;
        List<string> columnNames;

        public TableForm()
        {
            this.Padding = new Padding(0, 25, 0, 0);
            this.InitializeComponent();
        }

        public DataTable DataSource
        {
            get { return m_grid.DataSource as DataTable; }
            set
            {
                if (ReferenceEquals(value, m_grid.DataSource))
                    return;

                m_grid.DataSource = value;

                // инициализация массива полей и имен колонок
                filterTextBoxes = new TextBox[m_grid.ColumnCount];
                columnNames = GetColumnNames();
                
                if (value != null)
                {
                    value.AcceptChanges();

                    if (m_grid.ReadOnly)
                    {
                        foreach (DataGridViewColumn col in m_grid.Columns)
                        {
                            // Отступ для строки фильтра
                            col.HeaderCell.Style.Padding = new Padding(0, 0, 0, 25);

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

            // Добавление полей поиска
            AddSearchControlToColumnHeader(m_grid);

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


        private List<string> GetColumnNames()
        {
            List<string> names = new List<string>();

            DataTable dataTable = m_grid.DataSource as DataTable;
            if (dataTable != null)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    names.Add(column.ColumnName);
                }
            }

            return names;
        }


        private void AddSearchControlToColumnHeader(DataGridView dataGridView)
        {
            int i = 0;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {

                // Создаем текстовое поле
                TextBox textBox = new TextBox();

                // Добавляем текстовое поле в массив
                filterTextBoxes[i] = textBox;

                // Позиционирование поля
                Rectangle headerRect = m_grid.GetCellDisplayRectangle(column.Index, -1, true);
                textBox.Location = new Point(headerRect.X, headerRect.Y + 50);
                textBox.Size = new Size(headerRect.Width, headerRect.Height);

                this.Controls.Add(textBox);
                i++;
            }
            this.Controls.SetChildIndex(m_grid, Controls.Count - 1);
        }


        private void reset_Click(object sender, EventArgs e)
        {
            // очистка полей поиска и сброс фильтра
            foreach (var textBox in filterTextBoxes)
                textBox.Text = "";

            (m_grid.DataSource as DataTable).DefaultView.RowFilter = "";
        }


        private void m_grid_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Проверка, является ли щелчок мыши на заголовке столбца
            if (e.Button == MouseButtons.Left && e.RowIndex == -1)
            {
                int index = e.ColumnIndex;
                if (filterTextBoxes[index] != null)
                {
                    TextBox textBox = new TextBox();

                    // Добавляем текстовое поле в массив
                    filterTextBoxes[index] = textBox;

                    // Позиционирование поля относительно щелкнутого заголовка столбца
                    Rectangle headerRect = m_grid.GetCellDisplayRectangle(index, -1, true);
                    textBox.Location = new Point(headerRect.X, headerRect.Y);
                    textBox.Size = new Size(headerRect.Width, headerRect.Height);

                   

                    // Добавление поля к родительскому контейнеру DataGridView
                    m_grid.Parent.Controls.Add(textBox);
                }
            }
        }

        private void DoFilter_Click(object sender, EventArgs e)
        {
            string filterExpression = "";

            for (int i = 0; i < filterTextBoxes.Length; i++)
            {
                if (filterTextBoxes[i] != null)
                {
                    filterExpression += $"Convert([{columnNames[i]}], 'System.String') LIKE '%{filterTextBoxes[i].Text}%' AND ";
                }
            }

            filterExpression = filterExpression.TrimEnd(" AND ".ToCharArray()); // Удалить последний " AND "

            (m_grid.DataSource as DataTable).DefaultView.RowFilter = filterExpression;
        }

        private void m_grid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            int i = 0;

            foreach(var textBox in filterTextBoxes)
            {
                // Позиционирование поля
                Rectangle headerRect = m_grid.GetCellDisplayRectangle(i, -1, true);
                textBox.Location = new Point(headerRect.X, headerRect.Y + 50);
                textBox.Size = new Size(headerRect.Width, headerRect.Height);

                i++;
            }      
        }
    }
}
