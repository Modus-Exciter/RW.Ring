using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung
{
  public interface IValidator
  {
    bool Validate(InfoBuffer buffer);
  }
  
  public sealed class Info
  {
  }

  public sealed class InfoBuffer
  {
  }
}
