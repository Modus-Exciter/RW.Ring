
namespace Schicksal.Helm.Dialogs
{
  partial class RegressionDetailsDialog
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
      this.m_layout_panel = new System.Windows.Forms.TableLayoutPanel();
      this.m_label_metrics1 = new System.Windows.Forms.Label();
      this.m_label_metrics2 = new System.Windows.Forms.Label();
      this.m_panel = new System.Windows.Forms.Panel();
      this.m_layout_panel.SuspendLayout();
      this.m_panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_layout_panel
      // 
      this.m_layout_panel.ColumnCount = 1;
      this.m_layout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_layout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_layout_panel.Controls.Add(this.m_label_metrics1, 0, 0);
      this.m_layout_panel.Controls.Add(this.m_label_metrics2, 0, 1);
      this.m_layout_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_layout_panel.Location = new System.Drawing.Point(0, 0);
      this.m_layout_panel.Margin = new System.Windows.Forms.Padding(4);
      this.m_layout_panel.Name = "m_layout_panel";
      this.m_layout_panel.RowCount = 2;
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.m_layout_panel.Size = new System.Drawing.Size(579, 112);
      this.m_layout_panel.TabIndex = 0;
      // 
      // m_label_metrics1
      // 
      this.m_label_metrics1.AutoSize = true;
      this.m_label_metrics1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_label_metrics1.Location = new System.Drawing.Point(4, 0);
      this.m_label_metrics1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.m_label_metrics1.Name = "m_label_metrics1";
      this.m_label_metrics1.Size = new System.Drawing.Size(571, 56);
      this.m_label_metrics1.TabIndex = 0;
      this.m_label_metrics1.Text = "label1";
      this.m_label_metrics1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.m_label_metrics1.Click += new System.EventHandler(this.RegressionDetailsDialog_Click);
      this.m_label_metrics1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RegressionDetailsDialog_MouseDown);
      this.m_label_metrics1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RegressionDetailsDialog_MouseMove);
      // 
      // m_label_metrics2
      // 
      this.m_label_metrics2.AutoSize = true;
      this.m_label_metrics2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_label_metrics2.Location = new System.Drawing.Point(4, 56);
      this.m_label_metrics2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.m_label_metrics2.Name = "m_label_metrics2";
      this.m_label_metrics2.Size = new System.Drawing.Size(571, 56);
      this.m_label_metrics2.TabIndex = 1;
      this.m_label_metrics2.Text = "label2";
      this.m_label_metrics2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.m_label_metrics2.Click += new System.EventHandler(this.RegressionDetailsDialog_Click);
      this.m_label_metrics2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RegressionDetailsDialog_MouseDown);
      this.m_label_metrics2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RegressionDetailsDialog_MouseMove);
      // 
      // m_panel
      // 
      this.m_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.m_panel.Controls.Add(this.m_layout_panel);
      this.m_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_panel.Location = new System.Drawing.Point(0, 0);
      this.m_panel.Margin = new System.Windows.Forms.Padding(4);
      this.m_panel.Name = "m_panel";
      this.m_panel.Size = new System.Drawing.Size(581, 114);
      this.m_panel.TabIndex = 1;
      // 
      // RegressionDetailsDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.OldLace;
      this.ClientSize = new System.Drawing.Size(581, 114);
      this.Controls.Add(this.m_panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "RegressionDetailsDialog";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "RegressionDetailsDialog";
      this.Deactivate += new System.EventHandler(this.RegressionDetailsDialog_Deactivate);
      this.m_layout_panel.ResumeLayout(false);
      this.m_layout_panel.PerformLayout();
      this.m_panel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel m_layout_panel;
    private System.Windows.Forms.Panel m_panel;
    private System.Windows.Forms.Label m_label_metrics1;
    private System.Windows.Forms.Label m_label_metrics2;
  }
}