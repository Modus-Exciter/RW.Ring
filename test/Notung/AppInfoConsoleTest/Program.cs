using System;
using Notung;

namespace AppInfoConsoleTest
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine(ApplicationInfo.Instance);

      Console.ReadKey();
    }
  }
}
