
namespace Schicksal.Helm
{
  partial class ClusteringResults
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClusteringResults));
            this.m_split_container = new System.Windows.Forms.SplitContainer();
            this.m_data_grid_view_clusters = new System.Windows.Forms.DataGridView();
            this.m_data_grid_view_points = new System.Windows.Forms.DataGridView();
            this.m_tab_control = new System.Windows.Forms.TabControl();
            this.m_table_tab = new System.Windows.Forms.TabPage();
            this.m_tab_graphic = new System.Windows.Forms.TabPage();
            this.m_pictureBox = new System.Windows.Forms.PictureBox();
            this.m_tool_strip = new System.Windows.Forms.ToolStrip();
            this.m_toolStripLabel_abscissa = new System.Windows.Forms.ToolStripLabel();
            this.m_ComboBox_abscissa = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_toolStripLabel_ordinate = new System.Windows.Forms.ToolStripLabel();
            this.m_ComboBox_ordinate = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_toolStripButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.m_split_container)).BeginInit();
            this.m_split_container.Panel1.SuspendLayout();
            this.m_split_container.Panel2.SuspendLayout();
            this.m_split_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_data_grid_view_clusters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_data_grid_view_points)).BeginInit();
            this.m_tab_control.SuspendLayout();
            this.m_table_tab.SuspendLayout();
            this.m_tab_graphic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBox)).BeginInit();
            this.m_tool_strip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_split_container
            // 
            resources.ApplyResources(this.m_split_container, "m_split_container");
            this.m_split_container.Name = "m_split_container";
            // 
            // m_split_container.Panel1
            // 
            this.m_split_container.Panel1.Controls.Add(this.m_data_grid_view_clusters);
            // 
            // m_split_container.Panel2
            // 
            this.m_split_container.Panel2.Controls.Add(this.m_data_grid_view_points);
            // 
            // m_data_grid_view_clusters
            // 
            this.m_data_grid_view_clusters.AllowUserToAddRows = false;
            this.m_data_grid_view_clusters.AllowUserToDeleteRows = false;
            this.m_data_grid_view_clusters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.m_data_grid_view_clusters, "m_data_grid_view_clusters");
            this.m_data_grid_view_clusters.Name = "m_data_grid_view_clusters";
            this.m_data_grid_view_clusters.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_data_grid_view_clusters_RowEnter);
            // 
            // m_data_grid_view_points
            // 
            this.m_data_grid_view_points.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.m_data_grid_view_points, "m_data_grid_view_points");
            this.m_data_grid_view_points.Name = "m_data_grid_view_points";
            // 
            // m_tab_control
            // 
            this.m_tab_control.Controls.Add(this.m_table_tab);
            this.m_tab_control.Controls.Add(this.m_tab_graphic);
            resources.ApplyResources(this.m_tab_control, "m_tab_control");
            this.m_tab_control.Name = "m_tab_control";
            this.m_tab_control.SelectedIndex = 0;
            // 
            // m_table_tab
            // 
            this.m_table_tab.Controls.Add(this.m_split_container);
            resources.ApplyResources(this.m_table_tab, "m_table_tab");
            this.m_table_tab.Name = "m_table_tab";
            this.m_table_tab.UseVisualStyleBackColor = true;
            // 
            // m_tab_graphic
            // 
            this.m_tab_graphic.Controls.Add(this.m_pictureBox);
            this.m_tab_graphic.Controls.Add(this.m_tool_strip);
            resources.ApplyResources(this.m_tab_graphic, "m_tab_graphic");
            this.m_tab_graphic.Name = "m_tab_graphic";
            this.m_tab_graphic.UseVisualStyleBackColor = true;
            // 
            // m_pictureBox
            // 
            this.m_pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.m_pictureBox, "m_pictureBox");
            this.m_pictureBox.Name = "m_pictureBox";
            this.m_pictureBox.TabStop = false;
            // 
            // m_tool_strip
            // 
            this.m_tool_strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_toolStripLabel_abscissa,
            this.m_ComboBox_abscissa,
            this.toolStripSeparator1,
            this.m_toolStripLabel_ordinate,
            this.m_ComboBox_ordinate,
            this.toolStripSeparator2,
            this.m_toolStripButton});
            resources.ApplyResources(this.m_tool_strip, "m_tool_strip");
            this.m_tool_strip.Name = "m_tool_strip";
            // 
            // m_toolStripLabel_abscissa
            // 
            this.m_toolStripLabel_abscissa.Name = "m_toolStripLabel_abscissa";
            resources.ApplyResources(this.m_toolStripLabel_abscissa, "m_toolStripLabel_abscissa");
            // 
            // m_ComboBox_abscissa
            // 
            this.m_ComboBox_abscissa.Name = "m_ComboBox_abscissa";
            resources.ApplyResources(this.m_ComboBox_abscissa, "m_ComboBox_abscissa");
            this.m_ComboBox_abscissa.SelectedIndexChanged += new System.EventHandler(this.m_ComboBox_abscissa_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // m_toolStripLabel_ordinate
            // 
            this.m_toolStripLabel_ordinate.Name = "m_toolStripLabel_ordinate";
            resources.ApplyResources(this.m_toolStripLabel_ordinate, "m_toolStripLabel_ordinate");
            // 
            // m_ComboBox_ordinate
            // 
            this.m_ComboBox_ordinate.Name = "m_ComboBox_ordinate";
            resources.ApplyResources(this.m_ComboBox_ordinate, "m_ComboBox_ordinate");
            this.m_ComboBox_ordinate.SelectedIndexChanged += new System.EventHandler(this.m_ComboBox_ordinate_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // m_toolStripButton
            // 
            this.m_toolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.m_toolStripButton, "m_toolStripButton");
            this.m_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.m_toolStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 30, 2);
            this.m_toolStripButton.Name = "m_toolStripButton";
            this.m_toolStripButton.Click += new System.EventHandler(this.m_toolStripButton_Click);
            // 
            // ClusteringResults
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_tab_control);
            this.Name = "ClusteringResults";
            this.Resize += new System.EventHandler(this.ClusteringResults_Resize);
            this.m_split_container.Panel1.ResumeLayout(false);
            this.m_split_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_split_container)).EndInit();
            this.m_split_container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_data_grid_view_clusters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_data_grid_view_points)).EndInit();
            this.m_tab_control.ResumeLayout(false);
            this.m_table_tab.ResumeLayout(false);
            this.m_tab_graphic.ResumeLayout(false);
            this.m_tab_graphic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBox)).EndInit();
            this.m_tool_strip.ResumeLayout(false);
            this.m_tool_strip.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl m_tab_control;
    private System.Windows.Forms.TabPage m_table_tab;
    private System.Windows.Forms.TabPage m_tab_graphic;
    private System.Windows.Forms.SplitContainer m_split_container;
    private System.Windows.Forms.DataGridView m_data_grid_view_clusters;
    private System.Windows.Forms.DataGridView m_data_grid_view_points;
    private System.Windows.Forms.ToolStrip m_tool_strip;
    private System.Windows.Forms.ToolStripLabel m_toolStripLabel_abscissa;
    private System.Windows.Forms.ToolStripComboBox m_ComboBox_abscissa;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripLabel m_toolStripLabel_ordinate;
    private System.Windows.Forms.ToolStripComboBox m_ComboBox_ordinate;
    private System.Windows.Forms.PictureBox m_pictureBox;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton m_toolStripButton;
  }
}