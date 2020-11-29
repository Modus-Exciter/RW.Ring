using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Notung.Helm.Properties;
using Notung.Loader;
using Notung.Threading;

namespace Notung.Helm.Controls
{
  /// <summary>
  /// Элемент управления, который автоматически может замениться 
  /// другим элементом управления, имеющим зависимости
  /// </summary>
  public sealed class ControlPlaceholder : UserControl, IApplicationLoader
  {
    private readonly object m_lock = new object();

    private string m_replacing_type;
    private volatile bool m_loading;
    private IApplicationLoader m_inner_loader;
    private readonly IOperationWrapper m_op_wrapper;

    public ControlPlaceholder()
    {
      m_op_wrapper = new SynchronizeOperationWrapper(this);
    }

    [Category("Contract")]
    public event EventHandler<LoadingCompletedEventArgs> LoadingCompleted;

    [Category("Contract")]
    [TypeConverter(typeof(ControlTypeTypeConverter))]
    public string ReplacingType
    {
      get { return m_replacing_type; }
      set
      {
        lock (m_lock)
        {
          m_replacing_type = value;
          m_inner_loader = null;
        }
      }
    }

    [Category("Contract")]
    [DefaultValue(null)]
    public string ControlDescription { get; set; }

    public TControl GetControl<TControl>() where TControl : Control
    {
      if (this.Controls.Count != 1)
        return null;

      return (TControl)this.Controls[0];
    }

    #region IApplicationLoader members ------------------------------------------------------------

    bool IApplicationLoader.Load(LoadingContext context)
    {
      if (!string.IsNullOrEmpty(this.ControlDescription))
        context.Indicator.ReportProgress(ProgressPercentage.Unknown, this.ControlDescription);

      var loader = this.GetCurrentLoader();

      if (loader == null)
        return false;

      if (!loader.Load(context))
        return false;

      var control = m_op_wrapper.Invoke(GetControl(loader, context));

      if (control == null)
        return false;

      m_op_wrapper.Invoke(() => ReplaceControls(control));

      var ctrl_loader = control as IApplicationLoader;

      if (ctrl_loader != null && !ctrl_loader.Load(context))
        return false;

      m_op_wrapper.Invoke(() =>
      {
        if (this.LoadingCompleted != null)
          this.LoadingCompleted(this, new LoadingCompletedEventArgs(context));
      });

      return true;
    }

    void IApplicationLoader.Setup(LoadingContext context)
    {
      var loader = this.GetCurrentLoader();

      if (loader != null)
      {
        loader.Setup(context);

        var control = context.Container.GetService(loader.Key) as IApplicationLoader;

        if (control != null)
          control.Setup(context);
      }
    }

    #endregion

    #region IDependencyItem<Type> members ---------------------------------------------------------

    Type IDependencyItem<Type>.Key
    {
      get { return this.GetLoaderProperty(l => l.Key); }
    }

    ICollection<Type> IDependencyItem<Type>.Dependencies
    {
      get { return this.GetLoaderProperty(l => l.Dependencies); }
    }

    #endregion

    #region Implementation methods

    protected override void OnControlAdded(ControlEventArgs e)
    {
      if (!m_loading)
        throw new InvalidOperationException("m_loading");

      base.OnControlAdded(e);
    }

    protected override void OnControlRemoved(ControlEventArgs e)
    {
      if (!m_loading)
        throw new InvalidOperationException("m_loading");

      base.OnControlRemoved(e);
    }

    private void ReplaceControls(Control control)
    {
      m_loading = true;

      try
      {
        var diposee = this.Controls.Cast<Control>().ToArray();

        this.SuspendLayout();
        this.Controls.Clear();

        foreach (var ctrl in diposee)
        {
          if (!ReferenceEquals(ctrl, control))
            ctrl.Dispose();
        }

        control.Dock = DockStyle.Fill;
        this.Controls.Add(control);
        this.ResumeLayout();
      }
      finally
      {
        m_loading = false;
      }
    }

    private static Func<Control> GetControl(IApplicationLoader loader, LoadingContext context)
    {
      return () => context.Container.GetService(loader.Key) as Control;
    }

    private IApplicationLoader GetCurrentLoader()
    {
      lock (m_lock)
      {
        if (m_inner_loader == null && !string.IsNullOrEmpty(m_replacing_type))
        {
          var control_type = Type.GetType(m_replacing_type, false);

          if (control_type != null)
          {
            var loader_type = typeof(ControlPlaceholderApplicationLoader<,>)
              .MakeGenericType(
              control_type.GetCustomAttribute<ContractPlaceholderAttribute>(false).ContractType,
              control_type);

            m_inner_loader = (IApplicationLoader)Activator.CreateInstance(loader_type);
          }
        }

        return m_inner_loader;
      }
    }

    private TResult GetLoaderProperty<TResult>(Func<IApplicationLoader, TResult> func)
      where TResult : class
    {
      var loader = this.GetCurrentLoader();

      if (loader != null)
        return func(loader);
      else
        return null;
    }

    #endregion

    #region Implementation classes ----------------------------------------------------------------

    private class ControlPlaceholderApplicationLoader<TContract, TService>
      : ApplicationLoader<TContract, TService>
      where TService : TContract
      where TContract : class
    {
      protected override bool FilterProperty(PropertyInfo property)
      {
        return property.IsDefined(typeof(ContractDependencyAttribute), false);
      }
    }

    private class ControlTypeTypeConverter : TypeConverter
    {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return true;
      }

      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
        if (sourceType == typeof(Type))
          return true;

        return base.CanConvertFrom(context, sourceType);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
        if (destinationType == typeof(string))
          return true;

        return base.CanConvertTo(context, destinationType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
      {
        if (value is Type)
          return GetTypeString(value);

        return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value, Type destinationType)
      {
        if (value is Type && destinationType == typeof(string))
          return GetTypeString(value);

        return base.ConvertTo(context, culture, value, destinationType);
      }

      private static string GetTypeString(object value)
      {
        var t = (Type)value;

        return string.Format("{0}, {1}", t.FullName, t.Assembly.GetName().Name);
      }

      public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        var ret = new List<string>();
        foreach (var asm in AppManager.AssemblyClassifier.TrackingAssemblies)
        {
          try
          {
            foreach (var type in asm.GetAvailableTypes())
            {
              if (!typeof(Control).IsAssignableFrom(type))
                continue;

              var contract = type.GetCustomAttribute<ContractPlaceholderAttribute>(false);

              if (contract == null || !contract.ContractType.IsAssignableFrom(type))
                continue;

              ret.Add(GetTypeString(type));
            }
          }
          catch { }
        }

        return new StandardValuesCollection(ret);
      }
    }

  #endregion
  }

  /// <summary>
  /// Задаёт для элемента управления контракт 
  /// для отложенной инициализации с помощью <see cref="ControlPlaceholder"/>
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class ContractPlaceholderAttribute : Attribute
  {
    private readonly Type m_contract_type;

    /// <summary>
    /// Задаёт для элемента управления контракт 
    /// для отложенной инициализации с помощью <see cref="ControlPlaceholder"/>
    /// </summary>
    /// <param name="contractType">Тип контракта, реализуемый элементом управления</param>
    public ContractPlaceholderAttribute(Type contractType)
    {
      if (contractType == null)
        throw new ArgumentNullException("contractType");

      if (!contractType.IsInterface)
        throw new ArgumentException(Resources.CONTRACT_NOT_INTERFACE, "contractType");

      m_contract_type = contractType;
    }

    /// <summary>
    /// Тип контракта, реализуемый элементом управления
    /// </summary>
    public Type ContractType
    {
      get { return m_contract_type; }
    }
  }

  /// <summary>
  /// Помечает свойство как зависимость при отложенной инициализации
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public sealed class ContractDependencyAttribute : Attribute { }

  public class LoadingCompletedEventArgs : EventArgs
  {
    private readonly LoadingContext m_context;

    public LoadingCompletedEventArgs(LoadingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");

      m_context = context;
    }

    public LoadingContext Context
    {
      get { return m_context; }
    }
  }
}