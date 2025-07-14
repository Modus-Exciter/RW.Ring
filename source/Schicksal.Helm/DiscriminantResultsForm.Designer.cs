
namespace Schicksal.Helm
{
  partial class DiscriminantResultsForm
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
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.treeViewDecision = new System.Windows.Forms.TreeView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SuspendLayout();
            // 
            // txtSummary
            // 
            this.txtSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSummary.HideSelection = false;
            this.txtSummary.Location = new System.Drawing.Point(0, 0);
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSummary.Size = new System.Drawing.Size(800, 22);
            this.txtSummary.TabIndex = 0;
            // 
            // treeViewDecision
            // 
            this.treeViewDecision.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewDecision.Location = new System.Drawing.Point(0, 22);
            this.treeViewDecision.Name = "treeViewDecision";
            this.treeViewDecision.Size = new System.Drawing.Size(800, 428);
            this.treeViewDecision.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "Готово";
            // 
            // DiscriminantResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.treeViewDecision);
            this.Controls.Add(this.txtSummary);
            this.Name = "DiscriminantResultsForm";
            this.Text = "DiscriminantResultsForm";
            this.Load += new System.EventHandler(this.DiscriminantResultsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtSummary;
    private System.Windows.Forms.TreeView treeViewDecision;
    private System.Windows.Forms.StatusStrip statusStrip1;
  }
}