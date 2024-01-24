
namespace SqlDbImport
{
  partial class SqlDbImportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlDbImportForm));
            this.m_server = new System.Windows.Forms.TextBox();
            this.m_bottom_panel = new System.Windows.Forms.FlowLayoutPanel();
            this.m_button_cancel = new System.Windows.Forms.Button();
            this.m_button_ok = new System.Windows.Forms.Button();
            this.m_database = new System.Windows.Forms.TextBox();
            this.lb_server = new System.Windows.Forms.Label();
            this.lb_database = new System.Windows.Forms.Label();
            this.lb_table = new System.Windows.Forms.Label();
            this.m_table = new System.Windows.Forms.TextBox();
            this.lb_login = new System.Windows.Forms.Label();
            this.m_login = new System.Windows.Forms.TextBox();
            this.lb_password = new System.Windows.Forms.Label();
            this.m_password = new System.Windows.Forms.TextBox();
            this.cb_integrated_security = new System.Windows.Forms.CheckBox();
            this.lb_windows_authentication = new System.Windows.Forms.Label();
            this.m_bottom_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_server
            // 
            this.m_server.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_server.Location = new System.Drawing.Point(94, 12);
            this.m_server.Name = "m_server";
            this.m_server.Size = new System.Drawing.Size(216, 20);
            this.m_server.TabIndex = 11;
            // 
            // m_bottom_panel
            // 
            this.m_bottom_panel.AutoSize = true;
            this.m_bottom_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_bottom_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_bottom_panel.Controls.Add(this.m_button_cancel);
            this.m_bottom_panel.Controls.Add(this.m_button_ok);
            this.m_bottom_panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_bottom_panel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.m_bottom_panel.Location = new System.Drawing.Point(0, 186);
            this.m_bottom_panel.Name = "m_bottom_panel";
            this.m_bottom_panel.Padding = new System.Windows.Forms.Padding(4);
            this.m_bottom_panel.Size = new System.Drawing.Size(322, 43);
            this.m_bottom_panel.TabIndex = 10;
            // 
            // m_button_cancel
            // 
            resources.ApplyResources(this.m_button_cancel, "m_button_cancel");
            this.m_button_cancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_button_cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_button_cancel.Location = new System.Drawing.Point(234, 7);
            this.m_button_cancel.Name = "m_button_cancel";
            this.m_button_cancel.Size = new System.Drawing.Size(75, 25);
            this.m_button_cancel.TabIndex = 0;
            this.m_button_cancel.UseVisualStyleBackColor = true;
            // 
            // m_button_ok
            // 
            resources.ApplyResources(this.m_button_ok, "m_button_ok");
            this.m_button_ok.AutoSize = true;
            this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_button_ok.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_button_ok.Location = new System.Drawing.Point(153, 7);
            this.m_button_ok.Name = "m_button_ok";
            this.m_button_ok.Size = new System.Drawing.Size(75, 27);
            this.m_button_ok.TabIndex = 1;
            this.m_button_ok.UseVisualStyleBackColor = true;
            // 
            // m_database
            // 
            this.m_database.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_database.Location = new System.Drawing.Point(94, 38);
            this.m_database.Name = "m_database";
            this.m_database.Size = new System.Drawing.Size(216, 20);
            this.m_database.TabIndex = 12;
            // 
            // lb_server
            // 
            resources.ApplyResources(this.lb_server, "lb_server");
            this.lb_server.AutoSize = true;
            this.lb_server.Location = new System.Drawing.Point(12, 15);
            this.lb_server.Name = "lb_server";
            this.lb_server.Size = new System.Drawing.Size(0, 13);
            this.lb_server.TabIndex = 13;
            // 
            // lb_database
            // 
            resources.ApplyResources(this.lb_database, "lb_database");
            this.lb_database.AutoSize = true;
            this.lb_database.Location = new System.Drawing.Point(13, 41);
            this.lb_database.Name = "lb_database";
            this.lb_database.Size = new System.Drawing.Size(0, 13);
            this.lb_database.TabIndex = 14;
            // 
            // lb_table
            // 
            resources.ApplyResources(this.lb_table, "lb_table");
            this.lb_table.AutoSize = true;
            this.lb_table.Location = new System.Drawing.Point(13, 67);
            this.lb_table.Name = "lb_table";
            this.lb_table.Size = new System.Drawing.Size(0, 13);
            this.lb_table.TabIndex = 16;
            // 
            // m_table
            // 
            this.m_table.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_table.Location = new System.Drawing.Point(94, 64);
            this.m_table.Name = "m_table";
            this.m_table.Size = new System.Drawing.Size(216, 20);
            this.m_table.TabIndex = 15;
            // 
            // lb_login
            // 
            resources.ApplyResources(this.lb_login, "lb_login");
            this.lb_login.AutoSize = true;
            this.lb_login.Location = new System.Drawing.Point(13, 121);
            this.lb_login.Name = "lb_login";
            this.lb_login.Size = new System.Drawing.Size(0, 13);
            this.lb_login.TabIndex = 18;
            // 
            // m_login
            // 
            this.m_login.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_login.Location = new System.Drawing.Point(94, 118);
            this.m_login.Name = "m_login";
            this.m_login.Size = new System.Drawing.Size(216, 20);
            this.m_login.TabIndex = 17;
            // 
            // lb_password
            // 
            resources.ApplyResources(this.lb_password, "lb_password");
            this.lb_password.AutoSize = true;
            this.lb_password.Location = new System.Drawing.Point(13, 147);
            this.lb_password.Name = "lb_password";
            this.lb_password.Size = new System.Drawing.Size(0, 13);
            this.lb_password.TabIndex = 20;
            // 
            // m_password
            // 
            this.m_password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_password.Location = new System.Drawing.Point(94, 144);
            this.m_password.Name = "m_password";
            this.m_password.Size = new System.Drawing.Size(216, 20);
            this.m_password.TabIndex = 19;
            // 
            // cb_integrated_security
            // 
            this.cb_integrated_security.AutoSize = true;
            this.cb_integrated_security.Location = new System.Drawing.Point(295, 91);
            this.cb_integrated_security.Name = "cb_integrated_security";
            this.cb_integrated_security.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cb_integrated_security.Size = new System.Drawing.Size(15, 14);
            this.cb_integrated_security.TabIndex = 21;
            this.cb_integrated_security.UseVisualStyleBackColor = true;
            this.cb_integrated_security.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // lb_windows_authentication
            // 
            resources.ApplyResources(this.lb_windows_authentication, "lb_windows_authentication");
            this.lb_windows_authentication.AutoSize = true;
            this.lb_windows_authentication.Location = new System.Drawing.Point(12, 92);
            this.lb_windows_authentication.Name = "lb_windows_authentication";
            this.lb_windows_authentication.Size = new System.Drawing.Size(0, 13);
            this.lb_windows_authentication.TabIndex = 22;
            // 
            // SqlDbImportForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 229);
            this.Controls.Add(this.lb_windows_authentication);
            this.Controls.Add(this.cb_integrated_security);
            this.Controls.Add(this.lb_password);
            this.Controls.Add(this.m_password);
            this.Controls.Add(this.lb_login);
            this.Controls.Add(this.m_login);
            this.Controls.Add(this.lb_table);
            this.Controls.Add(this.m_table);
            this.Controls.Add(this.lb_database);
            this.Controls.Add(this.lb_server);
            this.Controls.Add(this.m_database);
            this.Controls.Add(this.m_server);
            this.Controls.Add(this.m_bottom_panel);
            this.MinimumSize = new System.Drawing.Size(292, 160);
            this.Name = "SqlDbImportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.m_bottom_panel.ResumeLayout(false);
            this.m_bottom_panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.TextBox m_server;
    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.TextBox m_database;
    private System.Windows.Forms.Label lb_server;
    private System.Windows.Forms.Label lb_database;
    private System.Windows.Forms.Label lb_table;
    private System.Windows.Forms.TextBox m_table;
    private System.Windows.Forms.Label lb_login;
    private System.Windows.Forms.TextBox m_login;
    private System.Windows.Forms.Label lb_password;
    private System.Windows.Forms.TextBox m_password;
    private System.Windows.Forms.CheckBox cb_integrated_security;
    private System.Windows.Forms.Label lb_windows_authentication;
  }
}