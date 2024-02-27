using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Clustering;
using System.Data;

namespace ClusteringTest
{
  [TestClass]
  public class WeightedTableGraphTest
  {

    [TestMethod]
    public void WeightedTableGraphConstructorTest()
    {
      DataTable table = new DataTable();
      DataColumn column = new DataColumn
      {
        ColumnName = "x",
        DataType = System.Type.GetType("System.Double")
      };
      table.Columns.Add(column);
      column = new DataColumn
      {
        ColumnName = "y",
        DataType = System.Type.GetType("System.Double")
      };
      table.Columns.Add(column);
      DataRow row;
      for (int i = 1; i <= 10; i++)
      {
        row = table.NewRow();
        row["x"] = i;
        row["y"] = i;
        table.Rows.Add(row);
      }
      string[] fields = { "x", "y" };
      WeightedTableGraph tableGraph = new WeightedTableGraph(table, fields);
      Assert.AreEqual(10, tableGraph.PeakCount);
      Assert.AreEqual(System.Math.Sqrt(2), tableGraph[0, 1]);
      Assert.AreEqual(9, tableGraph.IncomingCount(0));
      Assert.AreEqual(tableGraph.OutgoingCount(0), tableGraph.IncomingCount(0));
    }
    [TestMethod]
    public void WeightedTableGraphArcsTest()
    {
      DataTable table = new DataTable();
      DataColumn column = new DataColumn
      {
        ColumnName = "x",
        DataType = System.Type.GetType("System.Double")
      };
      table.Columns.Add(column);
      column = new DataColumn
      {
        ColumnName = "y",
        DataType = System.Type.GetType("System.Double")
      };
      table.Columns.Add(column);
      DataRow row;
      for (int i = 1; i <= 10; i++)
      {
        row = table.NewRow();
        row["x"] = i;
        row["y"] = i;
        table.Rows.Add(row);
      }
      string[] fields = { "x", "y" };
      WeightedTableGraph tableGraph = new WeightedTableGraph(table, fields);
      Assert.AreEqual(10, tableGraph.PeakCount);
      Assert.AreEqual(9, tableGraph.IncomingCount(0));
      Assert.AreEqual(tableGraph.OutgoingCount(0), tableGraph.IncomingCount(0));
      double[] weights = { 1, 0.5 };
      DataTable weightedTable = new DataTable();
      DataColumn weightedColumn = new DataColumn
      {
        ColumnName = "x",
        DataType = System.Type.GetType("System.Double")
      };
      weightedTable.Columns.Add(weightedColumn);
      weightedColumn = new DataColumn
      {
        ColumnName = "y",
        DataType = System.Type.GetType("System.Double")
      };
      weightedTable.Columns.Add(weightedColumn);
      DataRow weightedRow;
      for (int i = 1; i <= 10; i++)
      {
        weightedRow = weightedTable.NewRow();
        weightedRow["x"] = i;
        weightedRow["y"] = i;
        weightedTable.Rows.Add(weightedRow);
      }
      string[] weightedFields = { "x", "y" };
      IDistanceMetrics<double> euclidian = new EuclidDistanceMetrics();
      WeightedTableGraph testWeighted = new WeightedTableGraph(weightedTable, weightedFields, euclidian, weights);
      Assert.AreEqual(System.Math.Sqrt(1.25), testWeighted[0, 1]);
    }

  }
}