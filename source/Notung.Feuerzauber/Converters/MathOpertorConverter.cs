using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Notung.Feuerzauber.Converters
{
  public class MathOpertorConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (parameter == null)
        throw new ArgumentNullException("ConverterParameter");

      if (targetType == null)
        throw new ArgumentNullException("targetType");

      if (values == null)
        return null;

      if (values.Length != 2)
        return Aggregate(values, targetType, parameter);

      if (targetType == typeof(object))
        return ProcessDouble(values, parameter) ?? ProcessInt(values, parameter, false);
      else if (targetType == typeof(double)
        || targetType == typeof(float)
        || targetType == typeof(decimal))
      {
        return System.Convert.ChangeType(
          ProcessDouble(values, parameter) ??
          ProcessInt(values, parameter, false), targetType);
      }
      else if (values[0] is bool && values[1] is bool)
        return System.Convert.ChangeType(ProcessBoolean(values, parameter), targetType);
      else if (targetType.IsPrimitive)
        return System.Convert.ChangeType(ProcessInt(values, parameter, true), targetType);
      else if (targetType == typeof(string) && "+".Equals(parameter))
        return string.Format("{0}{1}", values);

      return null;
    }

    private static object Aggregate(object[] values, Type targetType, object parameter)
    {
      switch (parameter.ToString())
      {
        case "+":
          return Sum(values, targetType);
        case "*":
          return Mul(values, targetType);
        case "min":
        case "Min":
        case "MIN":
          return Min(values, targetType);
        case "max":
        case "Max":
        case "MAX":
          return Max(values, targetType);

        default:
          return null;
      }
    }

    private static object Sum(object[] values, Type targetType)
    {
      if (targetType == typeof(double)
      || targetType == typeof(float)
      || targetType == typeof(decimal))
      {
        return System.Convert.ChangeType(values.Select(
          System.Convert.ToDouble).Sum(), targetType);
      }
      else if (targetType.IsPrimitive && targetType != typeof(bool))
      {
        return System.Convert.ChangeType(values.Select(
         System.Convert.ToInt64).Sum(), targetType);
      }
      else if (targetType == typeof(string))
        return string.Join(string.Empty, values);
      else
        return null;
    }

    private static object Mul(object[] values, Type targetType)
    {
      if (targetType == typeof(double)
      || targetType == typeof(float)
      || targetType == typeof(decimal))
      {
        double mul = 1;

        foreach (var value in values.Select(System.Convert.ToDouble))
          mul *= value;

        return System.Convert.ChangeType(mul, targetType);
      }
      else if (targetType.IsPrimitive && targetType != typeof(bool))
      {
        long mul = 1;

        foreach (var value in values.Select(System.Convert.ToInt64))
          mul *= value;

        return System.Convert.ChangeType(mul, targetType);
      }
      else
        return null;
    }

    private static object Min(object[] values, Type targetType)
    {
      if (targetType == typeof(double)
      || targetType == typeof(float)
      || targetType == typeof(decimal))
      {
        return System.Convert.ChangeType(values.Select(
          System.Convert.ToDouble).Min(), targetType);
      }
      else if (targetType.IsPrimitive && targetType != typeof(bool))
      {
        return System.Convert.ChangeType(values.Select(
         System.Convert.ToInt64).Min(), targetType);
      }
      else if (targetType == typeof(string))
        return values.Select(v => (v ?? "").ToString()).Min();
      else
        return null;
    }

    private static object Max(object[] values, Type targetType)
    {
      if (targetType == typeof(double)
      || targetType == typeof(float)
      || targetType == typeof(decimal))
      {
        return System.Convert.ChangeType(values.Select(
          System.Convert.ToDouble).Max(), targetType);
      }
      else if (targetType.IsPrimitive && targetType != typeof(bool))
      {
        return System.Convert.ChangeType(values.Select(
         System.Convert.ToInt64).Max(), targetType);
      }
      else if (targetType == typeof(string))
        return values.Select(v => (v ?? "").ToString()).Max();
      else
        return null;
    }

    private static object ProcessBoolean(object[] values, object parameter)
    {
      bool v1 = System.Convert.ToBoolean(values[0]);
      bool v2 = System.Convert.ToBoolean(values[1]);

      switch (parameter.ToString())
      {
        case "&":
        case "&&":
          return v1 && v2;
        case "|":
        case "||":
          return v1 || v2;
      }

      return null;
    }

    private static object ProcessInt(object[] values, object parameter, bool throwOnParseError)
    {
      long v1, v2;

      try
      {
        v1 = System.Convert.ToInt64(values[0]);
        v2 = System.Convert.ToInt64(values[1]);
      }
      catch
      {
        if (throwOnParseError)
          throw;
        else
          return null;
      }

      switch (parameter.ToString())
      {
        case "<":
          return v1 < v2;
        case ">":
          return v1 > v2;
        case "=":
        case "==":
          return v1 == v2;
        case "<=":
          return v1 <= v2;
        case ">=":
          return v1 >= v2;
        case "!=":
          return v1 != v2;
        case "+":
          return v1 + v2;
        case "-":
          return v1 - v2;
        case "*":
          return v1 * v2;
        case "/":
          return v1 / v2;
        case "%":
          return v1 % v2;
        case "^":
          return v1 ^ v2;
        case "|":
          return v1 | v2;
        case "&":
          return v1 & v2;
        case ">>":
          return v1 >> (int)v2;
        case "<<":
          return v1 << (int)v2;
        case "pow":
        case "Pow":
        case "POW":
          return Math.Pow(v1, v2);
        case "log":
        case "Log":
        case "LOG":
          return Math.Log(v1, v2);
        case "min":
        case "Min":
        case "MIN":
          return Math.Min(v1, v2);
        case "max":
        case "Max":
        case "MAX":
          return Math.Max(v1, v2);
      }

      return null;
    }

    private static object ProcessDouble(object[] values, object parameter)
    {
      double v1 = System.Convert.ToDouble(values[0]);
      double v2 = System.Convert.ToDouble(values[1]);

      switch (parameter.ToString())
      {
        case "<":
          return v1 < v2;
        case ">":
          return v1 > v2;
        case "=":
        case "==":
          return v1 == v2;
        case "<=":
          return v1 <= v2;
        case ">=":
          return v1 >= v2;
        case "!=":
          return v1 != v2;
        case "+":
          return v1 + v2;
        case "-":
          return v1 - v2;
        case "*":
          return v1 * v2;
        case "/":
          return v1 / v2;
        case "%":
          return v1 % v2;
        case "pow":
        case "Pow":
        case "POW":
        case "^":
          return Math.Pow(v1, v2);
        case "log":
        case "Log":
        case "LOG":
          return Math.Log(v1, v2);
        case "min":
        case "Min":
        case "MIN":
          return Math.Min(v1, v2);
        case "max":
        case "Max":
        case "MAX":
          return Math.Max(v1, v2);
      }

      return null;
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
