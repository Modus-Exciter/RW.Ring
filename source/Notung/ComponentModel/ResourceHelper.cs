using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Notung.ComponentModel
{
  internal static class ResourceHelper
  {
    private static readonly Dictionary<Type, ResourceManager[]> _data = new Dictionary<Type, ResourceManager[]>();
    private static readonly Dictionary<Type, int> _counts = new Dictionary<Type, int>();
    private static readonly Dictionary<Assembly, string[]> _names = new Dictionary<Assembly, string[]>();

    public static ResourceManager[] GetResourceManagers(Type type, out int count)
    {
      ResourceManager[] ret;

      if (_data.TryGetValue(type, out ret))
      {
        count = _counts[type];
        return ret;
      }

      lock (_data)
      {
        if (_data.TryGetValue(type, out ret))
        {
          count = _counts[type];
          return ret;
        }

        _data.Add(type, ret = CreateResourceNames(type, out count));
        _counts.Add(type, count);
      }

      return ret;
    }

    public static string GetString(Type type, string resourceName)
    {
      int found_count;

      foreach (var manager in ResourceHelper.GetResourceManagers(type, out found_count))
      {
        if (found_count-- > 0)
          return manager.GetString(resourceName);
        else
        {
          string resource = manager.GetString(resourceName);

          if (!string.IsNullOrEmpty(resource))
            return resource;
        }
      }

      return null;
    }

    private static string[] GetNames(Assembly assembly)
    {
      string[] names;

      if (!_names.TryGetValue(assembly, out names))
        _names.Add(assembly, names = assembly.GetManifestResourceNames());

      return names;
    }

    private static ResourceManager[] CreateResourceNames(Type type, out int foundCount)
    {
      string[] resource_names = GetNames(type.Assembly);

      ResourceManager[] ret = new ResourceManager[resource_names.Length];
      string target_name = "." + type.Name;

      foundCount = 0;

      for (int i = 0; i < resource_names.Length; i++)
      {
        string base_name = resource_names[i].Replace(".resources", "");

        ret[i] = new ResourceManager(base_name, type.Assembly);

        if (base_name.EndsWith(target_name) || base_name == type.Name)
        {
          if (foundCount != i)
          {
            ResourceManager tmp = ret[i];
            ret[i] = ret[foundCount];
            ret[foundCount] = tmp;
          }

          foundCount++;
        }
      }

      return ret;
    }
  }
}