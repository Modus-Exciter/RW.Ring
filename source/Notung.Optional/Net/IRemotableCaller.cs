using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Net
{
  /// <summary>
  /// Абстракция для выполнеия команды удалённо
  /// </summary>
  public interface IRemotableCaller
  {
    /// <summary>
    /// Выполнение команды, потенциально удалённой
    /// </summary>
    /// <param name="command">Выполняемая команда</param>
    /// <returns>Результат выполнения командыы</returns>
    RemotableResult1 Call(IRemotableCommand command);
  }
}
