namespace Schicksal.Helm
{
  partial class TableForm
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableForm));
      this.m_grid = new System.Windows.Forms.DataGridView();
      this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_cmd_tbedit = new System.Windows.Forms.ToolStripMenuItem();
      this.m_switcher = new Notung.ComponentModel.LanguageSwitcher(this.components);
      this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_filter_panel = new System.Windows.Forms.Panel();
      this.m_filter_label = new System.Windows.Forms.Label();
      this.m_close_filter_button = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      this.m_context_menu.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_switcher)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
      this.m_filter_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_grid
      // 
      this.m_grid.AllowUserToOrderColumns = true;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 26);
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.m_grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.m_grid.ContextMenuStrip = this.m_context_menu;
      this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.m_grid.Location = new System.Drawing.Point(0, 0);
      this.m_grid.Margin = new System.Windows.Forms.Padding(4);
      this.m_grid.Name = "m_grid";
      this.m_grid.RowHeadersWidth = 62;
      this.m_grid.Size = new System.Drawing.Size(795, 541);
      this.m_grid.TabIndex = 0;
      this.m_grid.RowHeadersWidthChanged += new System.EventHandler(this.HandleGridRowHeadersWidthChanged);
      this.m_grid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.HandleGridCellEndEdit);
      this.m_grid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.HandleGridCellEnter);
      this.m_grid.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.HandleGridColumnWidthChanged);
      this.m_grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.HandleGridDataError);
      this.m_grid.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleGridScroll);
      this.m_grid.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.HandleGridUserDeletedRow);
      this.m_grid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HandleGridMouseClick);
      // 
      // m_context_menu
      // 
      this.m_context_menu.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_tbedit});
      this.m_context_menu.Name = "m_context_menu";
      this.m_context_menu.Size = new System.Drawing.Size(143, 28);
      this.m_context_menu.Opening += new System.ComponentModel.CancelEventHandler(this.HandleContextMenuOpening);
      // 
      // m_cmd_tbedit
      // 
      this.m_cmd_tbedit.Name = "m_cmd_tbedit";
      this.m_cmd_tbedit.Size = new System.Drawing.Size(142, 24);
      this.m_cmd_tbedit.Text = "Edit table";
      this.m_cmd_tbedit.Click += new System.EventHandler(this.HandleEditTableClick);
      // 
      // m_switcher
      // 
      this.m_switcher.LanguageChanged += new System.EventHandler<Notung.ComponentModel.LanguageEventArgs>(this.Switcher_LanguageChanged);
      // 
      // m_filter_panel
      // 
      this.m_filter_panel.BackColor = System.Drawing.SystemColors.Info;
      this.m_filter_panel.Controls.Add(this.m_filter_label);
      this.m_filter_panel.Controls.Add(this.m_close_filter_button);
      this.m_filter_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_filter_panel.Location = new System.Drawing.Point(0, 541);
      this.m_filter_panel.Name = "m_filter_panel";
      this.m_filter_panel.Size = new System.Drawing.Size(795, 30);
      this.m_filter_panel.TabIndex = 1;
      // 
      // m_filter_label
      // 
      this.m_filter_label.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_filter_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_filter_label.Location = new System.Drawing.Point(0, 0);
      this.m_filter_label.Name = "m_filter_label";
      this.m_filter_label.Padding = new System.Windows.Forms.Padding(8, 3, 3, 3);
      this.m_filter_label.Size = new System.Drawing.Size(765, 30);
      this.m_filter_label.TabIndex = 1;
      this.m_filter_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_close_filter_button
      // 
      this.m_close_filter_button.AutoSize = true;
      this.m_close_filter_button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_close_filter_button.BackColor = System.Drawing.Color.Transparent;
      this.m_close_filter_button.Dock = System.Windows.Forms.DockStyle.Right;
      this.m_close_filter_button.FlatAppearance.BorderSize = 0;
      this.m_close_filter_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.m_close_filter_button.Image = global::Schicksal.Helm.Properties.Resources.funnel_delete;
      this.m_close_filter_button.Location = new System.Drawing.Point(765, 0);
      this.m_close_filter_button.Name = "m_close_filter_button";
      this.m_close_filter_button.Size = new System.Drawing.Size(30, 30);
      this.m_close_filter_button.TabIndex = 0;
      this.m_close_filter_button.UseVisualStyleBackColor = false;
      this.m_close_filter_button.Click += new System.EventHandler(this.HandleCloseFilterButtonClick);
      // 
      // TableForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(795, 571);
      this.Controls.Add(this.m_grid);
      this.Controls.Add(this.m_filter_panel);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "TableForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      this.m_context_menu.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_switcher)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
      this.m_filter_panel.ResumeLayout(false);
      this.m_filter_panel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.ContextMenuStrip m_context_menu;
    private Notung.ComponentModel.LanguageSwitcher m_switcher;
        private System.Windows.Forms.ToolStripMenuItem m_cmd_tbedit;
    private System.Windows.Forms.BindingSource m_binding_source;
    private System.Windows.Forms.Panel m_filter_panel;
    private System.Windows.Forms.Button m_close_filter_button;
    private System.Windows.Forms.Label m_filter_label;
  }
}