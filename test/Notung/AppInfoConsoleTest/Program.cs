using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
