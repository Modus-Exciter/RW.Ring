using Notung.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Schicksal.Optimization.MathOptimization.Domain;

namespace Schicksal.Optimization
{
  public static partial class MathOptimization
  {
    [DebuggerDisplay("{ToString()}")]
    public class Domain : IEnumerable<Node>
    {
      Node[] m_domain;
      Node m_first;

      public Node this[int index] => m_domain[index];

      public Node Min { get => this.FindMin(); }

      public Domain(int size, Rectangle rectangle)
      {
        m_domain = new Node[size];
        for (int i = 0; i < size; i++)
          m_domain[i] = new Node(i);
        m_domain[0].Value.Enqueue(rectangle, rectangle.F);
        m_first = m_domain[0];
      }

      public Rectangle Exchange(Node parentNode, Rectangle[] children)
      {
        int i = parentNode.Index;
        int j = 0;
        while (i < parentNode.Index + children.Length / 2 && i < m_domain.Length)
        {
          if (m_domain[i].Right != m_domain[i + 1])
          {
            m_domain[i + 1].Right = m_domain[i].Right;
            m_domain[i + 1].Left = m_domain[i];
            if (m_domain[i].Right != null)
              m_domain[i].Right.Left = m_domain[i + 1];
            m_domain[i].Right = m_domain[i + 1];
          }
          i++;
          m_domain[i].Value.Enqueue(children[j], children[j].F); j++;
          m_domain[i].Value.Enqueue(children[j], children[j].F); j++;
        }
        m_domain[i].Value.Enqueue(children[j], children[j].F);

        if (parentNode.Value.Count == 1)
        {
          if (parentNode.Left == null)
            m_first = parentNode.Right;
          else
            parentNode.Left.Right = parentNode.Right;
          parentNode.Right.Left = parentNode.Left;
          parentNode.Left = null;
          parentNode.Right = null;
        }

        return parentNode.Value.Dequeue();
      }

      private Node FindMin()
      {
        Node min = m_first;
        foreach (Node node in this)
          if (node.Value.Peek().F <= m_first.Value.Peek().F)
            min = node;
        return min;
      }

      [DebuggerDisplay("{ToString()}")]
      public class Node
      {
        Node m_left;
        Node m_right;
        int m_index;

        readonly PriorityQueue<Rectangle, double> m_value = new PriorityQueue<Rectangle, double>();

        public Node Left { get { return m_left; } set { m_left = value; } }
        public Node Right { get { return m_right; } set { m_right = value; } }
        public int Index { get { return m_index; } }
        public PriorityQueue<Rectangle, double> Value { get { return m_value; } }

        public Node(int index, Node left = null, Node right = null)
        {
          m_index = index;
          m_left = left;
          m_right = right;
        }
#if DEBUG
        public override string ToString()
        {
          int left = m_left != null ? m_left.Index : -1;
          int right = m_right != null ? m_right.Index : -1;
          return string.Concat(left, " ", m_index, " ", right);
        }
#endif
      }

      public IEnumerator<Node> GetEnumerator()
      {
        Node current = m_first;
        Node result;
        while (current != null)
        {
          result = current;
          current = current.Right;
          yield return result;
        }
        yield break;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

#if DEBUG
      public override string ToString()
      {
        string result = "";
        for (int i = 0; i < m_domain.Length; i++)
          result += m_domain[i].Value.Count + " ";
        return result;
      }
#endif
    }
  }
}
