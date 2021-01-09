using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Notung.Net
{
  /// <summary>
  /// Сведения о клиенте, который обслуживается в текущий момент
  /// </summary>
  [Serializable, DataContract(Namespace = "")]
  public sealed class ClientInfo
  {
    private static readonly ClientInfo _process = new ClientInfo
    {
      Application = Global.MainAssembly.GetName().Name,
      MachineName = Environment.MachineName,
      UserName = Environment.UserName
    };

    [ThreadStatic]
    private static ClientInfo _thread;
    
    /// <summary>
    /// Имя пользователя, запустившего команду
    /// </summary>
    [DataMember(Name = "User")]
    public string UserName { get; internal set; }

    /// <summary>
    /// Приложение, через которое запущена команда
    /// </summary>
    [DataMember(Name = "App")]
    public string Application { get; internal set; }

    /// <summary>
    /// Имя компьютера, с которого запущена команда
    /// </summary>
    [DataMember(Name = "Machine")]
    public string MachineName { get; internal set; }

    /// <summary>
    /// Сведения о текущем процессе как о клиенте
    /// </summary>
    public static ClientInfo ProcessInfo
    {
      get { return _process; }
    }

    /// <summary>
    /// Сведения о клиенте, который обслуживается текущим потоком
    /// </summary>
    public static ClientInfo ThreadInfo
    {
      get { return _thread; }
      internal set { _thread = value; }
    }
  }
}
