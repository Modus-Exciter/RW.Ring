using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Schicksal.Clustering
{
  /// <summary>
  /// Класс для данных введённых пользователем на форме
  /// </summary>
  public class ClusteringParameters
  {
    uint m_k;
    DataTable m_table;
    public ClusteringParameters(DataTable table)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      var columns = new List<ColumnWeight>(table.Columns.Count);

      foreach (DataColumn column in table.Columns)
      {
        // Если тип колонки числовой, то её можно использовать для кластеризации
        if ((column.DataType.IsPrimitive && column.DataType != typeof(bool)) || column.DataType == typeof(decimal))
          columns.Add(new ColumnWeight(column.ColumnName));
      }

      columns.TrimExcess();
      this.ColumnWeights = columns.ToArray();
      this.m_table = table;
    }

    public IEnumerable<IDistanceMetrics<double>> GetAllDistanceMetrics()
    {
      foreach (var type in typeof(IDistanceMetrics<>).Assembly.GetTypes())
      {
        if (typeof(IDistanceMetrics<double>).IsAssignableFrom(type) && !type.IsAbstract)
          yield return (IDistanceMetrics<double>)Activator.CreateInstance(type);
      }
    }

    public IEnumerable<IArcDeleter<double>> GetAllArcDeleters()
    {
      foreach (var type in typeof(IArcDeleter<>).Assembly.GetTypes())
      {
        if (typeof(IArcDeleter<double>).IsAssignableFrom(type) && !type.IsAbstract)
          yield return (IArcDeleter<double>)Activator.CreateInstance(type);
      }
    }

    public uint ClusterCount
    {
      get { return m_k; }
      set {
        if (value < 2)
          throw new ArgumentOutOfRangeException("ClusterCount");
        this.m_k = value; }

    }
    public IDistanceMetrics<double> DistanceMetrics { get; set; }

    public IArcDeleter<double> ArcDeleter { get; set; }

    public ColumnWeight[] ColumnWeights { get; private set; }

    public PixelColumns PixelColumns { get; set; }
  }

  public class ColumnWeight
  {
    private double m_weight;
    public string Column { get; private set; }

    internal ColumnWeight(string column)
    {
      if (string.IsNullOrEmpty(column))
        throw new ArgumentNullException("column");

      this.Column = column;
    }

    public double Weight
    {
      get { return m_weight; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("Weight");

        if (double.IsNaN(value) || double.IsInfinity(value))
          throw new ArgumentOutOfRangeException("Weight");

        m_weight = value;
      }
    }
  }
  /// <summary>
  ///     Определяет колонки как координаты пикселя изображения (По соседним) 
  /// </summary>
  public class PixelColumns
  {
    private readonly List<string> m_columns;
    
    public PixelColumns(DataTable table)
    {
      if (table == null)
        throw new ArgumentNullException("table");

      m_columns = new List<string>(table.Columns.Count);

      foreach (DataColumn column in table.Columns)
      {
        if (column.DataType.IsPrimitive && column.DataType != typeof(bool)
          && column.DataType != typeof(float) && column.DataType != typeof(double))
        {
          m_columns.Add(column.ColumnName);
        }
      }

      m_columns.TrimExcess();
    }

    public string Height { get; set; }

    public string Width { get; set; }

    public IEnumerable<string> GetAllColumns()
    {
      return m_columns.AsEnumerable();
    }
  }
}
