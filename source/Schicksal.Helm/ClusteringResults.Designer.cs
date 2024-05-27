
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
            this.m_tab_control = new System.Windows.Forms.TabControl();
            this.m_table_tab = new System.Windows.Forms.TabPage();
            this.m_split_container = new System.Windows.Forms.SplitContainer();
            this.m_data_grid_view_clusters = new System.Windows.Forms.DataGridView();
            this.m_data_grid_view_points = new System.Windows.Forms.DataGridView();
            this.m_tab_graphic = new System.Windows.Forms.TabPage();
            this.m_tab_control.SuspendLayout();
            this.m_table_tab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_split_container)).BeginInit();
            this.m_split_container.Panel1.SuspendLayout();
            this.m_split_container.Panel2.SuspendLayout();
            this.m_split_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_data_grid_view_clusters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_data_grid_view_points)).BeginInit();
            this.SuspendLayout();
            // 
            // m_tab_control
            // 
            this.m_tab_control.Controls.Add(this.m_table_tab);
            this.m_tab_control.Controls.Add(this.m_tab_graphic);
            this.m_tab_control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tab_control.Location = new System.Drawing.Point(0, 0);
            this.m_tab_control.Name = "m_tab_control";
            this.m_tab_control.SelectedIndex = 0;
            this.m_tab_control.Size = new System.Drawing.Size(800, 450);
            this.m_tab_control.TabIndex = 0;
            // 
            // m_table_tab
            // 
            this.m_table_tab.Controls.Add(this.m_split_container);
            this.m_table_tab.Location = new System.Drawing.Point(4, 22);
            this.m_table_tab.Name = "m_table_tab";
            this.m_table_tab.Padding = new System.Windows.Forms.Padding(3);
            this.m_table_tab.Size = new System.Drawing.Size(792, 424);
            this.m_table_tab.TabIndex = 0;
            this.m_table_tab.Text = "Table";
            this.m_table_tab.UseVisualStyleBackColor = true;
            // 
            // m_split_container
            // 
            this.m_split_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_split_container.Location = new System.Drawing.Point(3, 3);
            this.m_split_container.Name = "m_split_container";
            this.m_split_container.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // m_split_container.Panel1
            // 
            this.m_split_container.Panel1.Controls.Add(this.m_data_grid_view_clusters);
            // 
            // m_split_container.Panel2
            // 
            this.m_split_container.Panel2.Controls.Add(this.m_data_grid_view_points);
            this.m_split_container.Size = new System.Drawing.Size(786, 418);
            this.m_split_container.SplitterDistance = 212;
            this.m_split_container.TabIndex = 0;
            // 
            // m_data_grid_view_clusters
            // 
            this.m_data_grid_view_clusters.AllowUserToAddRows = false;
            this.m_data_grid_view_clusters.AllowUserToDeleteRows = false;
            this.m_data_grid_view_clusters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_data_grid_view_clusters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_data_grid_view_clusters.Location = new System.Drawing.Point(0, 0);
            this.m_data_grid_view_clusters.Name = "m_data_grid_view_clusters";
            this.m_data_grid_view_clusters.Size = new System.Drawing.Size(786, 212);
            this.m_data_grid_view_clusters.TabIndex = 0;
            this.m_data_grid_view_clusters.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_data_grid_view_clusters_RowEnter);
            // 
            // m_data_grid_view_points
            // 
            this.m_data_grid_view_points.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_data_grid_view_points.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_data_grid_view_points.Location = new System.Drawing.Point(0, 0);
            this.m_data_grid_view_points.Name = "m_data_grid_view_points";
            this.m_data_grid_view_points.Size = new System.Drawing.Size(786, 202);
            this.m_data_grid_view_points.TabIndex = 0;
            // 
            // m_tab_graphic
            // 
            this.m_tab_graphic.Location = new System.Drawing.Point(4, 22);
            this.m_tab_graphic.Name = "m_tab_graphic";
            this.m_tab_graphic.Padding = new System.Windows.Forms.Padding(3);
            this.m_tab_graphic.Size = new System.Drawing.Size(792, 424);
            this.m_tab_graphic.TabIndex = 1;
            this.m_tab_graphic.Text = "Graphic";
            this.m_tab_graphic.UseVisualStyleBackColor = true;
            // 
            // ClusteringResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.m_tab_control);
            this.Name = "ClusteringResults";
            this.Text = "Clustering result";
            this.m_tab_control.ResumeLayout(false);
            this.m_table_tab.ResumeLayout(false);
            this.m_split_container.Panel1.ResumeLayout(false);
            this.m_split_container.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_split_container)).EndInit();
            this.m_split_container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_data_grid_view_clusters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_data_grid_view_points)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl m_tab_control;
    private System.Windows.Forms.TabPage m_table_tab;
    private System.Windows.Forms.TabPage m_tab_graphic;
    private System.Windows.Forms.SplitContainer m_split_container;
    private System.Windows.Forms.DataGridView m_data_grid_view_clusters;
    private System.Windows.Forms.DataGridView m_data_grid_view_points;
  }
}