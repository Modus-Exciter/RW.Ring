using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schicksal.Clustering;
using System;
using System.Collections.Generic;
using System.Data;
using static Notung.Data.MST;



namespace ClusteringTest
{

  public class TupleComparer : IComparer<Tuple<int, int, double>>
  {
    public int Compare(Tuple<int, int, double> x, Tuple<int, int, double> y)
    {
      if (x == null)
      {
        if (y == null)
          return 0;
        else
          return 1;
      }
      else if (y == null)
        return -1;

      return x.Item3.CompareTo(y.Item3);
    }
  }

  [TestClass]
    public class SEMST_test
    {
        [TestMethod]
        public void TwoMoons()
        {
          System.IO.FileStream fileStream=new System.IO.FileStream("C:\\Users\\Golik\\source\\repos\\Modus-Exciter\\RW.Ring\\test\\Schicksal\\ClusteringTest\\Resources\\2moons.sks",System.IO.FileMode.Open);
          DataTable data_table = Schicksal.DataTableSaver.ReadDataTable(fileStream);
          string[] fields = { "x", "y" };
          SEMST semst = new SEMST(data_table,fields,2);
          semst.start();
          List<DataRow[]> clusters = new List<DataRow[]>(); ;
          clusters.Add( data_table.Select("cluster_id = 0"));
          clusters.Add(data_table.Select("cluster_id = 1"));
          HashSet<int>[] result = semst.m_clusters;
          foreach (var cluster in clusters) {
          for (int i = 0; i < 2; i++) {
          if (result[i].Contains(int.Parse(cluster[0][0].ToString()))) {
                foreach (var row in cluster) {
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
              SEMST semst = new SEMST(data_table, fields, 2);
              semst.start();
              List<DataRow[]> clusters = new List<DataRow[]>();
              clusters.Add(data_table.Select("cluster_id = 0"));
              clusters.Add(data_table.Select("cluster_id = 1"));
              HashSet<int>[] result = semst.m_clusters;
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
              SEMST semst = new SEMST(data_table, fields, 3);
              semst.start();
              List<DataRow[]> clusters = new List<DataRow[]>(); ;
              clusters.Add(data_table.Select("cluster_id = 0"));
              clusters.Add(data_table.Select("cluster_id = 1"));
              clusters.Add(data_table.Select("cluster_id = 2"));  
              HashSet<int>[] result = semst.m_clusters;
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
