using System;
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
    private const int POWER = 5; // показатель двойки для 32 - размер int в битах
    public readonly int Length;
    private readonly int* m_array;
    public FastBitArray(int length)
      : base(GetSize(length))
    {
      this.Length = length;
      m_array = (int*)this.Pointer;
    }

    private static int GetSize(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      return (((length - 1) >> POWER) + 1) << (POWER - 1);
    }

    public bool this[int index]
    {
      get
      {
        return (m_array[index >> POWER] & (1 << index)) != 0;
      }
      set
      {
        if (value)
          m_array[index >> POWER] |= 1 << index;
        else
          m_array[index >> POWER] &= ~(1 << index);
      }
    }
  }

  public struct SimpleBitArray
  {
    private const int POWER = 5; // показатель двойки для 32 - размер int в битах
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
      _version = 0;
    }

    public bool Get(int index)
    {
      return (m_array[index >> POWER] & (1 << index)) != 0;
    }

    public void Set(int index, bool value)
    {
      if (value)
        m_array[index >> POWER] |= 1 << index;
      else
        m_array[index >> POWER] &= ~(1 << index);
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