using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegressionTest
{
  internal class ExcelConnection : IDisposable
  {
    readonly Workbook m_workbook;
    readonly Worksheet m_worksheet;
    bool m_is_disposed = false;

    public ExcelConnection(string filePath, int sheetIndex = 1)
    {
      Workbook workbook = (new Application()).Workbooks.Open(filePath);
      m_worksheet = workbook.Worksheets[sheetIndex];
    }

    public object[,] GetRange(string start, string end)
    {
      return m_worksheet.Range[start, end].Value;
    }

    public int LastRow
    {
      get 
      { 
        return
          m_worksheet.Cells.Find("*", 
          SearchOrder: XlSearchOrder.xlByRows, 
          SearchDirection: XlSearchDirection.xlPrevious)
          .Row; 
      }
    }

    public int LastCol
    {
      get 
      {
        return
          m_worksheet.Cells.Find("*",
          SearchOrder: XlSearchOrder.xlByColumns,
          SearchDirection: XlSearchDirection.xlPrevious).
          Row;
      }
    }

    public void Dispose()
    {
      if (m_workbook != null && !m_is_disposed)
        m_workbook.Close();
    }
  }
}
