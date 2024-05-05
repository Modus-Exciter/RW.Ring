
namespace Schicksal.Helm.Dialogs
{
  partial class ClusteringParametersDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.m_all_columns = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.m_add_button = new System.Windows.Forms.Button();
            this.m_del_button = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.m_ok_button = new System.Windows.Forms.Button();
            this.m_cancel_button = new System.Windows.Forms.Button();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.m_metric_combobox = new System.Windows.Forms.ComboBox();
            this.m_clust_algo_combobox = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.23401F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, -2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.26087F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.73913F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(638, 575);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.45614F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.54386F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 290F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel8, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(632, 375);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(343, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.56911F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.43089F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(284, 369);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(278, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selected columns";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.m_all_columns, 0, 1);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.84011F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.15989F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(275, 369);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(269, 40);
            this.label2.TabIndex = 0;
            this.label2.Text = "All columns";
            // 
            // m_all_columns
            // 
            this.m_all_columns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_all_columns.FormattingEnabled = true;
            this.m_all_columns.ItemHeight = 20;
            this.m_all_columns.Location = new System.Drawing.Point(3, 43);
            this.m_all_columns.Name = "m_all_columns";
            this.m_all_columns.Size = new System.Drawing.Size(269, 323);
            this.m_all_columns.TabIndex = 1;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Controls.Add(this.m_add_button, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.m_del_button, 0, 2);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(284, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 4;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 56.61765F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43.38235F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(53, 369);
            this.tableLayoutPanel8.TabIndex = 2;
            // 
            // m_add_button
            // 
            this.m_add_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_add_button.Location = new System.Drawing.Point(3, 80);
            this.m_add_button.Name = "m_add_button";
            this.m_add_button.Size = new System.Drawing.Size(47, 53);
            this.m_add_button.TabIndex = 0;
            this.m_add_button.Text = "add";
            this.m_add_button.UseVisualStyleBackColor = true;
            // 
            // m_del_button
            // 
            this.m_del_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_del_button.Location = new System.Drawing.Point(3, 139);
            this.m_del_button.Name = "m_del_button";
            this.m_del_button.Size = new System.Drawing.Size(47, 56);
            this.m_del_button.TabIndex = 1;
            this.m_del_button.Text = "del";
            this.m_del_button.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 384);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.80851F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.19149F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(632, 188);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 4;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel6.Controls.Add(this.m_ok_button, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.m_cancel_button, 3, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 137);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(623, 48);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // m_ok_button
            // 
            this.m_ok_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ok_button.Location = new System.Drawing.Point(467, 3);
            this.m_ok_button.Name = "m_ok_button";
            this.m_ok_button.Size = new System.Drawing.Size(77, 42);
            this.m_ok_button.TabIndex = 0;
            this.m_ok_button.Text = "OK";
            this.m_ok_button.UseVisualStyleBackColor = true;
            // 
            // m_cancel_button
            // 
            this.m_cancel_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_cancel_button.Location = new System.Drawing.Point(550, 3);
            this.m_cancel_button.Name = "m_cancel_button";
            this.m_cancel_button.Size = new System.Drawing.Size(70, 42);
            this.m_cancel_button.TabIndex = 1;
            this.m_cancel_button.Text = "cancel";
            this.m_cancel_button.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.m_metric_combobox, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.m_clust_algo_combobox, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.13636F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(623, 128);
            this.tableLayoutPanel7.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(305, 63);
            this.label3.TabIndex = 0;
            this.label3.Text = "Metric";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(305, 65);
            this.label5.TabIndex = 2;
            this.label5.Text = "Clustering algorithm";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_metric_combobox
            // 
            this.m_metric_combobox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_metric_combobox.FormattingEnabled = true;
            this.m_metric_combobox.Location = new System.Drawing.Point(314, 3);
            this.m_metric_combobox.Name = "m_metric_combobox";
            this.m_metric_combobox.Size = new System.Drawing.Size(306, 28);
            this.m_metric_combobox.TabIndex = 3;
            this.m_metric_combobox.SelectedIndexChanged += new System.EventHandler(this.m_metric_combobox_SelectedIndexChanged);
            // 
            // m_clust_algo_combobox
            // 
            this.m_clust_algo_combobox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_clust_algo_combobox.FormattingEnabled = true;
            this.m_clust_algo_combobox.Location = new System.Drawing.Point(314, 66);
            this.m_clust_algo_combobox.Name = "m_clust_algo_combobox";
            this.m_clust_algo_combobox.Size = new System.Drawing.Size(306, 28);
            this.m_clust_algo_combobox.TabIndex = 5;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 42);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(278, 324);
            this.dataGridView1.TabIndex = 1;
            // 
            // ClusteringParametersDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 571);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ClusteringParametersDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ClusteringParametersDialog";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
    private System.Windows.Forms.ListBox m_all_columns;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
    private System.Windows.Forms.Button m_add_button;
    private System.Windows.Forms.Button m_del_button;
    private System.Windows.Forms.Button m_ok_button;
    private System.Windows.Forms.Button m_cancel_button;
    private System.Windows.Forms.ComboBox m_metric_combobox;
    private System.Windows.Forms.ComboBox m_clust_algo_combobox;
    private System.Windows.Forms.DataGridView dataGridView1;
  }
}