using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.ComponentModel
{
  /// <summary>
  /// Определяет операции, соответствующие перетаскиванию узла
  /// </summary>
  public interface INodeMoveProvider
  {
    /// <summary>
    /// Проверяет допустимость перетаскивания
    /// </summary>
    /// <param name="source">Перетаскиваемый узел</param>
    /// <param name="destination">Узел, на который перетаскивается</param>
    /// <param name="list">Коллекция, связанная с узлом, на который перетаскивается</param>
    /// <returns>True, если перетаскивание допустимо</returns>
    bool Check(object source, object destination, IList list);

    /// <summary>
    /// Выполняет над моделью операцию, соответствующую перетаскиванию
    /// </summary>
    /// <param name="source">Перетаскиваемый узел</param>
    /// <param name="destination">Узел, на который перетаскивается</param>
    /// <param name="list">Коллекция, связанная с узлом, на который перетаскивается</param>
    /// <returns>Новый узел, возникший после перетаскивания</returns>
    object Perform(object source, object destination, IList list);
  }

  public sealed class NoBindAttribute : Attribute { }
}