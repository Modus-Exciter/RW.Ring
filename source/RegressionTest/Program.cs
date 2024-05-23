using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWDataFit;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using Schicksal.Regression;
using Schicksal.Basic;
using System.Runtime.CompilerServices;

namespace RegressionTest
{
  internal class Program
  {/*
    #region Logistic
    const string FILE_PATH = "D:\\AWork\\НИИ\\TestData\\mine\\logisticSynth.xls";
    static IDataGroup _x;
    static IDataGroup _yn;
    static IDataGroup _yp;
    static IDataGroup _yz;

    static void Main(string[] args)
    {
      ExcelConnection excel = new ExcelConnection(FILE_PATH);
      object[,] data = excel.GetRange("A1", "D201");

      double[] x, yn, yp, yz;
      x = new double[data.GetLength(0) - 1];
      yp = new double[data.GetLength(0) - 1];
      yn = new double[data.GetLength(0) - 1];
      yz = new double[data.GetLength(0) - 1];
      for (int i = 0; i < data.GetLength(0) - 1; i++)
      {
        x[i] = (double)data[i + 2, 1];
        yp[i] = (double)data[i + 2, 2];
        yn[i] = (double)data[i + 2, 3];
        yz[i] = (double)data[i + 2, 4];
      }
      _x = new ArrayDataGroup(x);
      _yp = new ArrayDataGroup(yn);
      _yn = new ArrayDataGroup(yp);
      _yz = new ArrayDataGroup(yz);

      var dataFit = new DataFit();
      var startPoint = new MWNumericArray(new double[] { 0, 2, 0 });
      var functionContainer = new MWObjectArray(new FunctionContainer(MathFunction.Logistic));
      
      var mwResult = ((MWNumericArray)dataFit.Optimize(new MWNumericArray(x), new MWNumericArray(yz), functionContainer, startPoint)).
        ToArray().Cast<double>().ToArray();
      
      var dependency = new LogisticDependency(_x, _yz);

      var comparison = new ModelComparison(MathFunction.Logistic, new ArrayDataGroup(mwResult), dependency.Coefs, _x, _yz);
    }
    #endregion*/
    #region Michaelis
    const string FILE_PATH = "D:\\AWork\\НИИ\\TestData\\mine\\michaelisSynth.xls";
    static IDataGroup _x;
    static IDataGroup _yn;
    static IDataGroup _yp;
    static IDataGroup _yz;

    static void Main(string[] args)
    {
      ExcelConnection excel = new ExcelConnection(FILE_PATH);
      object[,] data = excel.GetRange("A1", "D201");

      double[] x, yn, yp, yz;
      x = new double[data.GetLength(0) - 1];
      yp = new double[data.GetLength(0) - 1];
      yn = new double[data.GetLength(0) - 1];
      yz = new double[data.GetLength(0) - 1];
      for (int i = 0; i < data.GetLength(0) - 1; i++)
      {
        x[i] = (double)data[i + 2, 1];
        yp[i] = (double)data[i + 2, 2];
        yn[i] = (double)data[i + 2, 3];
        yz[i] = (double)data[i + 2, 4];
      }
      _x = new ArrayDataGroup(x);
      _yp = new ArrayDataGroup(yn);
      _yn = new ArrayDataGroup(yp);
      _yz = new ArrayDataGroup(yz);

      var dataFit = new DataFit();
      var startPoint = new MWNumericArray(new double[] { 0, 2, 0 });

      //var dependencyOld = new MichaelisDependency(_x, _yn);
      //var dependency = new LikehoodMichaelisDependency(_x, _yn);

      //var comparison = new ModelComparison(MathFunction.Michaelis, dependencyOld.Coefs, dependency.Coefs, _x, _yn);
    }
    #endregion
  }
}
