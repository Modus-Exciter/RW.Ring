namespace ConfiguratorGraphicalTest
{
  partial class TestPage2
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
      this.m_group = new System.Windows.Forms.GroupBox();
      this.m_contract_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_xml_source = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.m_contract_source)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_xml_source)).BeginInit();
      this.SuspendLayout();
      // 
      // m_group
      // 
      this.m_group.Location = new System.Drawing.Point(24, 19);
      this.m_group.Name = "m_group";
      this.m_group.Size = new System.Drawing.Size(239, 235);
      this.m_group.TabIndex = 0;
      this.m_group.TabStop = false;
      this.m_group.Text = "Second settings";
      // 
      // m_contract_source
      // 
      this.m_contract_source.DataSource = typeof(ConfiguratorGraphicalTest.OuterSectionDataContractName);
      // 
      // m_xml_source
      // 
      this.m_xml_source.DataSource = typeof(ConfiguratorGraphicalTest.OuterSectionXmlName);
      // 
      // TestPage2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_group);
      this.Name = "TestPage2";
      this.Size = new System.Drawing.Size(500, 412);
      ((System.ComponentModel.ISupportInitialize)(this.m_contract_source)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_xml_source)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox m_group;
    private System.Windows.Forms.BindingSource m_contract_source;
    private System.Windows.Forms.BindingSource m_xml_source;
  }
}
