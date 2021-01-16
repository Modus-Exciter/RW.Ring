using System;
using System.Collections.Generic;
using System.ComponentModel;
using Notung.Threading;

namespace Notung.Net
{
  internal static class HttpTypeHelper
  {
    private static readonly SharedLock _lock = new SharedLock(false);
    private static readonly Dictionary<Type, bool> _can_convert = new Dictionary<Type, bool>();
    private static readonly Dictionary<Type, HttpConversionHelper> _converters 
      = new Dictionary<Type, HttpConversionHelper>();

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

          var agruments = parametersType.GetGenericArguments();
          var converters = new TypeConverter[agruments.Length];

          for (int i = 0; i < agruments.Length; i++)
          {
            var type = agruments[i];

            if (type.IsByRef)
              type = type.GetElementType();
            
            converters[i] = TypeDescriptor.GetConverter(agruments[i]);

            if (!converters[i].CanConvertFrom(typeof(string)))
            {
              result = false;
              break;
            }
          }

          _can_convert[parametersType] = result;
          _converters[parametersType] = new HttpConversionHelper(converters);
        }

        return result;
      }
    }

    public static HttpConversionHelper GetConverter(Type parametersType)
    {
      using (_lock.ReadLock())
        return _converters[parametersType];
    }
  }

  internal sealed class HttpConversionHelper
  {
    private readonly TypeConverter[] m_converters;

    public HttpConversionHelper(TypeConverter[] converters)
    {
      m_converters = converters;
    }

    public string ConvertToString(object value, int index)
    {
      return Uri.EscapeDataString(m_converters[index].ConvertToInvariantString(value));
    }

    public object ConvertFromString(string value, int index)
    {
      return m_converters[index].ConvertFromInvariantString(Uri.UnescapeDataString(value));
    }
  }
}