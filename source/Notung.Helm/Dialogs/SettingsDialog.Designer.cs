namespace Notung.Helm.Dialogs
{
  partial class SettingsDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.m_section_panel = new System.Windows.Forms.Panel();
      this.m_button_panel = new System.Windows.Forms.FlowLayoutPanel();
      this.m_button_cancel = new System.Windows.Forms.Button();
      this.m_button_apply = new System.Windows.Forms.Button();
      this.m_button_ok = new System.Windows.Forms.Button();
      this.m_sections_tree = new System.Windows.Forms.TreeView();
      this.m_image_list = new System.Windows.Forms.ImageList(this.components);
      this.m_errors_view = new System.Windows.Forms.DataGridView();
      this.messageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.m_settings_controller = new Notung.Helm.Configuration.SettingsController(this.components);
      this.m_button_panel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_errors_view)).BeginInit();
      this.SuspendLayout();
      // 
      // m_section_panel
      // 
      this.m_section_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_section_panel.Location = new System.Drawing.Point(209, 5);
      this.m_section_panel.Name = "m_section_panel";
      this.m_section_panel.Size = new System.Drawing.Size(575, 482);
      this.m_section_panel.TabIndex = 2;
      // 
      // m_button_panel
      // 
      this.m_button_panel.Controls.Add(this.m_button_cancel);
      this.m_button_panel.Controls.Add(this.m_button_apply);
      this.m_button_panel.Controls.Add(this.m_button_ok);
      this.m_button_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_button_panel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
      this.m_button_panel.Location = new System.Drawing.Point(5, 539);
      this.m_button_panel.Name = "m_button_panel";
      this.m_button_panel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
      this.m_button_panel.Size = new System.Drawing.Size(779, 35);
      this.m_button_panel.TabIndex = 0;
      // 
      // m_button_cancel
      // 
      this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_button_cancel.Location = new System.Drawing.Point(701, 8);
      this.m_button_cancel.Name = "m_button_cancel";
      this.m_button_cancel.Size = new System.Drawing.Size(75, 23);
      this.m_button_cancel.TabIndex = 0;
      this.m_button_cancel.Text = "Cancel";
      this.m_button_cancel.UseVisualStyleBackColor = true;
      // 
      // m_button_apply
      // 
      this.m_button_apply.Location = new System.Drawing.Point(620, 8);
      this.m_button_apply.Name = "m_button_apply";
      this.m_button_apply.Size = new System.Drawing.Size(75, 23);
      this.m_button_apply.TabIndex = 1;
      this.m_button_apply.Text = "Apply";
      this.m_button_apply.UseVisualStyleBackColor = true;
      this.m_button_apply.Click += new System.EventHandler(this.m_button_apply_Click);
      // 
      // m_button_ok
      // 
      this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_button_ok.Location = new System.Drawing.Point(539, 8);
      this.m_button_ok.Name = "m_button_ok";
      this.m_button_ok.Size = new System.Drawing.Size(75, 23);
      this.m_button_ok.TabIndex = 2;
      this.m_button_ok.Text = "OK";
      this.m_button_ok.UseVisualStyleBackColor = true;
      // 
      // m_sections_tree
      // 
      this.m_sections_tree.Dock = System.Windows.Forms.DockStyle.Left;
      this.m_sections_tree.ImageIndex = 0;
      this.m_sections_tree.ImageList = this.m_image_list;
      this.m_sections_tree.ItemHeight = 32;
      this.m_sections_tree.Location = new System.Drawing.Point(5, 5);
      this.m_sections_tree.Name = "m_sections_tree";
      this.m_sections_tree.SelectedImageIndex = 0;
      this.m_sections_tree.ShowRootLines = false;
      this.m_sections_tree.Size = new System.Drawing.Size(204, 482);
      this.m_sections_tree.TabIndex = 1;
      this.m_sections_tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.m_sections_list_AfterSelect);
      // 
      // m_image_list
      // 
      this.m_image_list.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_image_list.ImageStream")));
      this.m_image_list.TransparentColor = System.Drawing.Color.Transparent;
      this.m_image_list.Images.SetKeyName(0, "Empty");
      // 
      // m_errors_view
      // 
      this.m_errors_view.AllowUserToAddRows = false;
      this.m_errors_view.AllowUserToDeleteRows = false;
      this.m_errors_view.AllowUserToOrderColumns = true;
      this.m_errors_view.AllowUserToResizeColumns = false;
      this.m_errors_view.AllowUserToResizeRows = false;
      this.m_errors_view.AutoGenerateColumns = false;
      this.m_errors_view.BackgroundColor = System.Drawing.SystemColors.Window;
      this.m_errors_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.m_errors_view.ColumnHeadersVisible = false;
      this.m_errors_view.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.messageDataGridViewTextBoxColumn});
      this.m_errors_view.DataSource = this.m_settings_controller;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.m_errors_view.DefaultCellStyle = dataGridViewCellStyle1;
      this.m_errors_view.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.m_errors_view.Location = new System.Drawing.Point(5, 487);
      this.m_errors_view.Name = "m_errors_view";
      this.m_errors_view.ReadOnly = true;
      this.m_errors_view.RowHeadersVisible = false;
      this.m_errors_view.Size = new System.Drawing.Size(779, 52);
      this.m_errors_view.TabIndex = 0;
      this.m_errors_view.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_errors_view_CellDoubleClick);
      // 
      // messageDataGridViewTextBoxColumn
      // 
      this.messageDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.messageDataGridViewTextBoxColumn.DataPropertyName = "Message";
      this.messageDataGridViewTextBoxColumn.HeaderText = "Message";
      this.messageDataGridViewTextBoxColumn.Name = "messageDataGridViewTextBoxColumn";
      this.messageDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // m_settings_controller
      // 
      this.m_settings_controller.PagePlace = this.m_section_panel;
      this.m_settings_controller.ValidationResults = this.m_errors_view;
      this.m_settings_controller.PageChanged += new System.EventHandler<Notung.Helm.Configuration.PageEventArgs>(this.m_settings_controller_PageChanged);
      // 
      // SettingsDialog
      // 
      this.AcceptButton = this.m_button_ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.m_button_cancel;
      this.ClientSize = new System.Drawing.Size(789, 579);
      this.Controls.Add(this.m_section_panel);
      this.Controls.Add(this.m_sections_tree);
      this.Controls.Add(this.m_errors_view);
      this.Controls.Add(this.m_button_panel);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(480, 320);
      this.Name = "SettingsDialog";
      this.Padding = new System.Windows.Forms.Padding(5);
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Settings";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsDialog_FormClosing);
      this.Load += new System.EventHandler(this.SettingsDialog_Load);
      this.m_button_panel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.m_errors_view)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Configuration.SettingsController m_settings_controller;
    private System.Windows.Forms.FlowLayoutPanel m_button_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_apply;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.TreeView m_sections_tree;
    private System.Windows.Forms.Panel m_section_panel;
    private System.Windows.Forms.DataGridView m_errors_view;
    private System.Windows.Forms.DataGridViewTextBoxColumn messageDataGridViewTextBoxColumn;
    private System.Windows.Forms.ImageList m_image_list;
  }
}