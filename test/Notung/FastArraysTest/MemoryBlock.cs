using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FastArraysTest
{
  public unsafe class MemoryBlock : IDisposable
  {
    public readonly void* Pointer;
    public readonly int Size;

    public MemoryBlock(int size)
    {
      this.Pointer = Marshal.AllocHGlobal(size).ToPointer();

      if (Pointer != null)
        this.Size = size;
    }

    ~MemoryBlock()
    {
      this.Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (Pointer != null)
        Marshal.FreeHGlobal(new IntPtr(this.Pointer));
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }
  }

  public unsafe class FastBitArray : MemoryBlock
  {
    public readonly int Length;
    private readonly int* m_data;
    public FastBitArray(int length)
      : base(GetSize(length))
    {
      this.Length = length;
      m_data = (int*)this.Pointer;
    }

    private static int GetSize(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      return ((length - 1) / 0x20) + 1;
    }

    public bool this[int index]
    {
      get
      {
        return (m_data[index / 0x20] & (1 << (index % 0x20))) != 0;
      }
      set
      {
        if (value)
        {
          m_data[index / 0x20] |= (1 << (index % 0x20));
        }
        else
        {
          m_data[index / 0x20] &= ~(1 << (index % 0x20));
        }
      }
    }
  }

    public class SimpleBitArray
    {
      private readonly int[] m_array;
      public readonly int Length;
      private int _version;

      private static int GetSize(int length)
      {
        if (length < 0)
          throw new ArgumentOutOfRangeException("length");

        return ((length - 1) / 0x20) + 1;
      }

      public SimpleBitArray(int length)
      {
        m_array = new int[GetSize(length)];
        this.Length = length;
      }

      public bool Get(int index)
      {
        if ((index < 0) || (index >= this.Length))
        {
          throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
        }
        return ((this.m_array[index / 0x20] & (((int)1) << (index % 0x20))) != 0);
      }

      public void Set(int index, bool value)
      {
        if ((index < 0) || (index >= this.Length))
        {
          throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
        }
        if (value)
        {
          this.m_array[index / 0x20] |= ((int)1) << (index % 0x20);
        }
        else
        {
          this.m_array[index / 0x20] &= ~(((int)1) << (index % 0x20));
        }
        this._version++;
      }

      public bool this[int index]
      {
        get
        {
          return this.Get(index);
        }
        set
        {
          this.Set(index, value);
        }
      }
  }
}
