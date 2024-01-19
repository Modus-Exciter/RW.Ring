using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Notung.Data
{
  public class PriorityQueue<TElement, TPriority>
  {
    const int ARITY = 4;
    const int LOG2ARITY = 2;
    const int START_GLOBAL_COUNT = 2;
    const int DOMAIN_SIZE = 15;

    private readonly IComparer<TPriority> m_comparer;
    private readonly (TElement element, TPriority priority)[][] m_domain;
    private int m_last_domain_index = 0;
    private int m_count = 0;
    private int m_last_local_index = -1;
    private int m_last_size = 1;

    public int Count { get { return m_count; } }

    public PriorityQueue(IEnumerable<(TElement element, TPriority priority)> values = null, IComparer<TPriority> comparer = null)
    {
      if (comparer == null)
      {
        if (typeof(TPriority).IsValueType)
          m_comparer = Comparer<TPriority>.Default;
        else
          throw new ArgumentException();
      }
      else
        m_comparer = comparer;

      m_domain = new (TElement element, TPriority priority)[DOMAIN_SIZE][];
      m_domain[m_last_domain_index] = new (TElement, TPriority)[1];

      if (values != null)
        foreach (var item in values)
          this.Enqueue(item.element, item.priority);
    }

    public void Enqueue(IEnumerable<(TElement element, TPriority priority)> values)
    {
      foreach (var item in values)
        this.Enqueue(item.element, item.priority);
    }

    public void Enqueue(TElement element, TPriority priority)
    {
      if (m_last_local_index == m_last_size - 1)
      {
        m_last_size = m_last_size == 0 ? 1 : m_last_size << LOG2ARITY;
        m_last_domain_index++;
        m_last_local_index = -1;
        m_domain[m_last_domain_index] = new (TElement, TPriority)[m_last_size];
      }
      m_last_local_index++;
      m_count++;
      var level = m_last_domain_index;
      int index = m_last_local_index;
      (TElement element, TPriority priority) newNode = (element, priority);
      while (level > 0)
      {
        int parent = index >> LOG2ARITY;
        var parentLevel = level - 1;
        if (m_comparer.Compare(newNode.priority, m_domain[parentLevel][parent].priority) >= 0)
          break;
        m_domain[level][index] = m_domain[parentLevel][parent];
        index = parent;
        level = parentLevel;
      }
      m_domain[level][index] = newNode;
    }

    public TElement Dequeue()
    {
      TElement result = m_domain[0][0].element;
      var node = m_domain[m_last_domain_index][m_last_local_index];
      m_count--;
      m_last_local_index--;
      if (m_last_local_index == -1)
      {
        m_domain[m_last_domain_index] = null;
        m_last_size >>= LOG2ARITY;
        m_last_local_index = m_last_size - 1;
        m_last_domain_index--;
      }
      if (m_last_domain_index >= 0)
      {
        int index = 0;
        int minChild = 0;
        int levelCount = 0;
        int globalCount = START_GLOBAL_COUNT;
        var level = 0;
        var childLevel = 1;
        while (globalCount + minChild <= m_count)
        {
          int nextChild = minChild + 1;
          int childThreshold = minChild + ARITY <= m_count - globalCount + 1 ? minChild + ARITY : m_count - globalCount + 1;
          while (nextChild < childThreshold)
          {
            if (m_comparer.Compare(m_domain[childLevel][nextChild].priority, m_domain[childLevel][minChild].priority) <= 0)
              minChild = nextChild;
            nextChild++;
          }

          if (m_comparer.Compare(node.priority, m_domain[childLevel][minChild].priority) <= 0)
            break;

          m_domain[level][index] = m_domain[childLevel][minChild];
          index = minChild;
          minChild = index << LOG2ARITY;
          level++;
          levelCount++;
          childLevel++;
          globalCount = globalCount + (1 << LOG2ARITY * levelCount);
        }
        m_domain[level][index] = node;
      }
      return result;
    }

    public TElement Peek()
    {
      return m_domain[0][0].element;
    }
  }
}
