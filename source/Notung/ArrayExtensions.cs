
namespace Notung
{
  public static class ArrayExtensions
  {
    public static T[] Empty<T>()
    {
      return EmptyImpl<T>.Instance;
    }
    
    private static class EmptyImpl<T>
    {
      public static readonly T[] Instance = new T[0];
    }
  }
}
