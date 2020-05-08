using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Notung.ComponentModel;

namespace Notung
{
  // TODO: не создавать лишних экземпляров этого класса
  public sealed class ApplicationInfo
  {
    private readonly Assembly m_product_assembly;

    private string m_company;
    private string m_product;
    private string m_description;
    private Version m_version;

    public ApplicationInfo(Assembly productAssembly)
    {
      if (productAssembly == null)
        throw new ArgumentNullException("productAssembly");

      m_product_assembly = productAssembly;
    }

    public ApplicationInfo() : this(Assembly.GetEntryAssembly() ?? typeof(ApplicationInfo).Assembly) { }

    public Assembly ProductAssembly
    {
      get { return m_product_assembly; }
    }

    public string Company
    {
      get
      {
        if (m_company == null)
        {
          string company = null;
          var attr = m_product_assembly.GetCustomAttribute<AssemblyCompanyAttribute>();

          if (attr != null && !string.IsNullOrWhiteSpace(attr.Company))
            company = attr.Company;

          if (company == null)
            company = string.Empty;

          m_company = company;
        }

        return m_company;
      }
    }

    public string Product
    {
      get
      {
        if (m_product == null)
        {
          string product = null;
          var attr = m_product_assembly.GetCustomAttribute<AssemblyProductAttribute>();

          if (attr != null && !string.IsNullOrWhiteSpace(attr.Product))
            product = attr.Product;

          if (string.IsNullOrWhiteSpace(product))
            product = m_product_assembly.GetName().Name;

          m_product = product;
        }

        return m_product;
      }
    }

    public string Description
    {
      get
      {
        if (m_description == null)
        {
          string description = null;
          var attr = m_product_assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();

          if (attr != null && !string.IsNullOrWhiteSpace(attr.Description))
            description = attr.Description;

          if (description == null)
            description = string.Empty;

          m_description = description;
        }

        return m_description;
      }
    }

    public Version Version
    {
      get
      {
        if (m_version == null)
          m_version = m_product_assembly.GetName().Version;

        return m_version;
      }
    }

    public override string ToString()
    {
      return m_product_assembly.ToString();
    }

    public override bool Equals(object obj)
    {
      ApplicationInfo other = obj as ApplicationInfo;

      if (other == null)
        return false;

      return m_product_assembly.Equals(other.m_product_assembly);
    }

    public override int GetHashCode()
    {
      return m_product_assembly.GetHashCode();
    }
  }
}
