using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Notung;
using static Notung.Data.MST;


namespace Schicksal.Clustering
{
  public class ClusteringProcessor : RunBase
  {
    private  DataTable m_table;
    private ColumnWeight[] m_column_weights;
    private double[] m_weights;
    private string[] m_fields;
    private  uint m_k;
    private  IDistanceMetrics<double> m_distance_metric;
    private IArcDeleter<double> m_arc_deleter;
    private HashSet<int>[] result;
    private DataTable m_clusters;
    private DataTable[] m_cluster_points;

    public ClusteringProcessor(DataTable table, ClusteringParameters parameters) {
      if (table == null)
        throw new ArgumentNullException("table");

      if (parameters == null)
        throw new ArgumentNullException("fields");
      if (parameters.ClusterCount < 2)
        throw new ArgumentNullException("ClusterCount");
      m_table = table;
      m_column_weights = parameters.ColumnWeights;
      m_k = parameters.ClusterCount;
      result = new HashSet<int>[m_k];
      m_arc_deleter = parameters.ArcDeleter;
      m_distance_metric = parameters.DistanceMetrics;
      m_clusters = new DataTable();
      m_cluster_points = new DataTable[m_k];
      m_fields = this.GetFields();
      for (int j = 0; j < this.m_k; j++)
      {
        m_cluster_points[j] = new DataTable();
      }
      for (int i = 0; i < m_fields.Length; i++)
      {
        m_clusters.Columns.Add(m_fields[i],typeof(Double));
      }
      for (int j=0; j < this.m_k; j++) {
      for (int i = 0; i < table.Columns.Count; i++){
      m_cluster_points[j].Columns.Add(table.Columns[i].ColumnName, table.Columns[i].DataType);
      }
      }
      m_clusters.Columns.Add("dispersion", typeof(Double));
      m_clusters.Columns.Add("radius", typeof(Double));
    }

    public DataTable GetResultClusters {
      get { return this.m_clusters; }
      private set { }
    }

    public DataTable[] GetResultClusterPoints
    {
      get { return this.m_cluster_points; }
      private set { }
    }
    public double[] Weights{
      get{return m_weights;}
    private set{
        if (value != null) { m_weights = value;}}
    }

    public string[] fields
    {
      get { return m_fields; }
      private set
      {
        if (value != null) { m_fields = value; }
      }
    }
    public override void Run()
    {
      this.GetFields();
      WeightedTableGraph graph = new WeightedTableGraph(m_table,m_fields,m_distance_metric,m_weights);
      if (m_arc_deleter.GetType() == typeof(DoubleSimpleArcDeleter)) {
      this.m_arc_deleter = new DoubleSimpleArcDeleter();
       ((DoubleSimpleArcDeleter)this.m_arc_deleter).ClusterCount=this.m_k;
      this.result = new HashSet<int>[this.m_k];
        ((DoubleSimpleArcDeleter)this.m_arc_deleter).MST = Prim(graph).ToList();
        ((DoubleSimpleArcDeleter)this.m_arc_deleter).start();
      this.result = ((DoubleSimpleArcDeleter)this.m_arc_deleter).GetResult;
      List<DataRow>[] clusters = new List<DataRow>[this.m_k] ;
        for (int i = 0; i < this.m_k; i++)
        {
            clusters[i]=new List<DataRow>();
        }
          for (int i = 0; i < this.m_k; i++) {
          foreach (var point in this.result[i]) {
            clusters[i].Add(this.m_table.Rows[point]);
          }
        }
        
        for (int i = 0; i < this.m_k; i++)
        {
          List<object> Data = new List<object>();
          List<double> rows = new List<double>(this.CalculateCenter(clusters[i]));
          for(int j=0;j<rows.Count;j++) {
          Data.Add((object)rows[j]);
          }
          Data.Add((object)this.CalculateDispersion(clusters[i]));  
          Data.Add((object)this.CalculateRadius(clusters[i]));
          m_clusters.Rows.Add(Data.ToArray());
        }
        for (int i = 0; i < this.m_k; i++)
        {
          foreach (var point in result[i]) {
          List<object> Data = new List<object>();
            for(int j=0;j<this.m_table.Columns.Count; j++) {
              Data.Add((object)this.m_table.Rows[point][j]);
            }
            m_cluster_points[i].Rows.Add(Data.ToArray());
          }
          }
      }
      if (m_arc_deleter.GetType() == typeof(DoubleCciMST_deleter))
      {
        this.m_arc_deleter = new DoubleCciMST_deleter();
        ((DoubleCciMST_deleter)this.m_arc_deleter).MST = Prim(graph).ToList();
        ((DoubleCciMST_deleter)this.m_arc_deleter).getGraph = graph;
        ((DoubleCciMST_deleter)this.m_arc_deleter).start();
        this.result = ((DoubleCciMST_deleter)this.m_arc_deleter).GetResult;
        List<DataRow>[] clusters = new List<DataRow>[this.m_k];
        for (int i = 0; i < this.m_k; i++)
        {
          clusters[i] = new List<DataRow>();
        }
        for (int i = 0; i < this.m_k; i++)
        {
          foreach (var point in this.result[i])
          {
            clusters[i].Add(this.m_table.Rows[point]);
          }
        }

        for (int i = 0; i < this.m_k; i++)
        {
          List<object> Data = new List<object>();
          List<double> rows = new List<double>(this.CalculateCenter(clusters[i]));
          for (int j = 0; j < rows.Count; j++)
          {
            Data.Add((object)rows[j]);
          }
          Data.Add((object)this.CalculateDispersion(clusters[i]));
          Data.Add((object)this.CalculateRadius(clusters[i]));
          m_clusters.Rows.Add(Data.ToArray());
        }
        for (int i = 0; i < this.m_k; i++)
        {
          foreach (var point in result[i])
          {
            List<object> Data = new List<object>();
            for (int j = 0; j < this.m_table.Columns.Count; j++)
            {
              Data.Add((object)this.m_table.Rows[point][j]);
            }
            m_cluster_points[i].Rows.Add(Data.ToArray());
          }
        }
      }

      }

    double CalculateRadius(List<DataRow> cluster)
      {
      List<double> center = this.CalculateCenter(cluster);
      double distance = 0;
      foreach (var point in cluster)
      {
        for (int i = 0; i < m_fields.Length; i++)
        {
          this.m_distance_metric.BeginCalculation();
          this.m_distance_metric.AddDifference(center[i], double.Parse(point[m_fields[i]].ToString()));
        }
        double res = this.m_distance_metric.GetResult();
        if (distance < res) { distance = res; }

      }
        return distance;
      }
      double CalculateDispersion(List<DataRow> cluster) {
      List<double> center = this.CalculateCenter(cluster);
      double res = 0;
      foreach (var point in cluster)
      {
      for (int i = 0; i < m_fields.Length; i++)
      {
          this.m_distance_metric.BeginCalculation();
          this.m_distance_metric.AddDifference(center[i], double.Parse(point[m_fields[i]].ToString()));
      }
      res+= this.m_distance_metric.GetResult();
      }
      res = res / cluster.Count;

      return Math.Sqrt(res);
    }
    

    List<double> CalculateCenter(List<DataRow> cluster) {
      List<double> center = new List<double>();
      for (int i = 0; i < m_fields.Length; i++)
      {
        center.Add(0);
      }
      foreach (var point in cluster) {
        for(int i=0;i<m_fields.Length;i++) {
          center[i]+=double.Parse(point[m_fields[i]].ToString());
        }
      }
      for (int i = 0; i < m_fields.Length; i++)
      {
        center[i] = center[i]/cluster.Count;
      }
      return center;
    }
   string[] GetFields() {
      List<string> fields = new List<string>();
      List<double> weights = new List<double>();
      foreach (var field in m_column_weights) {
        if (field.Weight>0) {
        fields.Add(field.Column);
        weights.Add(field.Weight);
        }
      }
      m_weights = weights.ToArray();
      m_fields = fields.ToArray();
      return fields.ToArray();
    }
  }
}
