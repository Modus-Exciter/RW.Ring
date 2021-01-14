using System;
using System.ComponentModel;

namespace Notung.ComponentModel
{
  public abstract class CustomTypeDescriptionProvider : TypeDescriptionProvider
  {
    private static readonly TypeDescriptionProvider _base_provider = TypeDescriptor.GetProvider(typeof(object));

    public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
    {
      return _base_provider.CreateInstance(provider, objectType, argTypes, args);
    }

    public override System.Collections.IDictionary GetCache(object instance)
    {
      return _base_provider.GetCache(instance);
    }

    public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
    {
      return _base_provider.GetExtendedTypeDescriptor(instance);
    }

    public override string GetFullComponentName(object component)
    {
      return _base_provider.GetFullComponentName(component);
    }

    public override Type GetReflectionType(Type objectType, object instance)
    {
      return _base_provider.GetReflectionType(objectType, instance);
    }

    public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
    {
      return _base_provider.GetTypeDescriptor(objectType, instance);
    }
  }
}