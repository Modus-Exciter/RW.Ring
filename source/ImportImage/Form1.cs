using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            openFileDialog1.Filter = "Image files(*.png)|*.png|All files(*.*)|*.*";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;

            transformationImage(filename);
        }

        private void transformationImage(string filename)
        {
            Bitmap bitmapImage = new Bitmap(filename);

            string[,] masName = new string[bitmapImage.Height, bitmapImage.Width];
            int[,] masARGB = new int[bitmapImage.Height, bitmapImage.Width];
            int x, y = 0;
            for (x = 0; x < bitmapImage.Height; x++)
                for (y = 0; y < bitmapImage.Width; y++)
                {
                    Color pixelColor = bitmapImage.GetPixel(y, x);
                    masName[x, y] = pixelColor.Name;
                    masARGB[x, y] = pixelColor.ToArgb();
                }

            dataGridView1.ColumnCount = masName.GetUpperBound(1) + 1;
            dataGridView1.RowCount = masName.GetUpperBound(0) + 1;

            if (comboBox1.SelectedIndex == -1 || comboBox1.SelectedIndex == 0)
                fillCode(masName, masARGB);
            else
                fillColor(masName, masARGB);

        }

        /// <summary>
        /// Бавыкина Д.А. Метод, заполняющий ячейки DataGridView кодами цветов
        /// </summary>
        private void fillCode(string[,] masName, int[,] masARGB)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int k = 0; k < dataGridView1.ColumnCount; k++)
                {
                    dataGridView1.Rows[i].Cells[k].Value = masName[i, k];
                    DataGridViewColumn column = dataGridView1.Columns[i];
                    column.Width = 125;
                    DataGridViewRow row = dataGridView1.Rows[i];
                    row.Height = 24;
                }
        }

        /// <summary>
        /// Бавыкина Д.А. Метод, закрашивающий ячейки DataGridView
        /// </summary>
        private void fillColor(string[,] masName, int[,] masARGB)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int k = 0; k < dataGridView1.ColumnCount; k++)
                {
                    dataGridView1.Rows[i].Cells[k].Value = "";
                    dataGridView1.Rows[i].Cells[k].Style.BackColor = Color.FromArgb(masARGB[i, k]);
                    DataGridViewColumn column = dataGridView1.Columns[i];
                    column.Width = 10;
                    DataGridViewRow row = dataGridView1.Rows[i];
                    row.Height = 10;
                }
        }
    }
}
