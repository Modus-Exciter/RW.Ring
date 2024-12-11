
namespace Schicksal.Helm
{
  partial class FilterCell
  {
    /// <summary> 
    /// Обязательная переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором компонентов

    /// <summary> 
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_text_box = new System.Windows.Forms.TextBox();
      this.m_text_border = new System.Windows.Forms.Panel();
      this.m_worker = new System.ComponentModel.BackgroundWorker();
      this.m_text_border.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_text_box
      // 
      this.m_text_box.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
      this.m_text_box.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
      this.m_text_box.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_text_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.m_text_box.Location = new System.Drawing.Point(2, 2);
      this.m_text_box.Name = "m_text_box";
      this.m_text_box.Size = new System.Drawing.Size(318, 17);
      this.m_text_box.TabIndex = 0;
      this.m_text_box.TextChanged += new System.EventHandler(this.HandleTextChanged);
      this.m_text_box.MouseEnter += new System.EventHandler(this.HandleMouseEnter);
      this.m_text_box.MouseLeave += new System.EventHandler(this.HandleMouseLeave);
      // 
      // m_text_border
      // 
      this.m_text_border.BackColor = System.Drawing.SystemColors.Window;
      this.m_text_border.Controls.Add(this.m_text_box);
      this.m_text_border.Dock = System.Windows.Forms.DockStyle.Fill;
      this.m_text_border.Location = new System.Drawing.Point(1, 1);
      this.m_text_border.Name = "m_text_border";
      this.m_text_border.Padding = new System.Windows.Forms.Padding(2);
      this.m_text_border.Size = new System.Drawing.Size(322, 125);
      this.m_text_border.TabIndex = 1;
      // 
      // m_worker
      // 
      this.m_worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.HandleDoWork);
      this.m_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.HandleRunWorkerCompleted);
      // 
      // FilterCell
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ControlDark;
      this.Controls.Add(this.m_text_border);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "FilterCell";
      this.Padding = new System.Windows.Forms.Padding(1);
      this.Size = new System.Drawing.Size(324, 127);
      this.Resize += new System.EventHandler(this.HandleSizeChanged);
      this.m_text_border.ResumeLayout(false);
      this.m_text_border.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox m_text_box;
    private System.Windows.Forms.Panel m_text_border;
    private System.ComponentModel.BackgroundWorker m_worker;
  }
}
