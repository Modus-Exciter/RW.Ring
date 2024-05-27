
namespace Schicksal.Helm.Dialogs
{
  partial class ClusteringParametersDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClusteringParametersDialog));
            this.m_layout_panel = new System.Windows.Forms.TableLayoutPanel();
            this.m_label_selected_columns = new System.Windows.Forms.Label();
            this.m_grid_selected_columns = new System.Windows.Forms.DataGridView();
            this.columnWeightsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
            this.m_combo_metrics = new System.Windows.Forms.ComboBox();
            this.m_combo_arc_deleter = new System.Windows.Forms.ComboBox();
            this.m_label_metrics = new System.Windows.Forms.Label();
            this.m_button_panel = new System.Windows.Forms.FlowLayoutPanel();
            this.m_button_cancel = new System.Windows.Forms.Button();
            this.m_button_ok = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.m_label_cluster_count = new System.Windows.Forms.Label();
            this.m_numeric_cluster_count = new System.Windows.Forms.NumericUpDown();
            this.columnDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.weightDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_layout_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_grid_selected_columns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnWeightsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
            this.m_button_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numeric_cluster_count)).BeginInit();
            this.SuspendLayout();
            // 
            // m_layout_panel
            // 
            resources.ApplyResources(this.m_layout_panel, "m_layout_panel");
            this.m_layout_panel.Controls.Add(this.m_label_selected_columns, 0, 0);
            this.m_layout_panel.Controls.Add(this.m_grid_selected_columns, 1, 1);
            this.m_layout_panel.Controls.Add(this.m_combo_metrics, 1, 6);
            this.m_layout_panel.Controls.Add(this.m_combo_arc_deleter, 1, 7);
            this.m_layout_panel.Controls.Add(this.m_label_metrics, 0, 6);
            this.m_layout_panel.Controls.Add(this.m_button_panel, 0, 8);
            this.m_layout_panel.Controls.Add(this.label1, 0, 7);
            this.m_layout_panel.Controls.Add(this.m_label_cluster_count, 0, 5);
            this.m_layout_panel.Controls.Add(this.m_numeric_cluster_count, 1, 5);
            this.m_layout_panel.Name = "m_layout_panel";
            // 
            // m_label_selected_columns
            // 
            resources.ApplyResources(this.m_label_selected_columns, "m_label_selected_columns");
            this.m_layout_panel.SetColumnSpan(this.m_label_selected_columns, 2);
            this.m_label_selected_columns.Name = "m_label_selected_columns";
            // 
            // m_grid_selected_columns
            // 
            this.m_grid_selected_columns.AutoGenerateColumns = false;
            this.m_grid_selected_columns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_grid_selected_columns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnDataGridViewTextBoxColumn,
            this.weightDataGridViewTextBoxColumn});
            this.m_grid_selected_columns.DataSource = this.columnWeightsBindingSource;
            resources.ApplyResources(this.m_grid_selected_columns, "m_grid_selected_columns");
            this.m_grid_selected_columns.Name = "m_grid_selected_columns";
            this.m_layout_panel.SetRowSpan(this.m_grid_selected_columns, 4);
            this.m_grid_selected_columns.RowTemplate.Height = 24;
            this.m_grid_selected_columns.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_grid_selected_columns_CellEndEdit);
            // 
            // columnWeightsBindingSource
            // 
            this.columnWeightsBindingSource.DataMember = "ColumnWeights";
            this.columnWeightsBindingSource.DataSource = this.m_binding_source;
            // 
            // m_binding_source
            // 
            this.m_binding_source.DataSource = typeof(Schicksal.Clustering.ClusteringParameters);
            // 
            // m_combo_metrics
            // 
            this.m_combo_metrics.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.m_binding_source, "DistanceMetrics", true));
            resources.ApplyResources(this.m_combo_metrics, "m_combo_metrics");
            this.m_combo_metrics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_combo_metrics.FormattingEnabled = true;
            this.m_combo_metrics.Name = "m_combo_metrics";
            this.m_combo_metrics.SelectedIndexChanged += new System.EventHandler(this.m_combo_metrics_SelectedIndexChanged);
            // 
            // m_combo_arc_deleter
            // 
            this.m_combo_arc_deleter.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.m_binding_source, "ArcDeleter", true));
            resources.ApplyResources(this.m_combo_arc_deleter, "m_combo_arc_deleter");
            this.m_combo_arc_deleter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_combo_arc_deleter.FormattingEnabled = true;
            this.m_combo_arc_deleter.Name = "m_combo_arc_deleter";
            this.m_combo_arc_deleter.SelectedIndexChanged += new System.EventHandler(this.m_combo_arc_deleter_SelectedIndexChanged);
            // 
            // m_label_metrics
            // 
            resources.ApplyResources(this.m_label_metrics, "m_label_metrics");
            this.m_label_metrics.Name = "m_label_metrics";
            // 
            // m_button_panel
            // 
            resources.ApplyResources(this.m_button_panel, "m_button_panel");
            this.m_button_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_layout_panel.SetColumnSpan(this.m_button_panel, 2);
            this.m_button_panel.Controls.Add(this.m_button_cancel);
            this.m_button_panel.Controls.Add(this.m_button_ok);
            this.m_button_panel.Name = "m_button_panel";
            // 
            // m_button_cancel
            // 
            resources.ApplyResources(this.m_button_cancel, "m_button_cancel");
            this.m_button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_button_cancel.Name = "m_button_cancel";
            this.m_button_cancel.UseVisualStyleBackColor = true;
            // 
            // m_button_ok
            // 
            resources.ApplyResources(this.m_button_ok, "m_button_ok");
            this.m_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_button_ok.Name = "m_button_ok";
            this.m_button_ok.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_label_cluster_count
            // 
            resources.ApplyResources(this.m_label_cluster_count, "m_label_cluster_count");
            this.m_label_cluster_count.Name = "m_label_cluster_count";
            // 
            // m_numeric_cluster_count
            // 
            resources.ApplyResources(this.m_numeric_cluster_count, "m_numeric_cluster_count");
            this.m_numeric_cluster_count.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.m_numeric_cluster_count.Name = "m_numeric_cluster_count";
            this.m_numeric_cluster_count.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.m_numeric_cluster_count.ValueChanged += new System.EventHandler(this.m_numeric_cluster_count_ValueChanged);
            // 
            // columnDataGridViewTextBoxColumn
            // 
            this.columnDataGridViewTextBoxColumn.DataPropertyName = "Column";
            resources.ApplyResources(this.columnDataGridViewTextBoxColumn, "columnDataGridViewTextBoxColumn");
            this.columnDataGridViewTextBoxColumn.Name = "columnDataGridViewTextBoxColumn";
            this.columnDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // weightDataGridViewTextBoxColumn
            // 
            this.weightDataGridViewTextBoxColumn.DataPropertyName = "Weight";
            resources.ApplyResources(this.weightDataGridViewTextBoxColumn, "weightDataGridViewTextBoxColumn");
            this.weightDataGridViewTextBoxColumn.Name = "weightDataGridViewTextBoxColumn";
            // 
            // ClusteringParametersDialog
            // 
            this.AcceptButton = this.m_button_ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_button_cancel;
            this.Controls.Add(this.m_layout_panel);
            this.Name = "ClusteringParametersDialog";
            this.m_layout_panel.ResumeLayout(false);
            this.m_layout_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_grid_selected_columns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnWeightsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
            this.m_button_panel.ResumeLayout(false);
            this.m_button_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numeric_cluster_count)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel m_layout_panel;
    private System.Windows.Forms.Label m_label_selected_columns;
    private System.Windows.Forms.DataGridView m_grid_selected_columns;
    private System.Windows.Forms.ComboBox m_combo_metrics;
    private System.Windows.Forms.ComboBox m_combo_arc_deleter;
    private System.Windows.Forms.Label m_label_metrics;
    private System.Windows.Forms.FlowLayoutPanel m_button_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.BindingSource columnWeightsBindingSource;
    private System.Windows.Forms.BindingSource m_binding_source;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label m_label_cluster_count;
    private System.Windows.Forms.NumericUpDown m_numeric_cluster_count;
    private System.Windows.Forms.DataGridViewTextBoxColumn columnDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn weightDataGridViewTextBoxColumn;
  }
}