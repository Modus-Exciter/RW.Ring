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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableForm));
            this.m_grid = new System.Windows.Forms.DataGridView();
            this.m_context_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_cmd_tbedit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_switcher = new Notung.ComponentModel.LanguageSwitcher(this.components);
            this.reset = new System.Windows.Forms.Button();
            this.DoFilter = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
            this.m_context_menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_switcher)).BeginInit();
            this.SuspendLayout();
            // 
            // m_grid
            // 
            this.m_grid.AllowUserToOrderColumns = true;
            this.m_grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_grid.ContextMenuStrip = this.m_context_menu;
            this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.m_grid.Location = new System.Drawing.Point(0, 0);
            this.m_grid.Margin = new System.Windows.Forms.Padding(4);
            this.m_grid.Name = "m_grid";
            this.m_grid.RowHeadersWidth = 62;
            this.m_grid.Size = new System.Drawing.Size(795, 571);
            this.m_grid.TabIndex = 0;
            this.m_grid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_grid_CellEndEdit);
            this.m_grid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_grid_CellEnter);
            this.m_grid.ColumnHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.m_grid_ColumnHeaderMouseDoubleClick);
            this.m_grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.m_grid.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.m_grid_UserDeletedRow);
            this.m_grid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.m_grid_MouseClick);
            // 
            // m_context_menu
            // 
            this.m_context_menu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.m_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_cmd_tbedit});
            this.m_context_menu.Name = "m_context_menu";
            this.m_context_menu.Size = new System.Drawing.Size(143, 28);
            this.m_context_menu.Opening += new System.ComponentModel.CancelEventHandler(this.m_context_menu_Opening);
            // 
            // m_cmd_tbedit
            // 
            this.m_cmd_tbedit.Name = "m_cmd_tbedit";
            this.m_cmd_tbedit.Size = new System.Drawing.Size(142, 24);
            this.m_cmd_tbedit.Text = "Edit table";
            this.m_cmd_tbedit.Click += new System.EventHandler(this.m_cmd_tbedit_Click);
            // 
            // m_switcher
            // 
            this.m_switcher.LanguageChanged += new System.EventHandler<Notung.ComponentModel.LanguageEventArgs>(this.Switcher_LanguageChanged);
            // 
            // reset
            // 
            this.reset.Location = new System.Drawing.Point(24, 0);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(25, 23);
            this.reset.TabIndex = 2;
            this.reset.Text = "X";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // DoFilter
            // 
            this.DoFilter.Location = new System.Drawing.Point(0, 0);
            this.DoFilter.Name = "DoFilter";
            this.DoFilter.Size = new System.Drawing.Size(27, 23);
            this.DoFilter.TabIndex = 3;
            this.DoFilter.Text = "🔍︎";
            this.DoFilter.UseVisualStyleBackColor = true;
            this.DoFilter.Click += new System.EventHandler(this.DoFilter_Click);
            // 
            // TableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 571);
            this.Controls.Add(this.DoFilter);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.m_grid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TableForm";
            ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
            this.m_context_menu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_switcher)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView m_grid;
    private System.Windows.Forms.ContextMenuStrip m_context_menu;
    private Notung.ComponentModel.LanguageSwitcher m_switcher;
        private System.Windows.Forms.ToolStripMenuItem m_cmd_tbedit;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.Button DoFilter;
    }
}