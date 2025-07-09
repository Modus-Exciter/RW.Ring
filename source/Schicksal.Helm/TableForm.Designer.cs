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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableForm));
            this.m_grid = new System.Windows.Forms.DataGridView();
            this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_cmd_tbedit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
            this.m_filter_panel = new System.Windows.Forms.Panel();
            this.m_filter_layout = new System.Windows.Forms.FlowLayoutPanel();
            this.m_close_filter_button = new System.Windows.Forms.Button();
            this.m_unsort_button = new System.Windows.Forms.Button();
            this.m_filter_label = new System.Windows.Forms.Label();
            this.m_tool_tip = new System.Windows.Forms.ToolTip(this.components);
            this.m_switcher = new Notung.ComponentModel.LanguageSwitcher(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
            this.m_context_menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
            this.m_filter_panel.SuspendLayout();
            this.m_filter_layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_switcher)).BeginInit();
            this.SuspendLayout();
            // 
            // m_grid
            // 
            this.m_grid.AllowUserToResizeRows = false;
            this.m_grid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 28);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_grid.ContextMenuStrip = this.m_context_menu;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(1);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.m_grid.DefaultCellStyle = dataGridViewCellStyle2;
            this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.m_grid.Location = new System.Drawing.Point(0, 0);
            this.m_grid.Name = "m_grid";
            this.m_grid.RowHeadersWidth = 51;
            this.m_grid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.m_grid.Size = new System.Drawing.Size(630, 455);
            this.m_grid.TabIndex = 0;
            this.m_grid.ColumnHeadersHeightChanged += new System.EventHandler(this.HandleGridColumnHeadersHeightChanged);
            this.m_grid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.HandleGridCellEndEdit);
            this.m_grid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.HandleGridCellEnter);
            this.m_grid.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.HandleGridColumnWidthChanged);
            this.m_grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.HandleGridDataError);
            this.m_grid.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleGridScroll);
            this.m_grid.Sorted += new System.EventHandler(this.HandleGridSorted);
            this.m_grid.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.HandleGridUserDeletedRow);
            this.m_grid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HandleGridMouseClick);
            // 
            // m_context_menu
            // 
            this.m_context_menu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_tbedit});
            this.m_context_menu.Name = "m_context_menu";
            this.m_context_menu.Size = new System.Drawing.Size(124, 26);
            this.m_context_menu.Opening += new System.ComponentModel.CancelEventHandler(this.HandleContextMenuOpening);
            // 
            // m_cmd_tbedit
            // 
            this.m_cmd_tbedit.Name = "m_cmd_tbedit";
            this.m_cmd_tbedit.Size = new System.Drawing.Size(123, 22);
            this.m_cmd_tbedit.Text = "Edit table";
            this.m_cmd_tbedit.Click += new System.EventHandler(this.HandleEditTableClick);
            // 
            // m_filter_panel
            // 
            this.m_filter_panel.AutoSize = true;
            this.m_filter_panel.BackColor = System.Drawing.SystemColors.Info;
            this.m_filter_panel.Controls.Add(this.m_filter_layout);
            this.m_filter_panel.Controls.Add(this.m_filter_label);
            this.m_filter_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_filter_panel.Location = new System.Drawing.Point(0, 455);
            this.m_filter_panel.Margin = new System.Windows.Forms.Padding(2);
            this.m_filter_panel.Name = "m_filter_panel";
            this.m_filter_panel.Padding = new System.Windows.Forms.Padding(2);
            this.m_filter_panel.Size = new System.Drawing.Size(630, 28);
            this.m_filter_panel.TabIndex = 1;
            // 
            // m_filter_layout
            // 
            this.m_filter_layout.AutoSize = true;
            this.m_filter_layout.Controls.Add(this.m_close_filter_button);
            this.m_filter_layout.Controls.Add(this.m_unsort_button);
            this.m_filter_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_filter_layout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.m_filter_layout.Location = new System.Drawing.Point(10, 2);
            this.m_filter_layout.Margin = new System.Windows.Forms.Padding(0);
            this.m_filter_layout.Name = "m_filter_layout";
            this.m_filter_layout.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.m_filter_layout.Size = new System.Drawing.Size(618, 24);
            this.m_filter_layout.TabIndex = 4;
            // 
            // m_close_filter_button
            // 
            this.m_close_filter_button.AutoSize = true;
            this.m_close_filter_button.BackColor = System.Drawing.Color.Transparent;
            this.m_close_filter_button.BackgroundImage = global::Schicksal.Helm.Properties.Resources.funnel_delete;
            this.m_close_filter_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.m_close_filter_button.FlatAppearance.BorderSize = 0;
            this.m_close_filter_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_close_filter_button.Location = new System.Drawing.Point(594, 2);
            this.m_close_filter_button.Margin = new System.Windows.Forms.Padding(4, 2, 2, 2);
            this.m_close_filter_button.Name = "m_close_filter_button";
            this.m_close_filter_button.Size = new System.Drawing.Size(18, 20);
            this.m_close_filter_button.TabIndex = 0;
            this.m_close_filter_button.UseVisualStyleBackColor = false;
            this.m_close_filter_button.Visible = false;
            this.m_close_filter_button.Click += new System.EventHandler(this.HandleCloseFilterButtonClick);
            // 
            // m_unsort_button
            // 
            this.m_unsort_button.AutoSize = true;
            this.m_unsort_button.BackColor = System.Drawing.Color.Transparent;
            this.m_unsort_button.BackgroundImage = global::Schicksal.Helm.Properties.Resources.down_minus;
            this.m_unsort_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.m_unsort_button.FlatAppearance.BorderSize = 0;
            this.m_unsort_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_unsort_button.Location = new System.Drawing.Point(570, 2);
            this.m_unsort_button.Margin = new System.Windows.Forms.Padding(4, 2, 2, 2);
            this.m_unsort_button.Name = "m_unsort_button";
            this.m_unsort_button.Size = new System.Drawing.Size(18, 20);
            this.m_unsort_button.TabIndex = 2;
            this.m_unsort_button.UseVisualStyleBackColor = false;
            this.m_unsort_button.Visible = false;
            this.m_unsort_button.Click += new System.EventHandler(this.HandleUnsortClick);
            // 
            // m_filter_label
            // 
            this.m_filter_label.AutoSize = true;
            this.m_filter_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.m_filter_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_filter_label.Location = new System.Drawing.Point(2, 2);
            this.m_filter_label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_filter_label.Name = "m_filter_label";
            this.m_filter_label.Padding = new System.Windows.Forms.Padding(6, 2, 2, 2);
            this.m_filter_label.Size = new System.Drawing.Size(8, 19);
            this.m_filter_label.TabIndex = 1;
            this.m_filter_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_switcher
            // 
            this.m_switcher.LanguageChanged += new System.EventHandler<Notung.ComponentModel.LanguageEventArgs>(this.Switcher_LanguageChanged);
            // 
            // TableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(630, 483);
            this.Controls.Add(this.m_grid);
            this.Controls.Add(this.m_filter_panel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TableForm";
            ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
            this.m_context_menu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
            this.m_filter_panel.ResumeLayout(false);
            this.m_filter_panel.PerformLayout();
            this.m_filter_layout.ResumeLayout(false);
            this.m_filter_layout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_switcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    private System.Windows.Forms.Button m_unsort_button;
    private System.Windows.Forms.ToolTip m_tool_tip;
    private System.Windows.Forms.FlowLayoutPanel m_filter_layout;
  }
}