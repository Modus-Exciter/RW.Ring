using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Notung.Data
{
  public static class ArrayExtensions
  {
    /// <summary>
    /// Получение пустого массива указанного типа
    /// </summary>
    public static T[] Empty<T>()
    {
      return EmptyImpl<T>.Instance;
    }

    private static class EmptyImpl<T>
    {
      public static readonly T[] Instance = (Enumerable.Empty<T>() as T[]) ?? new T[0];
    }

    public static void Fill<T>(this T[] array, Func<T> filler)
    {
      if (filler == null)
        return;

      for (int i = 0; i < array.Length; i++)
        array[i] = filler();
    }

    public static void Fill<T>(this T[] array, Func<int, T> filler)
    {
      if (filler == null)
        return;

      for (int i = 0; i < array.Length; i++)
        array[i] = filler(i);
    }

    public static void Fill<T>(this T[] array, T value)
    {
      if (array.Length == 0)
        return;

      array[0] = value;
      int half = array.Length / 2;
      int count;

      if (typeof(T).IsPrimitive)
      {
        int size;

        if (typeof(T) == typeof(char))
          size = 2;
        else if (typeof(T) == typeof(bool))
          size = 1;
        else
          size = Marshal.SizeOf(typeof(T));

        for (count = 1; count <= half; count <<= 1)
          Buffer.BlockCopy(array, 0, array, count * size, count * size);

        Buffer.BlockCopy(array, 0, array, count * size, (array.Length - count) * size);
      }
      else
      {
        for (count = 1; count <= half; count <<= 1)
          Array.Copy(array, 0, array, count, count);

        Array.Copy(array, 0, array, count, array.Length - count);
      }
    }

    public static T[] CreateAndFill<T>(int length, Func<T> filler)
    {
      var array = new T[length];

      if (filler != null)
      {
        for (int i = 0; i < array.Length; i++)
          array[i] = filler();
      }

      return array;
    }

    public static T[] CreateAndFill<T>(int length, Func<int, T> filler)
    {
      var array = new T[length];

      if (filler != null)
      {
        for (int i = 0; i < array.Length; i++)
          array[i] = filler(i);
      }

      return array;
    }
  }
}