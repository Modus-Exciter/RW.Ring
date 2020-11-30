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

  public class FastBitArray : MemoryBlock
  {
    public readonly int Length;
    public FastBitArray(int length)
      : base(GetSize(length))
    {
      this.Length = length;
    }

    private static int GetSize(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException("length");

      return ((length - 1) / 0x20) + 1;
    }

    public unsafe bool this[int index]
    {
      get
      {
        return (((int*)this.Pointer)[index / 0x20] & (1 << (index % 0x20))) != 0;
      }
      set
      {
        if (value)
        {
          ((int*)this.Pointer)[index / 0x20] |= (1 << (index % 0x20));
        }
        else
        {
          ((int*)this.Pointer)[index / 0x20] &= ~(1 << (index % 0x20));
        }
      }
    }
  }
}
