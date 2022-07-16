namespace Schicksal.Helm.Dialogs
{
  partial class StatisticsParametersDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticsParametersDialog));
            this.m_bottom_panel = new System.Windows.Forms.FlowLayoutPanel();
            this.m_button_cancel = new System.Windows.Forms.Button();
            this.m_button_ok = new System.Windows.Forms.Button();
            this.m_binding_source = new System.Windows.Forms.BindingSource(this.components);
            this.m_table_panel = new System.Windows.Forms.TableLayoutPanel();
            this.m_label_probability = new System.Windows.Forms.Label();
            this.m_list_total = new System.Windows.Forms.ListBox();
            this.totalBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_list_selected = new System.Windows.Forms.ListBox();
            this.predictorsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_label_total = new System.Windows.Forms.Label();
            this.m_label_selected = new System.Windows.Forms.Label();
            this.m_flow_panel = new System.Windows.Forms.FlowLayoutPanel();
            this.m_button_left = new System.Windows.Forms.Button();
            this.m_button_right = new System.Windows.Forms.Button();
            this.m_result_field = new System.Windows.Forms.ComboBox();
            this.calculatableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_label_result = new System.Windows.Forms.Label();
            this.m_label_filter = new System.Windows.Forms.Label();
            this.m_filter_edit = new System.Windows.Forms.TextBox();
            this.m_probability_edit = new System.Windows.Forms.ComboBox();
            this.m_bottom_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).BeginInit();
            this.m_table_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.predictorsBindingSource)).BeginInit();
            this.m_flow_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calculatableBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // m_bottom_panel
            // 
            resources.ApplyResources(this.m_bottom_panel, "m_bottom_panel");
            this.m_bottom_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_bottom_panel.Controls.Add(this.m_button_cancel);
            this.m_bottom_panel.Controls.Add(this.m_button_ok);
            this.m_bottom_panel.Name = "m_bottom_panel";
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
            // m_binding_source
            // 
            this.m_binding_source.DataSource = typeof(Schicksal.Helm.Dialogs.AnovaDialogData);
            // 
            // m_table_panel
            // 
            resources.ApplyResources(this.m_table_panel, "m_table_panel");
            this.m_table_panel.Controls.Add(this.m_label_probability, 0, 4);
            this.m_table_panel.Controls.Add(this.m_list_total, 0, 1);
            this.m_table_panel.Controls.Add(this.m_list_selected, 2, 1);
            this.m_table_panel.Controls.Add(this.m_label_total, 0, 0);
            this.m_table_panel.Controls.Add(this.m_label_selected, 2, 0);
            this.m_table_panel.Controls.Add(this.m_flow_panel, 1, 1);
            this.m_table_panel.Controls.Add(this.m_result_field, 2, 2);
            this.m_table_panel.Controls.Add(this.m_label_result, 0, 2);
            this.m_table_panel.Controls.Add(this.m_label_filter, 0, 3);
            this.m_table_panel.Controls.Add(this.m_filter_edit, 2, 3);
            this.m_table_panel.Controls.Add(this.m_probability_edit, 2, 4);
            this.m_table_panel.Name = "m_table_panel";
            // 
            // m_label_probability
            // 
            resources.ApplyResources(this.m_label_probability, "m_label_probability");
            this.m_table_panel.SetColumnSpan(this.m_label_probability, 2);
            this.m_label_probability.Name = "m_label_probability";
            // 
            // m_list_total
            // 
            this.m_list_total.DataSource = this.totalBindingSource;
            resources.ApplyResources(this.m_list_total, "m_list_total");
            this.m_list_total.FormattingEnabled = true;
            this.m_list_total.Name = "m_list_total";
            this.m_list_total.DoubleClick += new System.EventHandler(this.Button_left_Click);
            // 
            // totalBindingSource
            // 
            this.totalBindingSource.DataMember = "Total";
            this.totalBindingSource.DataSource = this.m_binding_source;
            // 
            // m_list_selected
            // 
            this.m_list_selected.DataSource = this.predictorsBindingSource;
            resources.ApplyResources(this.m_list_selected, "m_list_selected");
            this.m_list_selected.FormattingEnabled = true;
            this.m_list_selected.Name = "m_list_selected";
            this.m_list_selected.DoubleClick += new System.EventHandler(this.Button_right_Click);
            // 
            // predictorsBindingSource
            // 
            this.predictorsBindingSource.DataMember = "Predictors";
            this.predictorsBindingSource.DataSource = this.m_binding_source;
            // 
            // m_label_total
            // 
            resources.ApplyResources(this.m_label_total, "m_label_total");
            this.m_label_total.Name = "m_label_total";
            // 
            // m_label_selected
            // 
            resources.ApplyResources(this.m_label_selected, "m_label_selected");
            this.m_label_selected.Name = "m_label_selected";
            // 
            // m_flow_panel
            // 
            this.m_flow_panel.Controls.Add(this.m_button_left);
            this.m_flow_panel.Controls.Add(this.m_button_right);
            resources.ApplyResources(this.m_flow_panel, "m_flow_panel");
            this.m_flow_panel.Name = "m_flow_panel";
            // 
            // m_button_left
            // 
            resources.ApplyResources(this.m_button_left, "m_button_left");
            this.m_button_left.Image = global::Schicksal.Helm.Properties.Resources.arrow_right_blue;
            this.m_button_left.Name = "m_button_left";
            this.m_button_left.UseVisualStyleBackColor = true;
            this.m_button_left.Click += new System.EventHandler(this.Button_left_Click);
            // 
            // m_button_right
            // 
            resources.ApplyResources(this.m_button_right, "m_button_right");
            this.m_button_right.Image = global::Schicksal.Helm.Properties.Resources.arrow_left_blue;
            this.m_button_right.Name = "m_button_right";
            this.m_button_right.UseVisualStyleBackColor = true;
            this.m_button_right.Click += new System.EventHandler(this.Button_right_Click);
            // 
            // m_result_field
            // 
            resources.ApplyResources(this.m_result_field, "m_result_field");
            this.m_result_field.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.m_binding_source, "Result", true));
            this.m_result_field.DataSource = this.calculatableBindingSource;
            this.m_result_field.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_result_field.FormattingEnabled = true;
            this.m_result_field.Name = "m_result_field";
            // 
            // calculatableBindingSource
            // 
            this.calculatableBindingSource.DataMember = "Calculatable";
            this.calculatableBindingSource.DataSource = this.m_binding_source;
            // 
            // m_label_result
            // 
            resources.ApplyResources(this.m_label_result, "m_label_result");
            this.m_table_panel.SetColumnSpan(this.m_label_result, 2);
            this.m_label_result.Name = "m_label_result";
            // 
            // m_label_filter
            // 
            resources.ApplyResources(this.m_label_filter, "m_label_filter");
            this.m_table_panel.SetColumnSpan(this.m_label_filter, 2);
            this.m_label_filter.Name = "m_label_filter";
            // 
            // m_filter_edit
            // 
            this.m_filter_edit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_binding_source, "Filter", true));
            resources.ApplyResources(this.m_filter_edit, "m_filter_edit");
            this.m_filter_edit.Name = "m_filter_edit";
            // 
            // m_probability_edit
            // 
            resources.ApplyResources(this.m_probability_edit, "m_probability_edit");
            this.m_probability_edit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.m_binding_source, "Probability", true));
            this.m_probability_edit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_probability_edit.FormattingEnabled = true;
            this.m_probability_edit.Name = "m_probability_edit";
            // 
            // StatisticsParametersDialog
            // 
            this.AcceptButton = this.m_button_ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_button_cancel;
            this.Controls.Add(this.m_table_panel);
            this.Controls.Add(this.m_bottom_panel);
            this.Name = "StatisticsParametersDialog";
            this.ShowInTaskbar = false;
            this.m_bottom_panel.ResumeLayout(false);
            this.m_bottom_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_binding_source)).EndInit();
            this.m_table_panel.ResumeLayout(false);
            this.m_table_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.predictorsBindingSource)).EndInit();
            this.m_flow_panel.ResumeLayout(false);
            this.m_flow_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calculatableBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel m_bottom_panel;
    private System.Windows.Forms.Button m_button_cancel;
    private System.Windows.Forms.Button m_button_ok;
    private System.Windows.Forms.BindingSource m_binding_source;
    private System.Windows.Forms.TableLayoutPanel m_table_panel;
    private System.Windows.Forms.ListBox m_list_total;
    private System.Windows.Forms.BindingSource totalBindingSource;
    private System.Windows.Forms.ListBox m_list_selected;
    private System.Windows.Forms.BindingSource predictorsBindingSource;
    private System.Windows.Forms.Label m_label_total;
    private System.Windows.Forms.Label m_label_selected;
    private System.Windows.Forms.FlowLayoutPanel m_flow_panel;
    private System.Windows.Forms.Button m_button_left;
    private System.Windows.Forms.Button m_button_right;
    private System.Windows.Forms.ComboBox m_result_field;
    private System.Windows.Forms.Label m_label_result;
    private System.Windows.Forms.BindingSource calculatableBindingSource;
    private System.Windows.Forms.Label m_label_filter;
    private System.Windows.Forms.TextBox m_filter_edit;
    private System.Windows.Forms.Label m_label_probability;
    private System.Windows.Forms.ComboBox m_probability_edit;
  }
}