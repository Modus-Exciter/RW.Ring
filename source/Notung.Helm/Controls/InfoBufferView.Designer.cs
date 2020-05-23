namespace Notung.Helm.Controls
{
  partial class InfoBufferView
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoBufferView));
      this.m_tree = new System.Windows.Forms.TreeView();
      this.m_image_list = new System.Windows.Forms.ImageList(this.components);
      this.SuspendLayout();
      // 
      // m_tree
      // 
      this.m_tree.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_tree.ImageIndex = 0;
      this.m_tree.ImageList = this.m_image_list;
      this.m_tree.Location = new System.Drawing.Point(0, 0);
      this.m_tree.Name = "m_tree";
      this.m_tree.SelectedImageIndex = 0;
      this.m_tree.Size = new System.Drawing.Size(500, 411);
      this.m_tree.TabIndex = 0;
      // 
      // m_image_list
      // 
      this.m_image_list.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_image_list.ImageStream")));
      this.m_image_list.TransparentColor = System.Drawing.Color.Transparent;
      this.m_image_list.Images.SetKeyName(0, "Debug");
      this.m_image_list.Images.SetKeyName(1, "Info");
      this.m_image_list.Images.SetKeyName(2, "Warning");
      this.m_image_list.Images.SetKeyName(3, "Error");
      this.m_image_list.Images.SetKeyName(4, "Fatal");
      // 
      // InfoBufferView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_tree);
      this.Name = "InfoBufferView";
      this.Size = new System.Drawing.Size(500, 411);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView m_tree;
    private System.Windows.Forms.ImageList m_image_list;
  }
}
