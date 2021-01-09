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
  public class WeakSet<T> : IEnumerable<T> where T : class
  {
    private int m_count = 0;
    private int m_free_index = -1;
    private int m_last_index = 0;
    private Slot[] m_slots;
    private readonly SharedLock m_lock;

    private static readonly DisposableStub _fake_lock = new DisposableStub();

    /// <summary>
    /// Инициализация нового экземпляра множества удаляемых элементов
    /// </summary>
    /// <param name="synchronize">Следует ли синхронизировать доступ к элементам множества</param>
    public WeakSet(bool synchronize = true)
    {
      if (synchronize)
        m_lock = new SharedLock(true);
    }

    /// <summary>
    /// Добавление нового элемента в множество
    /// </summary>
    /// <param name="item">Добавляемый элемент</param>
    /// <returns>True, если элемент был добавлен в множество. False, если такой элемент уже был в множестве</returns>
    public bool Add(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      using (this.WriteLock())
      {
        if (m_slots == null)
          m_slots = new Slot[3];

        int previous;
        int bucket_index;
        int slot_index = this.FindSlotIndex(item, out bucket_index, out previous, true);

        if (slot_index >= 0)
          return false;

        this.AddSlot(item, bucket_index, slot_index);

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

      using (this.ReadLock())
      {
        return this.FindSlotIndex(item, out index, out previous, false) >= 0;
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
      int bucket_index;

      using (this.WriteLock())
      {
        var slot_index = this.FindSlotIndex(item, out bucket_index, out previous, true);

        if (slot_index < 0)
          return false;
        else
        {
          this.RemoveSlot(bucket_index, slot_index, previous);

          return true;
        }
      }
    }

    /// <summary>
    /// Удаление всех элементов множества
    /// </summary>
    public void Clear()
    {
      using (this.WriteLock())
      {
        for (int i = 0; i < m_slots.Length; i++)
        {
          if (m_slots[i].handle.IsAllocated)
            m_slots[i].handle.Free();
        }

        m_slots = null;
        m_count = 0;
        m_free_index = -1;
        m_last_index = 0;
      }
    }

    /// <summary>
    /// Удаление избытка резерва памяти, используемого множеством
    /// </summary>
    public void TrimExcess()
    {
      using (this.WriteLock())
      {
        var list = new List<T>(m_count);

        for (int i = 0; i < m_last_index; i++)
        {
          var item = m_slots[i].Target;

          if (item != null)
            list.Add(item);
        }

        this.Clear();

        if (list.Count > 0)
        {
          m_slots = new Slot[PrimeHelper.GetPrime(list.Count)];

          foreach (var item in list)
            this.Add(item);
        }
      }
    }

    /// <summary>
    /// Обход всех элементов множества, которые ещё не были удалены сборщиком мусора
    /// </summary>
    /// <returns>Элементы множества, ещё активные в памяти</returns>
    public IEnumerator<T> GetEnumerator()
    {
      using (this.ReadLock())
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

    private IDisposable ReadLock()
    {
      if (m_lock != null)
        return m_lock.ReadLock();
      else
        return _fake_lock;
    }

    private IDisposable WriteLock()
    {
      if (m_lock != null)
        return m_lock.WriteLock();
      else
        return _fake_lock;
    }

    private void IncreaseCapacity()
    {
      int min = this.m_count * 2 + 1;
      int prime = PrimeHelper.GetPrime(min < 0 ? m_count : min);
      Slot[] tmp_slots = new Slot[prime];

      if (m_slots != null)
        Array.Copy(m_slots, 0, tmp_slots, 0, m_last_index);

      for (int i = 0; i < m_last_index; i++)
      {
        int index = (tmp_slots[i].hashCode & int.MaxValue) % prime;
        tmp_slots[i].next = tmp_slots[index].bucket - 1;
        tmp_slots[index].bucket = i + 1;
      }

      m_slots = tmp_slots;
    }

    private int FindSlotIndex(T item, out int bucketIndex, out int previous, bool removeExpired)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      var hash_code = item.GetHashCode();

      previous = -1;
      bucketIndex = (hash_code & int.MaxValue) % m_slots.Length;

      for (int i = m_slots[bucketIndex].bucket - 1; i >= 0; i = m_slots[i].next)
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
            this.RemoveSlot(bucketIndex, i, previous);
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
        if (m_last_index == m_slots.Length)
        {
          this.IncreaseCapacity();
          bucketIndex = (hash_code & int.MaxValue) % m_slots.Length;
        }

        slotIndex = m_last_index;
        m_last_index++;
      }

      m_slots[slotIndex].hashCode = hash_code;
      m_slots[slotIndex].handle = GCHandle.Alloc(item, GCHandleType.Weak);
      m_slots[slotIndex].next = m_slots[bucketIndex].bucket - 1;
      m_slots[bucketIndex].bucket = slotIndex + 1;
      m_count++;
    }

    private void RemoveSlot(int bucketIndex, int slotIndex, int previous)
    {
      if (m_slots[slotIndex].handle.IsAllocated)
        m_slots[slotIndex].handle.Free();

      if (previous < 0)
        m_slots[bucketIndex].bucket = m_slots[slotIndex].next + 1;
      else
        m_slots[previous].next = m_slots[slotIndex].next;

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

    private sealed class DisposableStub : IDisposable
    {
      public void Dispose() { }
    }

    private struct Slot
    {
      public int hashCode;
      public int next;
      public int bucket;
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
      if (value < 4)
        return value > 1;
      else if (value % 2 == 0 || value % 3 == 0)
        return false;

      var sqrt = (int)Math.Sqrt(value);

      for (int i = 5; i <= sqrt; i += 6)
      {
        if (value % i == 0 || value % (i + 2) == 0)
          return false;
      }

      return true;
    }

    public static int GetPrime(int value)
    {
      if (value <= 3)
        return 3;

      if (value % 2 == 0)
        value++;

      if (value % 3 == 0)
        value += 2;

      if (!IsPrime(value))
      {
        bool even = false;

        if (value % 6 == 5)
        {
          value += 2;
          even = true;
        }
        else
          value = value + 5 - (value % 6);

        while (!IsPrime(value))
        {
          if (value > int.MaxValue - 6)
            return int.MaxValue;
          
          value += even ? 4 : 2;
          even = !even;
        }
      }

      return value;
    }
  }
}