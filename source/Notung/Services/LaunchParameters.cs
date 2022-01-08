using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using Notung.ComponentModel;

namespace Notung.Services
{
  /// <summary>
  /// Настройки отображения задачи с индикатором прогресса
  /// </summary>
  [Serializable]
  public sealed class LaunchParameters
  {
    [NonSerialized]
    private Image m_bitmap;
    private byte[] m_image_data;

    /// <summary>
    /// Инициализация нового набора настроек отображения задачи с индикатором прогресса
    /// </summary>
    public LaunchParameters()
    {
      this.CloseOnFinish = true;
    }

    [OnSerializing]
    void OnSerializing(StreamingContext context)
    {
      if (m_bitmap != null)
      {
        using (var ms = new MemoryStream())
        {
          m_bitmap.Save(ms, ImageFormat.Png);
          m_image_data = ms.ToArray();
        }
      }
    }

    [OnDeserialized]
    void OnDeserialized(StreamingContext context)
    {
      if (m_image_data != null)
      {
        using (var ms = new MemoryStream(m_image_data))
        {
          m_bitmap = (Bitmap)Image.FromStream(ms);
        }
      }
    }

    /// <summary>
    /// Заголовок задачи
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Картинка, отображающаяся при выполнении задачи
    /// </summary>
    public Image Bitmap
    {
      get { return m_bitmap; }
      set
      {
        m_bitmap = value;
        m_image_data = null;
      }
    }

    /// <summary>
    /// Закрывать ли диалог с прогрессом выполнения после завершения задачи
    /// </summary>
    public bool CloseOnFinish { get; set; }

    public override string ToString()
    {
      return !string.IsNullOrWhiteSpace(this.Caption) ? this.Caption : base.ToString();
    }

    internal IDisposableRunBase Wrap(IRunBase runBase)
    {
      if (runBase == null)
        throw new ArgumentNullException("runBase");

      if (runBase is ICancelableRunBase)
        return new CancelableLaunchParametersWrapper((ICancelableRunBase)runBase, this);
      else
        return new LaunchParametersWrapper(runBase, this);
    }

    internal interface IDisposableRunBase : IRunBase, IDisposable { }

    #region Implementation ------------------------------------------------------------------------

    private class LaunchParametersWrapper : IDisposableRunBase, IServiceProvider
    {
      protected readonly IRunBase m_run_base;
      private readonly LaunchParameters m_parameters;
      private readonly ProgressChangedEventHandler m_handler;
      private Image m_image;

      public LaunchParametersWrapper(IRunBase runBase, LaunchParameters parameters)
      {
        m_run_base = runBase;
        m_parameters = parameters;
        m_handler = this.HandleProgressChanged;

        if (m_run_base is IServiceProvider)
          m_run_base.ProgressChanged += m_handler;
      }

      public void Run()
      {
        m_run_base.Run();
      }

      private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
      {
        if (e.UserState is LaunchParametersChange)
        {
          var change = (LaunchParametersChange)e.UserState;

          if ((change & LaunchParametersChange.Image) != 0)
            m_image = ((IServiceProvider)m_run_base).GetService<Image>();
        }
      }

      public event ProgressChangedEventHandler ProgressChanged
      {
        add { m_run_base.ProgressChanged += value; }
        remove { m_run_base.ProgressChanged -= value; }
      }

      public override string ToString()
      {
        var caption = m_parameters.Caption;

        if (!string.IsNullOrWhiteSpace(caption))
          return caption;
        else
          return RunBase.GetDefaultCaption(m_run_base);
      }

      public object GetService(Type serviceType)
      {
        if (serviceType == typeof(Image))
        {
          if (m_image != null)
            return m_image;

          if (m_parameters.Bitmap != null)
            return m_parameters.Bitmap;
        }
        else if (serviceType == typeof(LaunchParameters))
          return m_parameters;

        var provider = m_run_base as IServiceProvider;

        if (provider != null)
          return provider.GetService(serviceType);
        else
          return null;
      }

      public void Dispose()
      {
        if (m_run_base is IServiceProvider)
          m_run_base.ProgressChanged -= m_handler;
      }
    }

    private class CancelableLaunchParametersWrapper : LaunchParametersWrapper, ICancelableRunBase
    {
      public CancelableLaunchParametersWrapper(ICancelableRunBase runBase, LaunchParameters parameters)
        : base(runBase, parameters) { }

      public CancellationToken CancellationToken
      {
        get { return ((ICancelableRunBase)m_run_base).CancellationToken; }
        set { ((ICancelableRunBase)m_run_base).CancellationToken = value; }
      }

      public bool CanCancel
      {
        get { return ((ICancelableRunBase)m_run_base).CanCancel; }
      }

      public event EventHandler CanCancelChanged
      {
        add { ((ICancelableRunBase)m_run_base).CanCancelChanged += value; }
        remove { ((ICancelableRunBase)m_run_base).CanCancelChanged -= value; }
      }
    }

    #endregion
  }
}