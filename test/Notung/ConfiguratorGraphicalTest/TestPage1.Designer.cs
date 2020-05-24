namespace ConfiguratorGraphicalTest
{
  partial class TestPage1
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
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.numericUpDown = new System.Windows.Forms.NumericUpDown();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.m_xml_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_contract_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_group.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_xml_source)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_contract_source)).BeginInit();
      this.SuspendLayout();
      // 
      // m_group
      // 
      this.m_group.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_group.Controls.Add(this.textBox1);
      this.m_group.Controls.Add(this.label2);
      this.m_group.Controls.Add(this.label1);
      this.m_group.Controls.Add(this.numericUpDown);
      this.m_group.Location = new System.Drawing.Point(13, 13);
      this.m_group.Name = "m_group";
      this.m_group.Size = new System.Drawing.Size(321, 81);
      this.m_group.TabIndex = 0;
      this.m_group.TabStop = false;
      this.m_group.Text = "Contract settings";
      // 
      // textBox1
      // 
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_contract_source, "Text", true));
      this.textBox1.Location = new System.Drawing.Point(68, 45);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(247, 20);
      this.textBox1.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(28, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Text";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 21);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(44, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Number";
      // 
      // numericUpDown
      // 
      this.numericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.numericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.m_contract_source, "Number", true));
      this.numericUpDown.Location = new System.Drawing.Point(68, 19);
      this.numericUpDown.Name = "numericUpDown";
      this.numericUpDown.Size = new System.Drawing.Size(247, 20);
      this.numericUpDown.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.textBox2);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.numericUpDown1);
      this.groupBox1.Location = new System.Drawing.Point(13, 100);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(321, 83);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "XML settings";
      // 
      // textBox2
      // 
      this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_xml_source, "Text", true));
      this.textBox2.Location = new System.Drawing.Point(68, 45);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(247, 20);
      this.textBox2.TabIndex = 3;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 48);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(28, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Text";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 21);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(44, 13);
      this.label4.TabIndex = 1;
      this.label4.Text = "Number";
      // 
      // numericUpDown1
      // 
      this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.m_xml_source, "Number", true));
      this.numericUpDown1.Location = new System.Drawing.Point(68, 19);
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new System.Drawing.Size(247, 20);
      this.numericUpDown1.TabIndex = 0;
      // 
      // m_xml_source
      // 
      this.m_xml_source.DataSource = typeof(ConfiguratorGraphicalTest.OuterSectionXml);
      this.m_xml_source.CurrentItemChanged += new System.EventHandler(this.HandleChanged);
      // 
      // m_contract_source
      // 
      this.m_contract_source.DataSource = typeof(ConfiguratorGraphicalTest.OuterSectionDataContract);
      this.m_contract_source.CurrentItemChanged += new System.EventHandler(this.HandleChanged);
      // 
      // TestPage1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.m_group);
      this.Name = "TestPage1";
      this.Size = new System.Drawing.Size(348, 412);
      this.m_group.ResumeLayout(false);
      this.m_group.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_xml_source)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_contract_source)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox m_group;
    private System.Windows.Forms.BindingSource m_contract_source;
    private System.Windows.Forms.BindingSource m_xml_source;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.NumericUpDown numericUpDown;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
  }
}
