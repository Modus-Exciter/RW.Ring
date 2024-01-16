using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Data
{
  internal class PriorityQueue<TElement, TPriority>
  {
    const int START_SIZE = 1;

    private IComparer<TPriority> m_comparer;
    private LinkedList<(TElement element, TPriority priority)[]> m_domain;
    private int m_count = 0;
    private int m_capacity = 1;
    private int m_last_size = 1;

    private int LastIndex { get { return m_count - (1 << m_domain.Count - 1); } }

    public PriorityQueue(Comparer<TPriority> comparer = null)
    {
      if(comparer == null)
      {
        if (typeof(TPriority).IsValueType)
          m_comparer = Comparer<TPriority>.Default;
        else
          throw new ArgumentException();
      }
      else
        m_comparer = comparer;

      m_domain = new LinkedList<(TElement, TPriority)[]>();
      m_domain.AddFirst(new (TElement, TPriority)[START_SIZE]);
    }

    public void Enqueue(TElement element, TPriority priority)
    {
      if (m_capacity == m_count)
      {
        m_last_size <<= 1;
        m_capacity += m_last_size;
        m_domain.AddLast(new (TElement, TPriority)[m_last_size]);
      }
      m_count++;
      var currentLevel = m_domain.Last;
      int index = this.LastIndex;
      (TElement element, TPriority priority) newNode = (element, priority);
      while (currentLevel.Previous != null)
      {
        int parentIndex = index >> 1;
        var parentLevel = currentLevel.Previous;
        if (m_comparer.Compare(newNode.priority, parentLevel.Value[parentIndex].priority) < 0)
        {
          currentLevel.Value[index] = parentLevel.Value[parentIndex];
          index = parentIndex;
          currentLevel = parentLevel;
        }
        else break;
      }

      currentLevel.Value[index] = newNode;
    }

    public TElement Dequeue()
    {
      TElement result = m_domain.First.Value[0].element;
      var node = m_domain.Last.Value[this.LastIndex];
      m_count--;
      if (m_capacity - m_count == m_last_size)
      {
        m_capacity -= m_last_size;
        m_last_size >>= 1;
        m_domain.RemoveLast();
      }
      if (m_domain.Count != 0)
      {
        int index = 0;
        var currentLevel = m_domain.First;
        while (currentLevel.Next != null)
        {
          var childLevel = currentLevel.Next;
          int childIndex = index << 1;
          if (childIndex + 1 < m_count)
          {
            if (m_comparer.Compare(childLevel.Value[childIndex].priority, childLevel.Value[childIndex + 1].priority) > 0)
              childIndex = childIndex + 1;
          }

          if (m_comparer.Compare(node.priority, childLevel.Value[childIndex].priority) > 0)
          {
            currentLevel.Value[index] = childLevel.Value[childIndex];
            index = childIndex;
            currentLevel = childLevel;
          }
          else break;
        }
        currentLevel.Value[index] = node;
      }
      return result;
    }
  }
}
