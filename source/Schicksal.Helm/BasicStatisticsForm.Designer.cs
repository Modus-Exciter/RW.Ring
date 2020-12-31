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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasicStatisticsForm));
      this.m_grid = new System.Windows.Forms.DataGridView();
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).BeginInit();
      this.SuspendLayout();
      // 
      // m_grid
      // 
      this.m_grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_grid.Location = new System.Drawing.Point(0, 0);
      this.m_grid.Name = "m_grid";
      this.m_grid.Size = new System.Drawing.Size(524, 451);
      this.m_grid.TabIndex = 0;
      // 
      // BasicStatisticsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(524, 451);
      this.Controls.Add(this.m_grid);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "BasicStatisticsForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "BasicStatisticsForm";
      ((System.ComponentModel.ISupportInitialize)(this.m_grid)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView m_grid;
  }
}