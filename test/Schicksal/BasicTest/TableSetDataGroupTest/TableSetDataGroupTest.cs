using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Basic;
using System.Data;

namespace TableSetDataGroupTest
{
  [TestClass]
  public class TableSetDataGroupTest
  {
    [TestMethod]
    public void DataTableVsTSDGComparison()
    {
      DataTable dt = new DataTable("Obs1");
      DataColumn dtCol;
      DataRow dtRow;

      dtCol = new DataColumn();
      dtCol.DataType = typeof(String);
      dtCol.ColumnName = "Variety";
      dtCol.Caption = "Crop type";
      dtCol.Unique = false;
      dtCol.ReadOnly = false;
      dt.Columns.Add(dtCol);

      dtCol = new DataColumn();
      dtCol.DataType = typeof(String);
      dtCol.ColumnName = "N";
      dtCol.Caption = "Nitrogen fertilizer";
      dtCol.Unique = false;
      dtCol.ReadOnly = false;
      dt.Columns.Add(dtCol);

      dtCol = new DataColumn();
      dtCol.DataType = typeof(Double);
      dtCol.ColumnName = "Yield";
      dtCol.Caption = "Amount of yield (centner/hectare)";
      dtCol.Unique = false;
      dtCol.ReadOnly = false;
      dt.Columns.Add(dtCol);

      dtCol = new DataColumn();
      dtCol.DataType = typeof(Int32);
      dtCol.ColumnName = "Ignore";
      dtCol.Caption = "Ignorable column";
      dtCol.Unique = false;
      dtCol.ReadOnly = false;
      dt.Columns.Add(dtCol);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Itensiv";
      dtRow["N"] = "N0";
      dtRow["Yield"] = 20;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Itensiv";
      dtRow["N"] = "N0";
      dtRow["Yield"] = 28;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Itensiv";
      dtRow["N"] = "N0";
      dtRow["Yield"] = 15;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Extensiv";
      dtRow["N"] = "N0";
      dtRow["Yield"] = 32;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Extensiv";
      dtRow["N"] = "N0";
      dtRow["Yield"] = 30;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Extensiv";
      dtRow["N"] = "N0";
      dtRow["Yield"] = 36;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Itensiv";
      dtRow["N"] = "N60";
      dtRow["Yield"] = 21;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Itensiv";
      dtRow["N"] = "N60";
      dtRow["Yield"] = 30;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Itensiv";
      dtRow["N"] = "N60";
      dtRow["Yield"] = 19;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Extensiv";
      dtRow["N"] = "N60";
      dtRow["Yield"] = 34;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Extensiv";
      dtRow["N"] = "N60";
      dtRow["Yield"] = 31;
      dt.Rows.Add(dtRow);

      dtRow = dt.NewRow();
      dtRow["Variety"] = "Extensiv";
      dtRow["N"] = "N60";
      dtRow["Yield"] = 36;
      dt.Rows.Add(dtRow);

      string[] fc = { "Variety", "N" };
      string[] ic = { "Ignore" };
      string rc = "Yield";

      double e0 = Convert.ToDouble(dt.Rows[0].ItemArray[2]);
      double e1 = Convert.ToDouble(dt.Rows[1].ItemArray[2]);
      double e2 = Convert.ToDouble(dt.Rows[2].ItemArray[2]);
      double e3 = Convert.ToDouble(dt.Rows[3].ItemArray[2]);
      double e4 = Convert.ToDouble(dt.Rows[4].ItemArray[2]);
      double e5 = Convert.ToDouble(dt.Rows[5].ItemArray[2]);
      double e6 = Convert.ToDouble(dt.Rows[6].ItemArray[2]);
      double e7 = Convert.ToDouble(dt.Rows[7].ItemArray[2]);
      double e8 = Convert.ToDouble(dt.Rows[8].ItemArray[2]);
      double e9 = Convert.ToDouble(dt.Rows[9].ItemArray[2]);
      double e10 = Convert.ToDouble(dt.Rows[10].ItemArray[2]);
      double e11 = Convert.ToDouble(dt.Rows[11].ItemArray[2]);

      TableSetDataGroup tsdg = new TableSetDataGroup(dt, fc, ic, rc);
      double a0 = tsdg[0][0][0];
      double a1 = tsdg[0][0][1];
      double a2 = tsdg[0][0][2];
      double a3 = tsdg[1][0][0];
      double a4 = tsdg[1][0][1];
      double a5 = tsdg[1][0][2];
      double a6 = tsdg[2][0][0];
      double a7 = tsdg[2][0][1];
      double a8 = tsdg[2][0][2];
      double a9 = tsdg[3][0][0];
      double a10 = tsdg[3][0][1];
      double a11 = tsdg[3][0][2];

      Assert.AreEqual(e0, a0);
      Assert.AreEqual(e1, a1);
      Assert.AreEqual(e2, a2);
      Assert.AreEqual(e3, a3);
      Assert.AreEqual(e4, a4);
      Assert.AreEqual(e5, a5);
      Assert.AreEqual(e6, a6);
      Assert.AreEqual(e7, a7);
      Assert.AreEqual(e8, a8);
      Assert.AreEqual(e9, a9);
      Assert.AreEqual(e10, a10);
      Assert.AreEqual(e11, a11);
    }
  }
}
