using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Schicksal.Clustering;
using System.Collections.Generic;
using System.Data;


namespace ClusteringTest
{
  [TestClass]
  public class cciMST_test
  {
    [TestMethod]
    public void TwoMoons_global()
    {
      System.IO.FileStream fileStream = new System.IO.FileStream("C:\\Users\\Golik\\source\\repos\\Modus-Exciter\\RW.Ring\\test\\Schicksal\\ClusteringTest\\Resources\\2moons.sks", System.IO.FileMode.Open);
      DataTable data_table = Schicksal.DataTableSaver.ReadDataTable(fileStream);
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
      System.IO.FileStream fileStream = new System.IO.FileStream("C:\\Users\\Golik\\source\\repos\\Modus-Exciter\\RW.Ring\\test\\Schicksal\\ClusteringTest\\Resources\\2moons.sks", System.IO.FileMode.Open);
      DataTable data_table = Schicksal.DataTableSaver.ReadDataTable(fileStream);
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
      System.IO.FileStream fileStream = new System.IO.FileStream("C:\\Users\\Golik\\source\\repos\\Modus-Exciter\\RW.Ring\\test\\Schicksal\\ClusteringTest\\Resources\\2blots.sks", System.IO.FileMode.Open);
      DataTable data_table = Schicksal.DataTableSaver.ReadDataTable(fileStream);
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
      System.IO.FileStream fileStream = new System.IO.FileStream("C:\\Users\\Golik\\source\\repos\\Modus-Exciter\\RW.Ring\\test\\Schicksal\\ClusteringTest\\Resources\\2blots.sks", System.IO.FileMode.Open);
      DataTable data_table = Schicksal.DataTableSaver.ReadDataTable(fileStream);
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
      System.IO.FileStream fileStream = new System.IO.FileStream("C:\\Users\\Golik" +
        "\\source\\repos\\Modus-Exciter\\RW.Ring\\test\\Schicksal" +
        "\\ClusteringTest\\Resources\\3circles.sks", System.IO.FileMode.Open);
      DataTable data_table = Schicksal.DataTableSaver.ReadDataTable(fileStream);
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
      System.IO.FileStream fileStream = new System.IO.FileStream("C:\\Users\\Golik" +
        "\\source\\repos\\Modus-Exciter\\RW.Ring\\test\\Schicksal" +
        "\\ClusteringTest\\Resources\\3circles.sks", System.IO.FileMode.Open);
      DataTable data_table = Schicksal.DataTableSaver.ReadDataTable(fileStream);
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
