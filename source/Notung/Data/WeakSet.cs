using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Notung.Threading;

namespace Notung.Data
{
  /// <summary>
  /// Множество элементов, которые могут быть удалены сборщиком мусора
  /// </summary>
  /// <typeparam name="T">Тип элемента множества</typeparam>
  public class WeakSet <T> : IEnumerable<T> where T : class
  {
    private int m_count = 0;
    private int m_free_index = -1;
    private int m_last_index = 0;
    private int[] m_buckets;
    private Slot[] m_slots;
    private readonly SharedLock m_lock = new SharedLock(false);

    /// <summary>
    /// Добавление нового элемента в множество
    /// </summary>
    /// <param name="item">Добавляемый элемент</param>
    public bool Add(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      using (m_lock.WriteLock())
      {
        if (m_buckets == null)
          this.Initialize(0);

        int previous;
        int index;
        int slot_index = this.FindIndex(item, out index, out previous, true);

        if (slot_index >= 0)
          return false;

        this.AddSlot(item, index, slot_index);
        return true;
      }
    }

    /// <summary>
    /// Проверка наличия элемента в множестве
    /// </summary>
    /// <param name="item">Элемент, наличие которого проверяется</param>
    /// <returns>True, если элемент присутствует в множестве. Иначе, false</returns>
    public bool Contains(T item)
    {
      int previous;
      int index;

      using (m_lock.ReadLock())
      {
        return this.FindIndex(item, out index, out previous, false) >= 0;
      }
    }

    /// <summary>
    /// Удаление элемента из множества
    /// </summary>
    /// <param name="item">Удаляемый элемент</param>
    /// <returns>True, если элемент был удалён из множества. False, если элемента не было в множестве</returns>
    public bool Remove(T item)
    {
      int previous;
      int index;

      using (m_lock.WriteLock())
      {
        var slot = this.FindIndex(item, out index, out previous, true);

        if (slot < 0)
          return false;
        else
        {
          this.RemoveSlot(index, slot, previous);
          return true;
        }
      }
    }

    /// <summary>
    /// Удаление всех элементов множества
    /// </summary>
    public void Clear()
    {
      using (m_lock.WriteLock())
      {
        for (int i = 0; i < m_slots.Length; i++)
        {
          if (m_slots[i].handle.IsAllocated)
            m_slots[i].handle.Free();
        }

        m_slots = null;
        m_buckets = null;
        m_count = 0;
        m_free_index = -1;
        m_last_index = 0;
      }
    }

    /// <summary>
    /// Обход всех элементов множества, которые ещё не были удалены сборщиком мусора
    /// </summary>
    /// <returns>Элементы множества, ещё активные в памяти</returns>
    public IEnumerator<T> GetEnumerator()
    {
      using (m_lock.ReadLock())
      {
        for (int i = 0; i < m_count; i++)
        {
          var tg = m_slots[i].Target;

          if (tg != null)
            yield return tg;
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #region Implementation ------------------------------------------------------------------------

    private void Initialize(int capacity)
    {
      int prime = PrimeHelper.GetPrime(capacity);

      m_buckets = new int[prime];
      m_slots = new Slot[prime];
    }

    private void IncreaseCapacity()
    {
      int min = this.m_count * 2;
      int prime = PrimeHelper.GetPrime(min < 0 ? m_count : min);
      Slot[] tmp_slots = new Slot[prime];
      int[] tmp_buckets = new int[prime];

      if (m_slots != null)
        Array.Copy(m_slots, 0, tmp_slots, 0, m_last_index);

      for (int i = 0; i < m_last_index; i++)
      {
        int index = (tmp_slots[i].hashCode & int.MaxValue) % prime;
        tmp_slots[i].next = tmp_buckets[index] - 1;
        tmp_buckets[index] = i + 1;
      }

      m_slots = tmp_slots;
      m_buckets = tmp_buckets;
    }

    private int FindIndex(T item, out int index, out int previous, bool removeExpired)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      var hash_code = item.GetHashCode() ;

      previous = -1;
      index = (hash_code & int.MaxValue) % m_buckets.Length;

      for (int i = this.m_buckets[index] - 1; i >= 0; i = this.m_slots[i].next)
      {
        if (m_slots[i].hashCode == hash_code)
        {
          var target = m_slots[i].Target;

          if (target != null)
          {
            if (target.Equals(item))
              return i;
          }
          else if (removeExpired)
            this.RemoveSlot(index, i, previous);
        }

        previous = i;
      }

      return -1;
    }

    private void AddSlot(T item, int bucketIndex, int slotIndex)
    {
      var hash_code = item.GetHashCode();

      if (m_free_index >= 0)
      {
        slotIndex = m_free_index;
        m_free_index = m_slots[slotIndex].next;
      }
      else
      {
        if (m_last_index == this.m_slots.Length)
        {
          this.IncreaseCapacity();
          bucketIndex = (hash_code & int.MaxValue) % m_buckets.Length;
        }

        slotIndex = m_last_index;
        m_last_index++;
      }

      m_slots[slotIndex].hashCode = hash_code;
      m_slots[slotIndex].handle = GCHandle.Alloc(item, GCHandleType.Weak);
      m_slots[slotIndex].next = m_buckets[bucketIndex] - 1;
      m_buckets[bucketIndex] = slotIndex + 1;
      m_count++;
    }

    private void RemoveSlot(int bucketIndex, int slotIndex, int previous)
    {
      if (m_slots[slotIndex].handle.IsAllocated)
        m_slots[slotIndex].handle.Free();

      if (previous < 0)
        m_buckets[bucketIndex] = m_slots[slotIndex].next + 1;
      else
        m_slots[previous].next = this.m_slots[slotIndex].next;

      m_slots[slotIndex].hashCode = -1;
      m_slots[slotIndex].handle = default(GCHandle);
      m_slots[slotIndex].next = m_free_index;
      m_count--;

      if (m_count == 0)
      {
        m_last_index = 0;
        m_free_index = -1;
      }
      else
        m_free_index = slotIndex;
    }

    private struct Slot
    {
      public int hashCode;
      public int next;
      public GCHandle handle;

      public T Target
      {
        get { return handle.IsAllocated ? (T)handle.Target : null; }
      }
    }

    #endregion
  }

  internal class PrimeHelper
  {
    public static bool IsPrime(int value)
    {
      for (int i = 2; i <= value / i; i++)
      {
        if (value % i == 0)
          return false;
      }

      return true;
    }

    public static int GetPrime(int value)
    {
      if (value < 3)
        return 3;

      if (value % 2 == 0)
        value++;

      if (value % 3 == 0)
        value += 2;

      if (!IsPrime(value))
      {
        value = value + 5 - (value % 6);

        bool even = false;

        while (!IsPrime(value))
        {
          value += even ? 4 : 2;
          even = !even;
        }
      }
      
      return value;
    }
  }
}