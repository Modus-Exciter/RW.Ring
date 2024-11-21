using System.Collections.Generic;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Reflection;
using System.Threading;
using Notung.Logging;
using System.Linq;
using System.Net;
using System.Xml;
using Notung.Net;
using System.IO;
using System;

namespace Notung.Services
{
  public class WsdlGenerateService
  {
    private static readonly Dictionary<string, string> _wsdl_cache = new Dictionary<string, string>();
    private static readonly ILog _log = LogManager.GetLogger(typeof(WsdlGenerateService));
    private readonly Dictionary<string, IRpcServiceInfo> m_services;

    // Добавить константы

    public WsdlGenerateService()
    {
      m_services = new Dictionary<string, IRpcServiceInfo>();
    }

    // По идеи можно добавить параметр serviceName и получать WSDL каждого сервиса, если структура соответствует
    public static async Task HandleWsdlRequests(HttpListener listener)
    {
      try
      {
        var context = await listener.GetContextAsync();
        HttpListenerRequest request = context.Request;
       
        if (request.Url.AbsolutePath.EndsWith("/ModelService") && request.Url.Query.Contains("wsdl"))
        {  
          if (!_wsdl_cache.TryGetValue("ModelService", out string wsdl))
          {
            IRpcServiceInfo serviceInfo = RpcServiceInfo.GetByName("ModelService");
            wsdl = GenerateWsdl(serviceInfo);
            _wsdl_cache["ModelService"] = wsdl;
          }

          HttpListenerResponse response = context.Response;

          byte[] buffer = System.Text.Encoding.UTF8.GetBytes(wsdl);
          response.ContentLength64 = buffer.Length;
          response.ContentType = "text/xml";

          await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        else
        {
          context.Response.StatusCode = 404;
          context.Response.Close();
        }
      }
      catch (ThreadAbortException)
      {
        return;
      }
      catch (Exception ex)
      {
        _log.Info($"Ошибка при обработке WSDL запроса: {ex.Message}");
      }
    }

    private static string GenerateWsdl(IRpcServiceInfo serviceInfo)
    {
      _log.Info("Начало генерации WSDL для сервиса: " + serviceInfo.ServiceName);

      var wsdl = new WsdlDocument
      {
        TargetNamespace = "http://tempuri.org/",
        ServiceName = serviceInfo.ServiceName,
        Types = new WsdlTypes(),
        Messages = new List<WsdlMessage>(),
        PortType = new WsdlPortType { Name = serviceInfo.ServiceName + "PortType" },
        Binding = new WsdlBinding
        {
          Name = serviceInfo.ServiceName + "Binding",
          Type = serviceInfo.ServiceName + "PortType",
          SoapBinding = new WsdlSoapBinding
          {
            Style = "document",
            Transport = "http://schemas.xmlsoap.org/soap/http"
          }
        },
        Service = new WsdlService
        {
          Name = serviceInfo.ServiceName,
          Port = new WsdlPort
          {
            Name = serviceInfo.ServiceName + "Port",
            Binding = serviceInfo.ServiceName + "Binding",
            SoapAddress = new WsdlSoapAddress { Location = "http://localhost:14488/ModelService" }
          }
        }
      };

      var methods = serviceInfo.GetMethods().ToList();
      _log.Info($"Найдено методов: {methods.Count}");

      var processedTypes = new HashSet<Type>();

      foreach (var methodEntry in methods)
      {
        string methodName = methodEntry.Key;
        var operationInfo = methodEntry.Value;

        _log.Info($"Обработка метода: {methodName}");

        string inputMessageName = methodName + "SoapIn";
        string outputMessageName = methodName + "SoapOut";

        AddTypeToSchema(wsdl.Types.Schema, operationInfo.RequestType, processedTypes);
        AddTypeToSchema(wsdl.Types.Schema, operationInfo.ResultType, processedTypes);

        wsdl.Messages.Add(new WsdlMessage
        {
          Name = inputMessageName,
          Part = new WsdlPart { Name = "parameters", Element = operationInfo.RequestType.Name }
        });

        wsdl.Messages.Add(new WsdlMessage
        {
          Name = outputMessageName,
          Part = new WsdlPart { Name = "parameters", Element = operationInfo.ResponseType.Name }
        });

        var operation = new WsdlOperation
        {
          Name = methodName,
          Input = new WsdlOperationIO { Message = inputMessageName },
          Output = new WsdlOperationIO { Message = outputMessageName }
        };

        AddDocumentation(operation, methodEntry.Value.Method);

        wsdl.PortType.Operations.Add(operation);

        wsdl.Binding.Operations.Add(new WsdlBindingOperation
        {
          Name = methodName,
          SoapOperation = new WsdlSoapOperation { SoapAction = "http://tempuri.org/" + methodName },
          Input = new WsdlBindingOperationIO { SoapBody = new WsdlSoapBody { Use = "literal" } },
          Output = new WsdlBindingOperationIO { SoapBody = new WsdlSoapBody { Use = "literal" } }
        });
      }

      var serializer = new XmlSerializer(typeof(WsdlDocument));
      var ns = new XmlSerializerNamespaces();

      ns.Add("wsdl", "http://schemas.xmlsoap.org/wsdl/");
      ns.Add("soap", "http://schemas.xmlsoap.org/wsdl/soap/");
      ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
      ns.Add("tns", wsdl.TargetNamespace);

      using (var stringWriter = new StringWriter())
      {
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
          serializer.Serialize(xmlWriter, wsdl, ns);
        }

        _log.Info("Генерация WSDL завершена");
        return stringWriter.ToString();
      }
    }

    // Метод с рекурсией
    private static void AddTypeToSchema(XmlSchema schema, Type type, HashSet<Type> processedTypes)
    {
      if (processedTypes.Contains(type))
        return;

      if (schema.Elements[new XmlQualifiedName(type.Name, "http://tempuri.org/")] != null)
        return;

      _log.Info($"Добавление типа в схему: {type.Name}");

      processedTypes.Add(type);
      var element = new XmlSchemaElement { Name = type.Name };

      if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
      {
        var arrayType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];

        var complexType = new XmlSchemaComplexType();
        var sequence = new XmlSchemaSequence();

        var itemElement = new XmlSchemaElement
        {
          Name = "Item",
          SchemaTypeName = GetXsdTypeName(arrayType)
        };

        sequence.Items.Add(itemElement);

        complexType.Particle = sequence;
        element.SchemaType = complexType;

        if (!IsSimpleType(arrayType))
          AddTypeToSchema(schema, arrayType, processedTypes);
      }
      else if (!IsSimpleType(type))
      {
        var complexType = new XmlSchemaComplexType();
        var sequence = new XmlSchemaSequence();

        foreach (var property in type.GetProperties())
        {
          _log.Info($" Обработка свойства: {property.Name}");

          var propertyElement = new XmlSchemaElement
          {
            Name = property.Name,
            SchemaTypeName = GetXsdTypeName(property.PropertyType)
          };

          sequence.Items.Add(propertyElement);

          if (!IsSimpleType(property.PropertyType))
            AddTypeToSchema(schema, property.PropertyType, processedTypes);
        }

        complexType.Particle = sequence;
        element.SchemaType = complexType;
      }

      schema.Items.Add(element);
    }

    private static XmlQualifiedName GetXsdTypeName(Type type)
    {
      if (IsSimpleType(type))
      {
        return new XmlQualifiedName(GetXsdType(type), "http://www.w3.org/2001/XMLSchema");
      }
      else if (type.IsArray)
      {
        return new XmlQualifiedName(type.GetElementType().Name + "Array", "http://tempuri.org/");
      }
      else
      {
        return new XmlQualifiedName(type.Name, "http://tempuri.org/");
      }
    }

    private static bool IsSimpleType(Type type)
      => type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);

    private static string GetXsdType(Type type)
    {
      if (type == typeof(int)) return "int";
      if (type == typeof(long)) return "long";
      if (type == typeof(string)) return "string";
      if (type == typeof(bool)) return "boolean";
      if (type == typeof(DateTime)) return "dateTime";
      if (type == typeof(float)) return "float";
      if (type == typeof(double)) return "double";
      if (type == typeof(decimal)) return "decimal";
      if (type == typeof(byte)) return "byte";
      if (type == typeof(Guid)) return "string";
      return "anyType";
    }

    private static void AddDocumentation(WsdlOperation operation, MethodInfo method)
    {
      var xmlDoc = method.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
      operation.Documentation = xmlDoc != null ? xmlDoc.Description : $"Operation: {method.Name}";
    }

    private static void ValidateWsdl(string wsdlContent)
    {
      try
      {
        var settings = new XmlReaderSettings
        {
          ValidationType = ValidationType.Schema
        };

        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

        settings.ValidationEventHandler +=
          (sender, e) =>
          {
            _log.Info($"WSDL Validation {e.Severity}: {e.Message}");
          };

        using (var stringReader = new StringReader(wsdlContent))
        using (var reader = XmlReader.Create(stringReader, settings))
        {
          while (reader.Read()) { }
        }
        _log.Info("WSDL успешно прошел валидацию");
      }
      catch (Exception ex)
      {
        _log.Info($"Ошибка при валидации WSDL: {ex.Message}");
      }
    }
  }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ThrowsAttribute : Attribute
{
  public Type ExceptionType { get; }

  public ThrowsAttribute(Type exceptionType)
  {
    ExceptionType = exceptionType;
  }
}

/// <summary>
/// Представляет корневой элемент WSDL документа.
/// </summary>
[XmlRoot("definitions", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
public class WsdlDocument
{
  [XmlAttribute("targetNamespace")]
  public string TargetNamespace { get; set; }

  [XmlAttribute("name")]
  public string ServiceName { get; set; }

  [XmlElement("types")]
  public WsdlTypes Types { get; set; }

  [XmlElement("message")]
  public List<WsdlMessage> Messages { get; set; }

  [XmlElement("portType")]
  public WsdlPortType PortType { get; set; }

  [XmlElement("binding")]
  public WsdlBinding Binding { get; set; }

  [XmlElement("service")]
  public WsdlService Service { get; set; }

  [XmlNamespaceDeclarations]
  public XmlSerializerNamespaces Namespaces { get; set; }

  public WsdlDocument()
  {
    Namespaces = new XmlSerializerNamespaces();
    Namespaces.Add("soap", "http://schemas.xmlsoap.org/wsdl/soap/");
    Namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
    Namespaces.Add("tns", "http://tempuri.org/");
  }
}

/// <summary>
/// Представляет секцию types в WSDL документе.
/// </summary>
public class WsdlTypes
{
  [XmlElement("schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
  public XmlSchema Schema { get; set; }

  public WsdlTypes()
  {
    Schema = new XmlSchema();
  }
}

/// <summary>
/// Представляет сообщение в WSDL документе.
/// </summary>
public class WsdlMessage
{
  [XmlAttribute("name")]
  public string Name { get; set; }

  [XmlElement("part")]
  public WsdlPart Part { get; set; }
}

/// <summary>
/// Представляет часть сообщения в WSDL документе.
/// </summary>
public class WsdlPart
{
  [XmlAttribute("name")]
  public string Name { get; set; }

  [XmlAttribute("element")]
  public string Element { get; set; }
}

/// <summary>
/// Представляет portType в WSDL документе.
/// </summary>
public class WsdlPortType
{
  [XmlAttribute("name")]
  public string Name { get; set; }

  [XmlElement("operation")]
  public List<WsdlOperation> Operations { get; set; }

  public WsdlPortType()
  {
    Operations = new List<WsdlOperation>();
  }
}

/// <summary>
/// Представляет операцию в WSDL документе.
/// </summary>
public class WsdlOperation
{
  [XmlAttribute("name")]
  public string Name { get; set; }

  [XmlElement("documentation", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
  public string Documentation { get; set; }

  [XmlElement("input")]
  public WsdlOperationIO Input { get; set; }

  [XmlElement("output")]
  public WsdlOperationIO Output { get; set; }

  [XmlElement("fault")]
  public List<WsdlOperationIO> Faults { get; set; } = new List<WsdlOperationIO>();
}

/// <summary>
/// Представляет входные/выходные данные операции в WSDL документе.
/// </summary>
public class WsdlOperationIO
{
  [XmlAttribute("message")]
  public string Message { get; set; }
}

/// <summary>
/// Представляет binding в WSDL документе.
/// </summary>
public class WsdlBinding
{
  [XmlAttribute("name")]
  public string Name { get; set; }

  [XmlAttribute("type")]
  public string Type { get; set; }

  [XmlElement("soap:binding", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
  public WsdlSoapBinding SoapBinding { get; set; }

  [XmlElement("operation")]
  public List<WsdlBindingOperation> Operations { get; set; }

  public WsdlBinding()
  {
    Operations = new List<WsdlBindingOperation>();
  }
}

/// <summary>
/// Представляет SOAP binding в WSDL документе.
/// </summary>
public class WsdlSoapBinding
{
  [XmlAttribute("style")]
  public string Style { get; set; }

  [XmlAttribute("transport")]
  public string Transport { get; set; }
}

/// <summary>
/// Представляет операцию в binding секции WSDL документа.
/// </summary>
public class WsdlBindingOperation
{
  [XmlAttribute("name")]
  public string Name { get; set; }

  [XmlElement("soap:operation", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
  public WsdlSoapOperation SoapOperation { get; set; }

  [XmlElement("input")]
  public WsdlBindingOperationIO Input { get; set; }

  [XmlElement("output")]
  public WsdlBindingOperationIO Output { get; set; }
}

/// <summary>
/// Представляет SOAP операцию в WSDL документе.
/// </summary>
public class WsdlSoapOperation
{
  [XmlAttribute("soapAction")]
  public string SoapAction { get; set; }
}

/// <summary>
/// Представляет входные/выходные данные операции в binding секции WSDL документа.
/// </summary>
public class WsdlBindingOperationIO
{
  [XmlElement("soap:body", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
  public WsdlSoapBody SoapBody { get; set; }
}

/// <summary>
/// Представляет SOAP body в WSDL документе.
/// </summary>
public class WsdlSoapBody
{
  [XmlAttribute("use")]
  public string Use { get; set; }
}

/// <summary>
/// Представляет service в WSDL документе.
/// </summary>
public class WsdlService
{
  [XmlAttribute("name")]
  public string Name { get; set; }

  [XmlElement("port")]
  public WsdlPort Port { get; set; }
}

/// <summary>
/// Представляет port в WSDL документе.
/// </summary>
public class WsdlPort
{
  [XmlAttribute("name")]
  public string Name { get; set; }

  [XmlAttribute("binding")]
  public string Binding { get; set; }

  [XmlElement("soap:address", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
  public WsdlSoapAddress SoapAddress { get; set; }
}

/// <summary>
/// Представляет SOAP адрес в WSDL документе.
/// </summary>
public class WsdlSoapAddress
{
  [XmlAttribute("location")]
  public string Location { get; set; }
}