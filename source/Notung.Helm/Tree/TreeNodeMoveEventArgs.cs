using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Notung.Helm.Tree
{
  /// <summary>
  /// Событие проверки допустимости перемещения одного узла дерева в другой
  /// </summary>
  public class TreeNodeMoveEventArgs : CancelEventArgs
  {
    internal TreeNodeMoveEventArgs(TreeNode source, TreeNode destination)
      : base(true)
    {
      this.Source = source;
      this.Destination = destination;
    }

    /// <summary>
    /// Исходный узел
    /// </summary>
    public TreeNode Source { get; private set; }

    /// <summary>
    /// Конечный узел
    /// </summary>
    public TreeNode Destination { get; private set; }
  }

  /// <summary>
  /// Событие, позволяющее затребовать изображение
  /// </summary>
  public class ImageIdxEventArgs : EventArgs
  {
    internal ImageIdxEventArgs(object tag)
    {
      this.Tag = tag;
      this.ImageIdx = -1;
    }
    /// <summary>
    /// Затребованное изображение
    /// </summary>
    public int ImageIdx { get; set; }

    /// <summary>
    /// Связанный с узлом объект
    /// </summary>
    public object Tag { get; private set; }
  }
}
