using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Clustering;
using System.Collections.Generic;
using System.Data;
using ClusteringTest.Properties;

namespace ClusteringTest
{
  [TestClass]
  public class cciMST_test
  {
    [TestMethod]


    public void Simple_global()
    {
      DataTable data_table;
      using (var table = new System.IO.MemoryStream(Resources.simple)) {
      data_table = Schicksal.DataTableSaver.ReadDataTable(table);
      }
      string[] fields = { "x", "y" };
      cciMST cciMST = new cciMST(data_table, fields);
      List<DataRow[]> clusters = new List<DataRow[]>(); ;
      clusters.Add(data_table.Select("cluster_id = 0"));
      clusters.Add(data_table.Select("cluster_id = 1"));
      HashSet<int>[] result = cciMST.get_global_result();
      Assert.AreEqual(2, cciMST.get_k_global());
      foreach (var cluster in clusters)
      {
        for (int i = 0; i < 2; i++)
        {
          if (result[i].Contains(int.Parse(cluster[0][0].ToString())))
          {
            foreach (var row in cluster)
            {
              Assert.IsTrue(result[i].Contains((int)row[0]));
            }
          }
        }
      }
    }
    public void TwoMoons_global()
    {
      DataTable data_table;
      using (var table = new System.IO.MemoryStream(Resources._2moons))
      {
        data_table = Schicksal.DataTableSaver.ReadDataTable(table);
      }
      string[] fields = { "x", "y" };
      cciMST cciMST = new cciMST(data_table, fields);
      List<DataRow[]> clusters = new List<DataRow[]>(); ;
      clusters.Add(data_table.Select("cluster_id = 0"));
      clusters.Add(data_table.Select("cluster_id = 1"));
      HashSet<int>[] result = cciMST.get_global_result();
      Assert.AreEqual(2, cciMST.get_k_global());
      foreach (var cluster in clusters)
      {
        for (int i = 0; i < 2; i++)
        {
          if (result[i].Contains(int.Parse(cluster[0][0].ToString())))
          {
            foreach (var row in cluster)
            {
              Assert.IsTrue(result[i].Contains((int)row[0]));
            }
          }
        }
      }
    }

    [TestMethod]
    public void TwoMoons_local()
    {
      DataTable data_table;
      using (var table = new System.IO.MemoryStream(Resources._2moons))
      {
        data_table = Schicksal.DataTableSaver.ReadDataTable(table);
      }
      string[] fields = { "x", "y" };
      cciMST cciMST = new cciMST(data_table, fields);
      List<DataRow[]> clusters = new List<DataRow[]>(); ;
      clusters.Add(data_table.Select("cluster_id = 0"));
      clusters.Add(data_table.Select("cluster_id = 1"));
      HashSet<int>[] result = cciMST.get_local_result();
      Assert.AreEqual(2, cciMST.get_k_local());
      foreach (var cluster in clusters)
      {
        for (int i = 0; i < 2; i++)
        {
          if (result[i].Contains(int.Parse(cluster[0][0].ToString())))
          {
            foreach (var row in cluster)
            {
              Assert.IsTrue(result[i].Contains((int)row[0]));
            }
          }
        }
      }
    }

    [TestMethod]
    public void TwoBlots()
    {
      DataTable data_table;
      using (var table = new System.IO.MemoryStream(Resources._2blots))
      {
        data_table = Schicksal.DataTableSaver.ReadDataTable(table);
      }
      string[] fields = { "x", "y" };
      cciMST cciMST = new cciMST(data_table, fields);
      List<DataRow[]> clusters = new List<DataRow[]>(); ;
      clusters.Add(data_table.Select("cluster_id = 0"));
      clusters.Add(data_table.Select("cluster_id = 1"));
      HashSet<int>[] result = cciMST.get_global_result();
      Assert.AreEqual(2, cciMST.get_k_global());
      foreach (var cluster in clusters)
      {
        for (int i = 0; i < 2; i++)
        {
          if (result[i].Contains(int.Parse(cluster[0][0].ToString())))
          {
            foreach (var row in cluster)
            {
              Assert.IsTrue(result[i].Contains((int)row[0]));
            }
          }
        }
      }
    }

    [TestMethod]
    public void TwoBlots_local()
    {
      DataTable data_table;
      using (var table = new System.IO.MemoryStream(Resources._2blots))
      {
        data_table = Schicksal.DataTableSaver.ReadDataTable(table);
      }
      string[] fields = { "x", "y" };
      cciMST cciMST = new cciMST(data_table, fields);
      List<DataRow[]> clusters = new List<DataRow[]>(); ;
      clusters.Add(data_table.Select("cluster_id = 0"));
      clusters.Add(data_table.Select("cluster_id = 1"));
      HashSet<int>[] result = cciMST.get_local_result();
      Assert.AreEqual(2,cciMST.get_k_local());
      foreach (var cluster in clusters)
      {
        for (int i = 0; i < 2; i++)
        {
          if (result[i].Contains(int.Parse(cluster[0][0].ToString())))
          {
            foreach (var row in cluster)
            {
              Assert.IsTrue(result[i].Contains((int)row[0]));
            }
          }
        }
      }
    }


    [TestMethod]
    public void ThreeCircles()
    {
      DataTable data_table;
      using (var table = new System.IO.MemoryStream(Resources._3circles))
      {
        data_table = Schicksal.DataTableSaver.ReadDataTable(table);
      }
      string[] fields = { "x", "y" };
      cciMST cciMST = new cciMST(data_table, fields);
      List<DataRow[]> clusters = new List<DataRow[]>(); ;
      clusters.Add(data_table.Select("cluster_id = 0"));
      clusters.Add(data_table.Select("cluster_id = 1"));
      clusters.Add(data_table.Select("cluster_id = 2"));
      HashSet<int>[] result = cciMST.get_global_result();
      Assert.AreEqual(3, cciMST.get_k_global());
      foreach (var cluster in clusters)
      {
        for (int i = 0; i < 3; i++)
        {
          if (result[i].Contains(int.Parse(cluster[0][0].ToString())))
          {
            foreach (var row in cluster)
            {
              Assert.IsTrue(result[i].Contains((int)row[0]));
            }
          }
        }
      }
    }

    [TestMethod]
    public void ThreeCircles_local()
    {
      DataTable data_table;
      using (var table = new System.IO.MemoryStream(Resources._3circles))
      {
        data_table = Schicksal.DataTableSaver.ReadDataTable(table);
      }
      string[] fields = { "x", "y" };
      cciMST cciMST = new cciMST(data_table, fields);
      List<DataRow[]> clusters = new List<DataRow[]>(); ;
      clusters.Add(data_table.Select("cluster_id = 0"));
      clusters.Add(data_table.Select("cluster_id = 1"));
      clusters.Add(data_table.Select("cluster_id = 2"));
      HashSet<int>[] result = cciMST.get_local_result();
      Assert.AreEqual(3, cciMST.get_k_local());
      foreach (var cluster in clusters)
      {
        for (int i = 0; i < 3; i++)
        {
          if (result[i].Contains(int.Parse(cluster[0][0].ToString())))
          {
            foreach (var row in cluster)
            {
              Assert.IsTrue(result[i].Contains((int)row[0]));
            }
          }
        }
      }
    }

  }
}
