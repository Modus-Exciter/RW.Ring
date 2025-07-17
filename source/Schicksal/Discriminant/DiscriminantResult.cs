using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schicksal.Discriminant
{
    /// <summary>
    /// Результат дискриминантного анализа
    /// </summary>
    public class DiscriminantResult
    {
    /// <summary>
    /// Корень дерева решений
    /// </summary>
    public DiscriminantTreeNode DecisionTree { get; set; }
    /// <summary>
    /// Точность модели
    /// </summary>
    public double Accuracy { get; set; }
    /// <summary>
    /// Распределение классов 
    /// Ключ — имя класса, значение — количество элементов.
    /// </summary>
    public Dictionary<string, int> Classandelement { get; set; }
    /// <summary>
    /// Возвращает текстовое описание результатов анализа
    /// </summary>
    public string Summary()
      {
        return $"Точность модели: {this.Accuracy:P2}\n" +
               " Распределение классов:\n" +
               string.Join("\n", this.Classandelement.Select(kv => $" Класс {kv.Key}: {kv.Value} элементов"));
      }
    }
  }