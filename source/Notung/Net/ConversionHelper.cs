using System;
using System.Collections.Generic;
using System.ComponentModel;
using Notung.Threading;

namespace Notung.Net
{
  internal static class ConversionHelper
  {
    private static readonly Dictionary<Type, bool> _can_convert = new Dictionary<Type, bool>();
    private static readonly SharedLock _lock = new SharedLock(false);

    public static Type GetResponseType(Type returnType)
    {
      if (returnType == typeof(void))
        returnType = typeof(string);

      return typeof(CallResult<>).MakeGenericType(returnType);
    }

    public static bool CanConvert(Type parametersType)
    {
      if (!typeof(IParametersList).IsAssignableFrom(parametersType))
        throw new ArgumentOutOfRangeException();

      if (!parametersType.IsGenericType)
        return parametersType == typeof(ParametersList);

      using (_lock.ReadLock())
      {
        bool result;

        if (_can_convert.TryGetValue(parametersType, out result))
          return result;
      }

      using (_lock.WriteLock())
      {
        bool result;

        if (!_can_convert.TryGetValue(parametersType, out result))
        {
          result = true;

          foreach (Type type in parametersType.GetGenericArguments())
          {
            var converter = TypeDescriptor.GetConverter(type);

            if (!converter.CanConvertFrom(typeof(string)))
            {
              result = false;
              break;
            }
          }

          _can_convert[parametersType] = result;
        }

        return result;
      }
    }
  }
}
