using Schicksal.Exchange;
using SqlDbImport.Properties;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SqlDbImport
{
  public class ImportSqlDb : MarshalByRefObject, ITableImport
  {
    public ImportResult Import(object context)
    {
      using (var dlg = new SqlDbImportForm())
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {

          var connectionString = string.Empty;
          if(dlg.IntegratedSecurity)
          {
            connectionString = $"Persist Security Info = False; Integrated Security = true; Initial Catalog = {dlg.DatabaseName}; Server = {dlg.ServerPath}";
          }
          else
          {
            connectionString = $"Persist Security Info = False; User ID={dlg.Login};Password={dlg.Password}; Initial Catalog = {dlg.DatabaseName}; Server = {dlg.ServerPath}";
          }
          using (SqlConnection connection = new SqlConnection(connectionString))
          {
            DataTable table = new DataTable();
            SqlCommand command = new SqlCommand($"SELECT * FROM {dlg.TableName};", connection);
            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(table);
            connection.Close();
            adapter.Dispose();

            return new ImportResult
            {
              Table = table,
              Description = string.Format(Resources.IMPORTED_DESCR, dlg.TableName)
            };
          }
        }
        return null;
      }
    }
    public override string ToString()
    {
      return Resources.DB_IMPORT;
    }
  }
}