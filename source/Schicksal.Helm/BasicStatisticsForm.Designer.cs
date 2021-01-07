namespace Schicksal.Helm
{
  partial class BasicStatisticsForm
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasicStatisticsForm));
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.m_col_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_col_mean = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_col_median = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_col_min = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_col_max = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_col_count = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_col_error = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_col_interval = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cmd_export = new System.Windows.Forms.ToolStripMenuItem();
      this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_lang = new Notung.ComponentModel.LanguageSwitcher(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      this.m_context_menu.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_lang)).BeginInit();
      this.SuspendLayout();
      // 
      // m_grid
      // 
      this.m_grid.AutoGenerateColumns = false;
      this.m_grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_col_description,
            this.m_col_mean,
            this.m_col_median,
            this.m_col_min,
            this.m_col_max,
            this.m_col_count,
            this.m_col_error,
            this.m_col_interval});
      this.m_grid.ContextMenuStrip = this.m_context_menu;
      this.m_grid.DataSource = this.m_binding_source;
      this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid.Location = new System.Drawing.Point(0, 0);
      this.m_grid.Name = "m_grid";
      this.m_grid.Size = new System.Drawing.Size(905, 451);
      this.m_grid.TabIndex = 0;
      // 
      // m_col_description
      // 
      this.m_col_description.DataPropertyName = "Description";
      this.m_col_description.HeaderText = "Description";
      this.m_col_description.Name = "m_col_description";
      this.m_col_description.ReadOnly = true;
      // 
      // m_col_mean
      // 
      this.m_col_mean.DataPropertyName = "Mean";
      this.m_col_mean.HeaderText = "Mean";
      this.m_col_mean.Name = "m_col_mean";
      this.m_col_mean.ReadOnly = true;
      // 
      // m_col_median
      // 
      this.m_col_median.DataPropertyName = "Median";
      this.m_col_median.HeaderText = "Median";
      this.m_col_median.Name = "m_col_median";
      this.m_col_median.ReadOnly = true;
      // 
      // m_col_min
      // 
      this.m_col_min.DataPropertyName = "Min";
      this.m_col_min.HeaderText = "Min";
      this.m_col_min.Name = "m_col_min";
      this.m_col_min.ReadOnly = true;
      // 
      // m_col_max
      // 
      this.m_col_max.DataPropertyName = "Max";
      this.m_col_max.HeaderText = "Max";
      this.m_col_max.Name = "m_col_max";
      this.m_col_max.ReadOnly = true;
      // 
      // m_col_count
      // 
      this.m_col_count.DataPropertyName = "Count";
      this.m_col_count.HeaderText = "Count";
      this.m_col_count.Name = "m_col_count";
      this.m_col_count.ReadOnly = true;
      // 
      // m_col_error
      // 
      this.m_col_error.DataPropertyName = "StdError";
      this.m_col_error.HeaderText = "StdError";
      this.m_col_error.Name = "m_col_error";
      this.m_col_error.ReadOnly = true;
      // 
      // m_col_interval
      // 
      this.m_col_interval.DataPropertyName = "ConfidenceInterval";
      this.m_col_interval.HeaderText = "ConfidenceInterval";
      this.m_col_interval.Name = "m_col_interval";
      this.m_col_interval.ReadOnly = true;
      // 
      // m_context_menu
      // 
      this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_export});
      this.m_context_menu.Name = "m_context_menu";
      this.m_context_menu.Size = new System.Drawing.Size(118, 26);
      // 
      // m_cmd_export
      // 
      this.m_cmd_export.Name = "m_cmd_export";
      this.m_cmd_export.Size = new System.Drawing.Size(117, 22);
      this.m_cmd_export.Text = "Export";
      this.m_cmd_export.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
      // 
      // m_binding_source
      // 
      this.m_binding_source.DataSource = typeof(Schicksal.Basic.DescriptionStatisticsEntry);
      // 
      // m_lang
      // 
      this.m_lang.LanguageChanged += new System.EventHandler<Notung.ComponentModel.LanguageEventArgs>(this.m_lang_LanguageChanged);
      // 
      // BasicStatisticsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(905, 451);
      this.Controls.Add(this.m_grid);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "BasicStatisticsForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "BasicStatisticsForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      this.m_context_menu.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_lang)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.BindingSource m_binding_source;
    private Notung.ComponentModel.LanguageSwitcher m_lang;
    private System.Windows.Forms.DataGridViewTextBoxColumn m_col_description;
    private System.Windows.Forms.DataGridViewTextBoxColumn m_col_mean;
    private System.Windows.Forms.DataGridViewTextBoxColumn m_col_median;
    private System.Windows.Forms.DataGridViewTextBoxColumn m_col_min;
    private System.Windows.Forms.DataGridViewTextBoxColumn m_col_max;
    private System.Windows.Forms.DataGridViewTextBoxColumn m_col_count;
    private System.Windows.Forms.DataGridViewTextBoxColumn m_col_error;
    private System.Windows.Forms.DataGridViewTextBoxColumn m_col_interval;
    private System.Windows.Forms.ContextMenuStrip m_context_menu;
    private System.Windows.Forms.ToolStripMenuItem m_cmd_export;
  }
}