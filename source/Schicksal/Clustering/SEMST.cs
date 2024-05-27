using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static Notung.Data.MST;

namespace Schicksal.Clustering
{
  public class SEMST
  {
    int m_k;
    List<Tuple<int, int, double>> m_edge_list;
    Tuple<int, int, double>[] m_mst;
    public HashSet<int>[] m_clusters;
    HashSet<int> m_peaks_of_deleted_edges;

    //public SimpleSEMST() { }
    public SEMST(DataTable table,string[] fields, int k){
      this.m_k = k;
      this.m_mst = Prim(new WeightedTableGraph(table, fields));
      Array.Sort(this.m_mst, new TupleComparer());
      m_edge_list = new List<Tuple<int, int, double>>(this.m_mst);
      m_peaks_of_deleted_edges = new HashSet<int>();
      m_clusters = new HashSet<int>[k];
      for (int i = 0; i < k; i++)
      {
        m_clusters[i] = new HashSet<int>();
      }
    }
    public void start(){
      for (int i = 0; i < m_k-1; i++)
      {
        this.m_peaks_of_deleted_edges.Add(this.m_edge_list.Last().Item1);
        this.m_peaks_of_deleted_edges.Add(this.m_edge_list.Last().Item2);
        this.m_edge_list.Remove(m_edge_list.Last());
      }
      for (int i = 0; i < m_k; i++) {
        this.m_clusters[i].Add(this.m_edge_list.First().Item1);
        this.m_clusters[i].Add(this.m_edge_list.First().Item2);
        this.AddChildsFromEdges(this.m_edge_list.First().Item1, i);
      }
      while (this.m_edge_list.Count > 0) {
        for (int j = 0; j < this.m_clusters.Length; j++) {
          for (int i = 0; i < this.m_edge_list.Count; i++) {
            if (this.m_clusters[j].Contains(this.m_edge_list[i].Item1) || this.m_clusters[j].Contains(this.m_edge_list[i].Item2))
            {
              this.m_clusters[j].Add(this.m_edge_list[i].Item1);
              this.m_clusters[j].Add(this.m_edge_list[i].Item2);
              this.m_edge_list.Remove(m_edge_list[i]);
            }
          }
        }
     }
      foreach (var peak in this.m_peaks_of_deleted_edges) {
        this.AddRoot(peak);      
      }
    }

    public void AddChildsFromEdges(int id,int clusterIndex)
      {
        List<int> childs = new List<int>();
        for(int i=0;i<this.m_edge_list.Count;i++)
        {
        if (this.m_edge_list[i].Item1 == id)
        {
          childs.Add(this.m_edge_list[i].Item2);
          this.m_edge_list.Remove(this.m_edge_list[i]);
        }
        else {
          if (this.m_edge_list[i].Item2 == id)
          {
          childs.Add(this.m_edge_list[i].Item1);
          this.m_edge_list.Remove(this.m_edge_list[i]);
          }
        }
        }
      foreach (var child in childs) {
        this.m_clusters[clusterIndex].Add(child);
        this.AddChildsFromEdges(child, clusterIndex);
      }
      }
    public void AddRoot(int id)
    {
      for (int i = 0; i < m_k; i++)
      {
        if (m_clusters[i].Contains(id)) { return; }
        if (m_clusters[i].Count == 0)
        {
          m_clusters[i].Add(id);
          this.AddChildsFromEdges(id,i);
          return;
        }
        else { continue; };
      }
    }
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
  }
}
