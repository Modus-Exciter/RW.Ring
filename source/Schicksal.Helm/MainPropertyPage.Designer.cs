namespace Schicksal.Helm
{
  partial class MainPropertyPage
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
      this.m_layout_panel = new System.Windows.Forms.TableLayoutPanel();
      this.m_significant_label = new System.Windows.Forms.Label();
      this.m_exclusive_label = new System.Windows.Forms.Label();
      this.m_locale_label = new System.Windows.Forms.Label();
      this.m_significat_panel = new System.Windows.Forms.Panel();
      this.m_significat_button = new System.Windows.Forms.Button();
      this.m_binging_source = new System.Windows.Forms.BindingSource(this.components);
      this.m_exclusive_panel = new System.Windows.Forms.Panel();
      this.m_exclusive_button = new System.Windows.Forms.Button();
      this.m_language_edit = new System.Windows.Forms.ComboBox();
      this.m_buton_open = new System.Windows.Forms.Button();
      this.m_switcher = new Notung.ComponentModel.LanguageSwitcher(this.components);
      this.m_layout_panel.SuspendLayout();
      this.m_significat_panel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_binging_source)).BeginInit();
      this.m_exclusive_panel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_switcher)).BeginInit();
      this.SuspendLayout();
      // 
      // m_layout_panel
      // 
      this.m_layout_panel.ColumnCount = 3;
      this.m_layout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.m_layout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
      this.m_layout_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
      this.m_layout_panel.Controls.Add(this.m_significant_label, 0, 1);
      this.m_layout_panel.Controls.Add(this.m_exclusive_label, 0, 2);
      this.m_layout_panel.Controls.Add(this.m_locale_label, 0, 3);
      this.m_layout_panel.Controls.Add(this.m_significat_panel, 1, 1);
      this.m_layout_panel.Controls.Add(this.m_exclusive_panel, 1, 2);
      this.m_layout_panel.Controls.Add(this.m_language_edit, 1, 3);
      this.m_layout_panel.Controls.Add(this.m_buton_open, 1, 4);
      this.m_layout_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_layout_panel.Location = new System.Drawing.Point(0, 0);
      this.m_layout_panel.Name = "m_layout_panel";
      this.m_layout_panel.RowCount = 6;
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
      this.m_layout_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
      this.m_layout_panel.Size = new System.Drawing.Size(457, 331);
      this.m_layout_panel.TabIndex = 0;
      // 
      // m_significant_label
      // 
      this.m_significant_label.AutoSize = true;
      this.m_significant_label.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_significant_label.Location = new System.Drawing.Point(3, 39);
      this.m_significant_label.Name = "m_significant_label";
      this.m_significant_label.Size = new System.Drawing.Size(82, 33);
      this.m_significant_label.TabIndex = 0;
      this.m_significant_label.Text = "Significant color";
      this.m_significant_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_exclusive_label
      // 
      this.m_exclusive_label.AutoSize = true;
      this.m_exclusive_label.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_exclusive_label.Location = new System.Drawing.Point(3, 72);
      this.m_exclusive_label.Name = "m_exclusive_label";
      this.m_exclusive_label.Size = new System.Drawing.Size(82, 33);
      this.m_exclusive_label.TabIndex = 1;
      this.m_exclusive_label.Text = "Exclusive color";
      this.m_exclusive_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_locale_label
      // 
      this.m_locale_label.AutoSize = true;
      this.m_locale_label.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_locale_label.Location = new System.Drawing.Point(3, 105);
      this.m_locale_label.Name = "m_locale_label";
      this.m_locale_label.Size = new System.Drawing.Size(82, 33);
      this.m_locale_label.TabIndex = 2;
      this.m_locale_label.Text = "Language";
      this.m_locale_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_significat_panel
      // 
      this.m_significat_panel.Controls.Add(this.m_significat_button);
      this.m_significat_panel.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", this.m_binging_source, "SignificatColor", true));
      this.m_significat_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_significat_panel.Location = new System.Drawing.Point(91, 42);
      this.m_significat_panel.Name = "m_significat_panel";
      this.m_significat_panel.Size = new System.Drawing.Size(240, 27);
      this.m_significat_panel.TabIndex = 3;
      // 
      // m_significat_button
      // 
      this.m_significat_button.AutoSize = true;
      this.m_significat_button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_significat_button.Dock = System.Windows.Forms.DockStyle.Right;
      this.m_significat_button.Location = new System.Drawing.Point(214, 0);
      this.m_significat_button.Name = "m_significat_button";
      this.m_significat_button.Size = new System.Drawing.Size(26, 27);
      this.m_significat_button.TabIndex = 0;
      this.m_significat_button.Text = "...";
      this.m_significat_button.UseVisualStyleBackColor = true;
      this.m_significat_button.Click += new System.EventHandler(this.m_significat_button_Click);
      // 
      // m_binging_source
      // 
      this.m_binging_source.DataSource = typeof(Schicksal.Helm.Program.Preferences);
      this.m_binging_source.CurrentItemChanged += new System.EventHandler(this.m_binging_source_CurrentItemChanged);
      // 
      // m_exclusive_panel
      // 
      this.m_exclusive_panel.Controls.Add(this.m_exclusive_button);
      this.m_exclusive_panel.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", this.m_binging_source, "ExclusiveColor", true));
      this.m_exclusive_panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_exclusive_panel.Location = new System.Drawing.Point(91, 75);
      this.m_exclusive_panel.Name = "m_exclusive_panel";
      this.m_exclusive_panel.Size = new System.Drawing.Size(240, 27);
      this.m_exclusive_panel.TabIndex = 4;
      // 
      // m_exclusive_button
      // 
      this.m_exclusive_button.AutoSize = true;
      this.m_exclusive_button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.m_exclusive_button.Dock = System.Windows.Forms.DockStyle.Right;
      this.m_exclusive_button.Location = new System.Drawing.Point(214, 0);
      this.m_exclusive_button.Name = "m_exclusive_button";
      this.m_exclusive_button.Size = new System.Drawing.Size(26, 27);
      this.m_exclusive_button.TabIndex = 1;
      this.m_exclusive_button.Text = "...";
      this.m_exclusive_button.UseVisualStyleBackColor = true;
      this.m_exclusive_button.Click += new System.EventHandler(this.m_exclusive_button_Click);
      // 
      // m_language_edit
      // 
      this.m_language_edit.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.m_binging_source, "Language", true));
      this.m_language_edit.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_language_edit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_language_edit.FormattingEnabled = true;
      this.m_language_edit.Items.AddRange(new object[] {
            "EN",
            "RU"});
      this.m_language_edit.Location = new System.Drawing.Point(91, 108);
      this.m_language_edit.Name = "m_language_edit";
      this.m_language_edit.Size = new System.Drawing.Size(240, 21);
      this.m_language_edit.TabIndex = 5;
      // 
      // m_buton_open
      // 
      this.m_buton_open.AutoSize = true;
      this.m_buton_open.Dock = System.Windows.Forms.DockStyle.Right;
      this.m_buton_open.Location = new System.Drawing.Point(224, 141);
      this.m_buton_open.Name = "m_buton_open";
      this.m_buton_open.Size = new System.Drawing.Size(107, 27);
      this.m_buton_open.TabIndex = 6;
      this.m_buton_open.Text = "Open system folder";
      this.m_buton_open.UseVisualStyleBackColor = true;
      this.m_buton_open.Click += new System.EventHandler(this.m_buton_open_Click);
      // 
      // m_switcher
      // 
      this.m_switcher.LanguageChanged += new System.EventHandler<Notung.ComponentModel.LanguageEventArgs>(this.m_switcher_LanguageChanged);
      // 
      // MainPropertyPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_layout_panel);
      this.Name = "MainPropertyPage";
      this.Size = new System.Drawing.Size(457, 331);
      this.m_layout_panel.ResumeLayout(false);
      this.m_layout_panel.PerformLayout();
      this.m_significat_panel.ResumeLayout(false);
      this.m_significat_panel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_binging_source)).EndInit();
      this.m_exclusive_panel.ResumeLayout(false);
      this.m_exclusive_panel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.m_switcher)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel m_layout_panel;
    private System.Windows.Forms.Label m_significant_label;
    private System.Windows.Forms.Label m_exclusive_label;
    private System.Windows.Forms.Label m_locale_label;
    private System.Windows.Forms.Panel m_exclusive_panel;
    private System.Windows.Forms.Button m_exclusive_button;
    private System.Windows.Forms.Panel m_significat_panel;
    private System.Windows.Forms.Button m_significat_button;
    private System.Windows.Forms.ComboBox m_language_edit;
    private System.Windows.Forms.BindingSource m_binging_source;
    private Notung.ComponentModel.LanguageSwitcher m_switcher;
    private System.Windows.Forms.Button m_buton_open;
  }
}
