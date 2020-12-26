using System;

using System.Data;

namespace Schicksal.Exchange
{
  public interface ITableImport
  {
    ImportResult Import();
  }

  public sealed class ImportResult
  {
    public DataTable Table { get; set; }

    public string Description { get; set; }
  }
}
