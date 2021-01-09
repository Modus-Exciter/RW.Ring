namespace Notung.Helm.Dialogs
{
  partial class ProgressIndicatorDialog
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
      this.m_picture = new System.Windows.Forms.PictureBox();
      this.m_progress_bar = new System.Windows.Forms.ProgressBar();
      this.m_button = new System.Windows.Forms.Button();
      this.m_state_label = new System.Windows.Forms.Label();
      this.m_layout_panel = new System.Windows.Forms.TableLayoutPanel();
      this.m_table_button = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.m_picture)).BeginInit();
      this.m_layout_panel.SuspendLayout();
      this.m_table_button.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_picture
      // 
      this.m_picture.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_picture.Location = new System.Drawing.Point(9, 9);
      this.m_picture.Margin = new System.Windows.Forms.Padding(9, 9, 9, 9);
      this.m_picture.Name = "m_picture";
      this.m_layout_panel.SetRowSpan(this.m_picture, 3);
      this.m_picture.Size = new System.Drawing.Size(97, 88);
      this.m_picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.m_picture.TabIndex = 0;
      this.m_picture.TabStop = false;
      // 
      // m_progress_bar
      // 
      this.m_progress_bar.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_progress_bar.Location = new System.Drawing.Point(119, 32);
      this.m_progress_bar.Margin = new System.Windows.Forms.Padding(4, 4, 27, 4);
      this.m_progress_bar.MinimumSize = new System.Drawing.Size(0, 25);
      this.m_progress_bar.Name = "m_progress_bar";
      this.m_progress_bar.Size = new System.Drawing.Size(473, 25);
      this.m_progress_bar.TabIndex = 1;
      // 
      // m_button
      // 
      this.m_button.AutoSize = true;
      this.m_button.Location = new System.Drawing.Point(173, 4);
      this.m_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.m_button.Name = "m_button";
      this.m_button.Size = new System.Drawing.Size(111, 33);
      this.m_button.TabIndex = 2;
      this.m_button.Text = "Cancel";
      this.m_button.UseVisualStyleBackColor = true;
      // 
      // m_state_label
      // 
      this.m_state_label.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_state_label.Location = new System.Drawing.Point(119, 0);
      this.m_state_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.m_state_label.Name = "m_state_label";
      this.m_state_label.Size = new System.Drawing.Size(496, 28);
      this.m_state_label.TabIndex = 3;
      this.m_state_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // m_layout_panel
      // 
      this.m_layout_panel.ColumnCount = 2;
      this.m_layout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
      this.m_layout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.m_layout_panel.Controls.Add(this.m_picture, 0, 0);
      this.m_layout_panel.Controls.Add(this.m_progress_bar, 1, 1);
      this.m_layout_panel.Controls.Add(this.m_state_label, 1, 0);
      this.m_layout_panel.Controls.Add(this.m_table_button, 1, 2);
      this.m_layout_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_layout_panel.Location = new System.Drawing.Point(0, 0);
      this.m_layout_panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.m_layout_panel.Name = "m_layout_panel";
      this.m_layout_panel.RowCount = 3;
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_layout_panel.Size = new System.Drawing.Size(619, 106);
      this.m_layout_panel.TabIndex = 4;
      // 
      // m_table_button
      // 
      this.m_table_button.AutoSize = true;
      this.m_table_button.ColumnCount = 3;
      this.m_table_button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
      this.m_table_button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.m_table_button.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
      this.m_table_button.Controls.Add(this.m_button, 1, 0);
      this.m_table_button.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_table_button.Location = new System.Drawing.Point(119, 60);
      this.m_table_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.m_table_button.Name = "m_table_button";
      this.m_table_button.RowCount = 1;
      this.m_table_button.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.m_table_button.Size = new System.Drawing.Size(496, 42);
      this.m_table_button.TabIndex = 4;
      // 
      // ProgressIndicatorDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(619, 106);
      this.ControlBox = false;
      this.Controls.Add(this.m_layout_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(637, 120);
      this.Name = "ProgressIndicatorDialog";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "   ";
      ((System.ComponentModel.ISupportInitialize)(this.m_picture)).EndInit();
      this.m_layout_panel.ResumeLayout(false);
      this.m_layout_panel.PerformLayout();
      this.m_table_button.ResumeLayout(false);
      this.m_table_button.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox m_picture;
    private System.Windows.Forms.ProgressBar m_progress_bar;
    private System.Windows.Forms.Button m_button;
    private System.Windows.Forms.Label m_state_label;
    private System.Windows.Forms.TableLayoutPanel m_layout_panel;
    private System.Windows.Forms.TableLayoutPanel m_table_button;
  }
}