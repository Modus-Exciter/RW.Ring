using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static System.Math;
using static Notung.Data.MST;

namespace Schicksal.Clustering
{
 public class cciMST
  {
    public List<Tuple<int, int, double>> m_mst_list;
    Tuple<int, int, double>[] m_mst;
    List<int> m_local_centroids = new List<int>();
    List<int> m_global_centroids = new List<int>();
    WeightedTableGraph m_graph;
    Dictionary<int, double> m_global_rho = new Dictionary<int, double>();
    Dictionary<int, double> m_global_delta = new Dictionary<int, double>();
    Dictionary<int, double> m_local_rho = new Dictionary<int, double>();
    Dictionary<int, double> m_local_delta = new Dictionary<int, double>();
    Dictionary<Tuple<int, int>, double> m_geodesic_distances = new Dictionary< Tuple<int, int>, double>();
    double m_local_s = 0.03;
    double m_global_s = 0.20;
    double m_global_variance;
    double m_local_variance;
    int m_k_global;
    int m_k_local;
    List<Tuple<int, int, double>> m_local_deleted_edges = new List<Tuple<int, int, double>>();
    List<Tuple<int, int, double>> m_global_deleted_edges = new List<Tuple<int, int, double>>();
    HashSet<int> m_local_deleted_edges_peak = new HashSet<int>();
    HashSet<int> m_global_deleted_edges_peak = new HashSet<int>();
    HashSet<int>[] m_local_clusters;
    HashSet<int>[] m_global_clusters;

    public cciMST(DataTable table, string[] fields)
    {
      this.m_graph = new WeightedTableGraph(table, fields);
      this.m_mst = Prim(m_graph);
      this.m_mst_list = new List<Tuple<int, int, double>>(m_mst);
      this.get_deodesic();
      this.calculate_variance();
      this.calculate_rho();
      this.calculate_delta();
      this.find_k();
      this.m_k_global = this.m_global_centroids.Count;
      this.m_k_local = this.m_local_centroids.Count;
      this.m_local_clusters = new HashSet<int>[m_k_local];
      this.m_global_clusters = new HashSet<int>[m_k_global];
      for (int i = 0; i < m_k_local; i++)
      {
        this.m_local_clusters[i] = new HashSet<int>();
      }
      for (int i = 0; i < m_k_global; i++)
      {
        this.m_global_clusters[i] = new HashSet<int>();
      }
      this.delete_edges_local();
      this.delete_edges_global();

    }

    public int get_k_local() {
      return this.m_k_local;
    }

    public int get_k_global()
    {
      return this.m_k_global;
    }
    void get_deodesic() {
      for (int i = 0; i < this.m_graph.PeakCount; i++) {
        for (int j = 0; j < this.m_graph.PeakCount; j++)
        {
          if (i == j) { continue; }
          List < Tuple<int, int, double> > path = this.FindPath(i,j);
          double distance = 0;
          foreach (var edge in path) { distance +=edge.Item3; }
          this.m_geodesic_distances.Add(new Tuple<int, int>(i,j),distance);
        }
      }
    }

    public HashSet<int>[] get_local_result() {
      return this.m_local_clusters;
    }

    public HashSet<int>[] get_global_result()
    {
      return this.m_global_clusters;
    }
    void delete_edges_local()
    {
      this.m_mst_list = new List<Tuple<int, int, double>>(this.m_mst);
      List < Tuple<int, int, double> > path = new List<Tuple<int, int, double>>();
      for (int i = 0; i < this.m_mst_list.Count - 1; i++) {
        this.m_mst_list[i] = new Tuple<int, int, double>(this.m_mst_list[i].Item1,
        this.m_mst_list[i].Item2, this.calculate_zeta(this.m_mst_list[i].Item1, this.m_mst_list[i].Item2, this.m_local_rho));
      }
      for (int i = 0; i < this.m_k_local - 1; i++)
      {
        path.AddRange(this.FindPath(this.m_local_centroids[i], this.m_local_centroids[i + 1]));
      }
      HashSet<Tuple<int, int, double>> hashset_path = new HashSet<Tuple<int, int, double>>(path);
      path = hashset_path.ToList();
      path.Sort(new TupleComparer());
      for (int i = 0; i < this.m_k_local - 1; i++)
      {
        Tuple<int, int, double> reverse = new Tuple<int, int, double>(path.Last().Item2, path.Last().Item1, path.Last().Item3);
        if (this.m_mst_list.Contains(path.Last())) {
          this.m_local_deleted_edges.Add(path.Last());
          this.m_local_deleted_edges_peak.Add(path.Last().Item1);
          this.m_local_deleted_edges_peak.Add(path.Last().Item2);
          this.m_mst_list.Remove(path.Last());
          path.Remove(path.Last());
        }
        if (this.m_mst_list.Contains(reverse)) {
          this.m_local_deleted_edges.Add(path.Last());
          this.m_local_deleted_edges_peak.Add(path.Last().Item1);
          this.m_local_deleted_edges_peak.Add(path.Last().Item2);
          this.m_mst_list.Remove(reverse);
          path.Remove(path.Last());
        }
      }
      for (int i = 0; i < this.m_local_clusters.Length; i++)
      {
        this.m_local_clusters[i].Add(this.m_local_centroids[i]);
        this.AddChildsFromEdges(this.m_local_centroids[i], i, this.m_local_clusters);
      }
      foreach (int peak in this.m_local_deleted_edges_peak)
      {
        double min = double.PositiveInfinity;
        int min_cluster_id = this.m_local_clusters.Length - 1;
        for (int i = 0; i < this.m_local_clusters.Length; i++)
        {
          if (peak == i) { continue; };
          double distance = this.m_geodesic_distances[new Tuple<int, int>(peak, i)];
          if (distance < min)
          {
            min = distance;
            min_cluster_id = i;
          }
        }
        this.m_local_clusters[min_cluster_id].Add(peak);
      }

      while (this.m_mst_list.Count > 0)
      {
        for (int j = 0; j < this.m_local_clusters.Length; j++)
        {
          for (int i = 0; i < this.m_mst_list.Count; i++)
          {
            if (this.m_local_clusters[j].Contains(this.m_mst_list[i].Item1) || this.m_local_clusters[j].Contains(this.m_mst_list[i].Item2))
            {
              this.m_local_clusters[j].Add(this.m_mst_list[i].Item1);
              this.m_local_clusters[j].Add(this.m_mst_list[i].Item2);
              this.m_mst_list.Remove(m_mst_list[i]);
            }
          }
        }
      }
    }

    void delete_edges_global()
    {
      this.m_mst_list = new List<Tuple<int, int, double>>(this.m_mst);
      List<Tuple<int, int, double>> path = new List<Tuple<int, int, double>>();
      for (int i = 0; i < this.m_mst_list.Count - 1; i++)
      {
        this.m_mst_list[i] = new Tuple<int, int, double>(this.m_mst_list[i].Item1,
        this.m_mst_list[i].Item2, this.calculate_zeta(this.m_mst_list[i].Item1, this.m_mst_list[i].Item2, this.m_global_rho));
      }
      for (int i = 0; i < this.m_k_global - 1; i++)
      {
        path.AddRange(this.FindPath(this.m_global_centroids[i], this.m_global_centroids[i + 1]));
      }
      HashSet<Tuple<int, int, double>> hashset_path = new HashSet<Tuple<int, int, double>>(path);
      path = hashset_path.ToList();
      path.Sort(new TupleComparer());
      for (int i = 0; i < this.m_k_global - 1; i++)
      {
        Tuple<int, int, double> reverse = new Tuple<int, int, double>(path.Last().Item2, path.Last().Item1, path.Last().Item3);
        if (this.m_mst_list.Contains(path.Last()))
        {
          this.m_global_deleted_edges.Add(path.Last());
          this.m_global_deleted_edges_peak.Add(path.Last().Item1);
          this.m_global_deleted_edges_peak.Add(path.Last().Item2);
          this.m_mst_list.Remove(path.Last());
          path.Remove(path.Last());
        }
        else {
        if (this.m_mst_list.Contains(reverse))
        {
          this.m_global_deleted_edges.Add(path.Last());
          this.m_global_deleted_edges_peak.Add(path.Last().Item1);
          this.m_global_deleted_edges_peak.Add(path.Last().Item2);
          this.m_mst_list.Remove(reverse);
          path.Remove(path.Last());
        }
        }
      }
        for(int i=0;i<this.m_global_clusters.Length;i++) {
          this.m_global_clusters[i].Add(this.m_global_centroids[i]);
          this.AddChildsFromEdges(this.m_global_centroids[i],i,this.m_global_clusters);
        }
        foreach (int peak in this.m_global_deleted_edges_peak) {
        double min=double.PositiveInfinity;
        int min_cluster_id= this.m_global_clusters.Length-1;
        for (int i = 0; i < this.m_global_clusters.Length; i++)
        {
          if (peak == i) { continue; };
          double distance = this.m_geodesic_distances[new Tuple<int,int>(peak,i)];
          if (distance < min) {
            min = distance;
            min_cluster_id = i;
          }
        }
        this.m_global_clusters[min_cluster_id].Add(peak);
        }

        while (this.m_mst_list.Count > 0)
        {
          for (int j = 0; j < this.m_global_clusters.Length; j++)
          {
            for (int i = 0; i < this.m_mst_list.Count; i++)
            {
            if (this.m_global_clusters[j].Contains(this.m_mst_list[i].Item1) || this.m_global_clusters[j].Contains(this.m_mst_list[i].Item2))
            {
              this.m_global_clusters[j].Add(this.m_mst_list[i].Item1);
              this.m_global_clusters[j].Add(this.m_mst_list[i].Item2);
              this.m_mst_list.Remove(m_mst_list[i]);
            }
            }
          }
        }
    }

    double calculate_Sep() { return 0; }
    double calculate_CP() { return 0; }
    public void AddChildsFromEdges(int id, int clusterIndex, HashSet<int>[] m_clusters)
    {
      List<int> childs = new List<int>();
      for (int i = 0; i < this.m_mst_list.Count; i++)
      {
        if (this.m_mst_list[i].Item1 == id)
        {
          childs.Add(this.m_mst_list[i].Item2);
          this.m_mst_list.Remove(this.m_mst_list[i]);
        }
        else
        {
          if (this.m_mst_list[i].Item2 == id)
          {
            childs.Add(this.m_mst_list[i].Item1);
            this.m_mst_list.Remove(this.m_mst_list[i]);
          }
        }
      }
      foreach (var child in childs)
      {
        m_clusters[clusterIndex].Add(child);
        this.AddChildsFromEdges(child, clusterIndex,m_clusters);
      }
    }
    public List<Tuple<int, int, double>> FindPath(int from, int to)
    {
      Queue<int> queue = new Queue<int>();
      queue.Enqueue(from);
      HashSet<int> visited = new HashSet<int>();
      Dictionary<int,List<Tuple<int, double>>> adj=new Dictionary<int, List<Tuple<int, double>>>();
      List<Tuple<int, int, double>> path = new List<Tuple<int, int, double>>();
      Hashtable parent = new Hashtable();
      while (queue.Count > 0)
      {
      int currentPeak = queue.Dequeue();
      visited.Add(currentPeak);
      if (currentPeak == to)
      {
        while (currentPeak != from)
          {
          int prev = (int)parent[currentPeak];
          double weight = adj[prev].Find(x => x.Item1 == currentPeak).Item2;
          path.Add(new Tuple<int, int, double>(prev, currentPeak, weight));
          currentPeak = prev;
          }
         path.Reverse();
        break;
      }
      for (int i = 0; i < m_mst_list.Count; i++)
      {
        if (m_mst_list[i].Item1 == currentPeak) {
            if (adj.ContainsKey(currentPeak)) {
            adj[currentPeak].Add(new Tuple<int, double>(m_mst_list[i].Item2, m_mst_list[i].Item3));
            }
            else
            {
              adj.Add(currentPeak, new List<Tuple<int, double>>());
              adj[currentPeak].Add(new Tuple<int, double>(m_mst_list[i].Item2, m_mst_list[i].Item3));
            }
        }
          if (m_mst_list[i].Item2 == currentPeak)
          {
            if (adj.ContainsKey(currentPeak))
            {
              adj[currentPeak].Add(new Tuple<int, double>(m_mst_list[i].Item1, m_mst_list[i].Item3));
            }
            else
            {
              adj.Add(currentPeak, new List<Tuple<int, double>>());
              adj[currentPeak].Add(new Tuple<int, double>(m_mst_list[i].Item1, m_mst_list[i].Item3));
            }
          }
        }
        foreach (var neighbor in adj[currentPeak])
        {
          int nextNode = neighbor.Item1;
          if (!visited.Contains(nextNode))
          {
            parent[nextNode] = currentPeak;
            queue.Enqueue(nextNode);
          }
        }
      }
      return path;
    }

    double calculate_zeta(int i, int j, Dictionary<int, double> rho) {
      return ((this.m_graph[i, j]) / (rho[i] + rho[j]));
    }


    HashSet<int>[] best_result() { return new HashSet<int>[this.m_k_global]; }

    void calculate_variance()
    {
      List<double> Distances = new List<double>();
      for (int i = 0; i < this.m_graph.PeakCount; i++) {
        for (int j = 0; j < this.m_graph.PeakCount; j++) {
          if (i == j) { continue; }
          Distances.Add(this.m_graph[i,j]);
        }
      }
      Distances.Sort();
      int n = m_graph.PeakCount;
      int th_local=(int)Math.Round(m_local_s*n*(n-1)/2,MidpointRounding.ToEven);
      int th_global = (int)Math.Round(m_global_s*n*(n - 1)/2, MidpointRounding.ToEven);
      this.m_local_variance = Distances[th_local];
      this.m_global_variance = Distances[th_global];
    }
    void calculate_rho()
    {
      for (int i=0; i < this.m_graph.PeakCount; i++) {
        for (int j = 0; j < this.m_graph.PeakCount; j++) {
          if (i==j) { continue; }
          Tuple<int,int> peak_pair= new Tuple<int, int>(i,j);
          double distance = this.m_geodesic_distances[peak_pair];
          if (!this.m_global_rho.ContainsKey(i))
          {
            this.m_global_rho.Add(i, Math.Exp(-(distance * distance / 2 * (this.m_global_variance * this.m_global_variance))));
          }
          else {
            this.m_global_rho[i] += Math.Exp(-((distance * distance) / 2 * (this.m_global_variance * this.m_global_variance)));
          }
          if (!this.m_local_rho.ContainsKey(i))
          {
            this.m_local_rho.Add(i, Math.Exp(-((distance * distance) / 2 * (this.m_local_variance * this.m_local_variance))));
          }
          else
          {
            this.m_local_rho[i] += Math.Exp(-((distance * distance) / 2 * (this.m_local_variance * this.m_local_variance)));
          }
        }
      }
    }
    void calculate_delta()
    {
      for (int i = 0; i < this.m_graph.PeakCount; i++)
      {
        bool rho_i_smaller=true;
        bool local_rho_i_smaller = true;
        double current_min_distance = this.m_geodesic_distances[new Tuple<int, int>(0, 1)];
        double current_max_distance = 0;
        for (int j = 0; j < this.m_graph.PeakCount; j++)
        {
        if (i == j) { continue; }
        Tuple<int, int> peak_pair = new Tuple<int, int>(i, j);
        double distance = this.m_geodesic_distances[peak_pair];
          if (distance < current_min_distance) { current_min_distance = distance;}
          if (distance > current_max_distance) { current_max_distance = distance;}
          if (this.m_global_rho[i] < this.m_global_rho[j] && rho_i_smaller)
          {
            this.m_global_delta[i] = current_min_distance;
          }
          else {
            this.m_global_delta[i] = current_max_distance;
            rho_i_smaller = false;
          }
          if (this.m_local_rho[i] < this.m_local_rho[j] && local_rho_i_smaller)
          {
            this.m_local_delta[i] = current_min_distance;
          }
          else
          {
            this.m_local_delta[i] = current_max_distance;
            local_rho_i_smaller = false;
          }
        }
      }
    }
    void find_k() {
      List<Tuple<int,double>> global_gamma =new List<Tuple<int, double>>();
      List<Tuple<int, double>> local_gamma = new List<Tuple<int, double>>();
      for (int i = 0; i < this.m_graph.PeakCount; i++) {
        global_gamma.Add(new Tuple<int, double>(i, this.m_global_delta[i] * this.m_global_rho[i]));
        local_gamma.Add(new Tuple<int, double>(i, this.m_local_delta[i] * this.m_local_rho[i]));
      }
      global_gamma.Sort(new GammaComparer());
      local_gamma.Sort(new GammaComparer());
      while (this.check_neighbour(local_gamma.Last().Item1, this.m_local_rho,this.m_local_delta)) {
          if (this.m_local_centroids.Count == 0)
          {
            this.m_local_centroids.Add(local_gamma.Last().Item1);
          }
         else {
        for (int i=0;i<m_local_centroids.Count;i++) {
          if (local_gamma.Last().Item1 == m_local_centroids[i]) { continue; }
          Tuple<int, int> pair = new Tuple<int, int>(local_gamma.Last().Item1,m_local_centroids[i]);
          if (this.m_geodesic_distances[pair] > this.m_local_variance) {
          this.m_local_centroids.Add(local_gamma.Last().Item1);
          }
          }
        }
          local_gamma.Remove(local_gamma.Last());
        
      }
      while (this.check_neighbour(global_gamma.Last().Item1, this.m_global_rho, this.m_global_delta))
      {
          if (this.m_global_centroids.Count == 0)
          {
            this.m_global_centroids.Add(global_gamma.Last().Item1);
          }
          else {
          for (int i = 0; i < m_global_centroids.Count; i++)
          {
            if (global_gamma.Last().Item1 == m_global_centroids[i]) { continue; }
            Tuple<int, int> pair = new Tuple<int, int>(global_gamma.Last().Item1, m_global_centroids[i]);
          if (this.m_geodesic_distances[pair] > this.m_global_variance)
          {
            this.m_global_centroids.Add(global_gamma.Last().Item1);
          }
          }
        }
            global_gamma.Remove(global_gamma.Last());
      }
    }

    bool check_neighbour(int Peak, Dictionary<int, double> rho, Dictionary<int, double> delta) {
      List<int> childs = new List<int>();
      foreach (var edge in this.m_mst) {
        if (edge.Item1 == Peak)
        {
          childs.Add(edge.Item2);
        }
        else
        {
          if (edge.Item2 == Peak)
          {
            childs.Add(edge.Item1); 
          }
        }
      }
      foreach (var child in childs) {
        if (rho[child] > rho[Peak]) { return false; }
      }
        return true; 
    }

    private class GammaComparer : IComparer<Tuple<int, double>>
    {
      public int Compare(Tuple<int, double> x, Tuple<int, double> y)
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
        return x.Item2.CompareTo(y.Item2);
      }
    }
    private class TupleComparer : IComparer<Tuple<int, int, double>>
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
