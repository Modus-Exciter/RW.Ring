using System;

namespace Notung.Data
{
  public static class ArrayExtensions
  {
    public static T[] Empty<T>()
    {
      return EmptyImpl<T>.Instance;
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
      for (int i = 0; i < array.Length; i++)
        array[i] = value;
    }

    public static T[] CreateAndFill<T>(int length, Func<T> filler)
    {
      T[] array = new T[length];

      if (filler != null)
      {
        for (int i = 0; i < array.Length; i++)
          array[i] = filler();
      }

      return array;
    }

    public static T[] CreateAndFill<T>(int length, Func<int, T> filler)
    {
      T[] array = new T[length];

      if (filler != null)
      {
        for (int i = 0; i < array.Length; i++)
          array[i] = filler(i);
      }

      return array;
    }

    private static class EmptyImpl<T>
    {
      public static readonly T[] Instance = new T[0];
    }
  }
}
