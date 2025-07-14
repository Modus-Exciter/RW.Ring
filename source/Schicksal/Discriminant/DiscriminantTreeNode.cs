using System;
using System.Collections.Generic;

namespace Schicksal.Discriminant
{
  /// <summary>
  /// Узел дерева решений
  /// </summary>
  public class DiscriminantTreeNode
  {
    public string FeatureName { get; set; }      // имя предиктора
    /// <summary>
    /// Пороговое значение для разделения данных.
    /// Все значения <= Znach идут налево, > Znach — направо
    /// </summary>
    public double Znach { get; set; }         // пороговое значение
    public string ClassName { get; set; }         // название класса
    public DiscriminantTreeNode Left { get; set; }  // <= Znach
    public DiscriminantTreeNode Right { get; set; } // > Znach
    /// <summary>
    /// Возвращает true, конечным узлом с классом
    /// </summary>
    public bool End => this.ClassName != null;
    public override string ToString()
    {
      return this.End
          ? $"Класс: {this.ClassName}"
          : $"{this.FeatureName} ≤ {this.Znach:F2}";
    }
    /// <summary>
    /// Рекурсивно выводит дерево
    /// </summary>
   
    }
  }

