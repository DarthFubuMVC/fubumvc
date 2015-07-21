using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FubuMVC.Core.ServiceBus.Runtime.Serializers
{
    //TODO: This needs a fair amount of rework with semantic model from registry to feed in
    //the various serialization behaviors
    public class XmlMessageSerializer : IMessageSerializer
    {
        private readonly Reflection reflection;

        public XmlMessageSerializer()
        {
            reflection = new Reflection();
        }

        public void Serialize(object message, Stream messageStream)
        {
            var messages = message as object[] ?? new object[] {message};

            var namespaces = GetNamespaces(message);
            var messagesElement = new XElement(namespaces["esb"] + "messages");
            var xml = new XDocument(messagesElement);

            foreach (var m in messages)
            {
                if (m == null)
                    continue;

                try
                {
                    WriteObject(reflection.GetNameForXml(m.GetType()), m, messagesElement, namespaces);
                }
                catch (Exception e)
                {
                    throw new SerializationException("Could not serialize " + m.GetType() + ".", e);
                }
            }

            messagesElement.Add(
                namespaces.Select(x => new XAttribute(XNamespace.Xmlns + x.Key, x.Value))
                );

            var streamWriter = new StreamWriter(messageStream);
            var writer = XmlWriter.Create(streamWriter, new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            });
            if (writer == null)
                throw new InvalidOperationException("Could not create xml writer from stream");

            xml.WriteTo(writer);
            writer.Flush();
            streamWriter.Flush();
        }

        private void WriteObject(string name, object value, XContainer parent, IDictionary<string, XNamespace> namespaces)
        {
            //TODO custom value converter IValueConverter<T> from RSB
            //TODO custom serializer ICustomElementSerializer from RSB
            if (ShouldPutAsString(value))
            {
                var elementName = GetXmlNamespace(namespaces, value.GetType()) + name;
                parent.Add(new XElement(elementName, FormatAsString(value)));
            }
            else if (value is byte[])
            {
                var elementName = GetXmlNamespace(namespaces, typeof(byte[])) + name;
                parent.Add(new XElement(elementName, Convert.ToBase64String((byte[])value)));
            }
            else if (ShouldTreatAsDictionary(value.GetType()))
            {
                XElement list = GetContentWithNamespace(value, namespaces, name);
                parent.Add(list);
                foreach (dynamic keyValuePair in ((IEnumerable)value))
                {
                    var entry = new XElement("entry");
                    object keyProp = keyValuePair.Key;
                    if (keyProp == null)
                        continue;
                    WriteObject("Key", keyProp, entry, namespaces);

                    object propVal = keyValuePair.Value;
                    if (propVal != null)
                    {
                        WriteObject("Value", propVal, entry, namespaces);
                    }

                    list.Add(entry);
                }
            }
            else if (value is IEnumerable)
            {
                XElement list = GetContentWithNamespace(value, namespaces, name);
                parent.Add(list);
                foreach (var item in ((IEnumerable)value))
                {
                    if (item == null)
                        continue;

                    WriteObject("value", item, list, namespaces);
                }
            }
            else
            {
                XElement content = GetContentWithNamespace(value, namespaces, name);
                foreach (var property in value.GetType().GetProperties())
                {
                    var propVal = property.GetValue(value, new object[]{});
                    if (propVal == null)
                        continue;
                    WriteObject(property.Name, propVal, content, namespaces);
                }
                content = ApplyMessageSerializationBehaviorIfNecessary(value.GetType(), content);
                parent.Add(content);
            }
        }

        private static bool ShouldTreatAsDictionary(Type type)
        {
            if (type.IsGenericType == false)
                return false;

            var genericArguments = type.GetGenericArguments();
            if (genericArguments.Length != 2)
                return false;

            var interfaceType = typeof(IDictionary<,>).MakeGenericType(genericArguments);
            if (interfaceType.IsAssignableFrom(type) == false)
                return false;

            return true;
        }

        private XElement ApplyMessageSerializationBehaviorIfNecessary(Type messageType, XElement element)
        {
            //TODO apply serialization behavior
            return element;
        }

        private XElement ApplyMessageDeserializationBehaviorIfNecessary(Type messageType, XElement element)
        {
            //TODO rework the serialization behavior
            return element;
        }

        private XNamespace GetXmlNamespace(IDictionary<string, XNamespace> namespaces, Type type)
        {
            var ns = reflection.GetNamespacePrefixForXml(type);
            XNamespace xmlNs;
            if (namespaces.TryGetValue(ns, out xmlNs) == false)
            {
                namespaces[ns] = xmlNs = reflection.GetNamespaceForXml(type);
            }
            return xmlNs;
        }

        private bool HaveCustomSerializer(Type type)
        {
            //TODO rework the custom serialization
            return false;
        }

        private XElement GetContentWithNamespace(object value, IDictionary<string, XNamespace> namespaces, string name)
        {
            var type = value.GetType();
            var xmlNsAlias = reflection.GetNamespacePrefixForXml(type);
            XNamespace xmlNs;
            if (namespaces.TryGetValue(xmlNsAlias, out xmlNs) == false)
            {
                namespaces[xmlNsAlias] = xmlNs = reflection.GetNamespaceForXml(type);
            }

            return new XElement(xmlNs + name);
        }

        private static bool ShouldPutAsString(object value)
        {
            return value is ValueType || value is string || value is Uri;
        }

        public static object FromString(Type type, string value)
        {
            if (type == typeof(string))
                return value;

            if (type == typeof(Uri))
                return new Uri(value);

            if (type.IsPrimitive)
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

            if (type == typeof(Guid))
                return new Guid(value);

            if (type == typeof(DateTime))
                return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            if (type == typeof(DateTimeOffset))
                return XmlConvert.ToDateTimeOffset(value);

            if (type == typeof(TimeSpan))
                return XmlConvert.ToTimeSpan(value);

            if (type.IsEnum)
                return Enum.Parse(type, value);

            if (type == typeof(decimal))
                return decimal.Parse(value, CultureInfo.InvariantCulture);

            throw new SerializationException("Don't know how to deserialize type: " + type + " from '" + value + "'");
        }

        private static string FormatAsString(object value)
        {
            if (value == null)
                return string.Empty;
            if (value is bool)
                return value.ToString().ToLower();
            if (value is string)
                return value as string;
            if (value is Uri)
                return value.ToString();

            if (value is DateTime)
                return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);

            if (value is DateTimeOffset)
                return XmlConvert.ToString((DateTimeOffset)value);

            if (value is TimeSpan)
            {
                var ts = (TimeSpan)value;
                return string.Format("P0Y0M{0}DT{1}H{2}M{3}S", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            }
            if (value is Guid)
                return ((Guid)value).ToString();

            if (value is decimal)
                return ((decimal)value).ToString(CultureInfo.InvariantCulture);

            if (value is double)
                return ((double)value).ToString(CultureInfo.InvariantCulture);

            if (value is float)
                return ((float)value).ToString(CultureInfo.InvariantCulture);

            return value.ToString();
        }

        private IDictionary<string, XNamespace> GetNamespaces(object message)
        {
            var messages = message as object[] ?? new []{message};

            var namespaces = new Dictionary<string, XNamespace>
            {
                {"esb", "http://servicebus.fubutransportation.com/2013/07/19/esb"},
            };
            foreach (var msg in messages)
            {
                if (msg == null)
                    continue;
                var type = msg.GetType();
                namespaces[reflection.GetNamespacePrefixForXml(type)] = reflection.GetNamespaceForXml(type);
            }
            return namespaces;
        }

        public object Deserialize(Stream message)
        {
            var document = XDocument.Load(XmlReader.Create(message));
            if (document.Root == null)
                throw new SerializationException("document doesn't have root element");

            if (document.Root.Name.LocalName != "messages")
                throw new SerializationException("message doesn't have root element named 'messages'");

            var msgs = new List<object>();
            foreach (var element in document.Root.Elements())
            {
                var type = reflection.GetTypeFromXmlNamespace(element.Name.NamespaceName);
                if (type == null)
                {
                    throw new SerializationException("Cannot find root message type: " + element.Name.Namespace);
                }
                var msg = ReadObject(type, element);
                msgs.Add(msg);
            }

            if (msgs.Count == 1)
            {
                return msgs.Single();
            }

            return msgs.ToArray();
        }

        public string ContentType { 
            get { return "application/xml"; }
        }

        private object ReadObject(Type type, XElement element)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            element = ApplyMessageDeserializationBehaviorIfNecessary(type, element);

            if (false)//LoadCustomValueConvertor(type) != null)
            {
                //var convertorType = reflection.GetGenericTypeOf(typeof(IValueConvertor<>), type);
                //var convertor = _serviceLocator.Resolve(convertorType);
                //return reflection.InvokeFromElement(convertor, element);
            }
            if (HaveCustomSerializer(type))
            {
                //var customSerializer = _customElementSerializers.First(s => s.CanSerialize(type));
                //return customSerializer.FromElement(type, element);
            }
            if (type == typeof(byte[]))
            {
                return Convert.FromBase64String(element.Value);
            }
            if (CanParseFromString(type))
            {
                return FromString(type, element.Value);
            }
            if (ShouldTreatAsDictionary(type))
            {
                return ReadDictionary(type, element);
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return ReadList(type, element);
            }
            object instance = reflection.CreateInstance(type);
            foreach (var prop in element.Elements())
            {
                var property = prop;
                reflection.Set(instance,
                    prop.Name.LocalName,
                    typeFromProperty =>
                    {
                        var propType = reflection.GetTypeFromXmlNamespace(property.Name.NamespaceName);
                        return ReadObject(propType ?? typeFromProperty, property);
                    });
            }
            return instance;
        }

        private static bool CanParseFromString(Type type)
        {
            if (type.IsPrimitive)
                return true;

            if (type == typeof(string))
                return true;

            if (type == typeof(Uri))
                return true;

            if (type == typeof(DateTime))
                return true;

            if (type == typeof(DateTimeOffset))
                return true;

            if (type == typeof(TimeSpan))
                return true;

            if (type == typeof(Guid))
                return true;

            if (type.IsEnum)
                return true;

            if (type == typeof(decimal))
                return true;

            return false;
        }

        private object ReadList(Type type, XContainer element)
        {
            object instance;
            Type elementType;
            if (type.IsArray)
            {
                instance = reflection.CreateInstance(type, element.Elements().Count());
                elementType = type.GetElementType();
            }
            else
            {
                instance = reflection.CreateInstance(type);
                elementType = type.GetGenericArguments()[0];
            }
            int index = 0;
            var array = instance as Array;
            foreach (var value in element.Elements())
            {
                var itemType = reflection.GetTypeFromXmlNamespace(value.Name.NamespaceName);
                object o = ReadObject(itemType ?? elementType, value);
                if (array != null)
                    array.SetValue(o, index);
                else
                {
                    dynamic dynamicInstance = instance;
                    dynamic dynamicO = o;
                    dynamicInstance.Add(dynamicO);
                }

                index += 1;
            }
            return instance;
        }

        private object ReadDictionary(Type type, XContainer element)
        {
            dynamic instance = reflection.CreateInstance(type);
            var genericArguments = type.GetGenericArguments();
            var keyType = genericArguments[0];
            var valueType = genericArguments[1];
            foreach (var entry in element.Elements())
            {
                var elements = entry.Elements().ToArray();
                var itemKeyType = reflection.GetTypeFromXmlNamespace(elements[0].Name.NamespaceName);
                dynamic key = ReadObject(itemKeyType ?? keyType, elements[0]);

                dynamic value = null;
                if (elements.Length > 1)
                {
                    var itemValueType = reflection.GetTypeFromXmlNamespace(elements[1].Name.NamespaceName);
                    value = ReadObject(itemValueType ?? valueType, elements[1]);
                }

                instance.Add(key, value);
            }
            return instance;
        }

        //TODO nuke this
        private class Reflection
        {
            private readonly IDictionary<Type, string> typeToWellKnownTypeName;
            private readonly IDictionary<string, Type> wellKnownTypeNameToType;

            public Reflection()
            {
                wellKnownTypeNameToType = new Dictionary<string, Type>();
                typeToWellKnownTypeName = new Dictionary<Type, string>
                {
                    {typeof (string), typeof (string).FullName},
                    {typeof (int), typeof (int).FullName},
                    {typeof (byte), typeof (byte).FullName},
                    {typeof (bool), typeof (bool).FullName},
                    {typeof (DateTime), typeof (DateTime).FullName},
                    {typeof (TimeSpan), typeof (TimeSpan).FullName},
                    {typeof (decimal), typeof (decimal).FullName},
                    {typeof (float), typeof (float).FullName},
                    {typeof (double), typeof (double).FullName},
                    {typeof (char), typeof (char).FullName},
                    {typeof (Guid), typeof (Guid).FullName},
                    {typeof (Uri), typeof (Uri).FullName},
                    {typeof (short), typeof (short).FullName},
                    {typeof (long), typeof (long).FullName},
                    {typeof (byte[]), "binary"}
                };
                foreach (var pair in typeToWellKnownTypeName)
                {
                    wellKnownTypeNameToType.Add(pair.Value, pair.Key);
                }
            }

            public Type GetTypeFromXmlNamespace(string xmlNamespace)
            {
                Type value;
                if (wellKnownTypeNameToType.TryGetValue(xmlNamespace, out value))
                    return value;
                if (xmlNamespace.StartsWith("array_of_"))
                {
                    return GetTypeFromXmlNamespace(xmlNamespace.Substring("array_of_".Length));
                }
                return Type.GetType(xmlNamespace);
            }

            public string GetNameForXml(Type type)
            {
                var typeName = type.Name;
                typeName = typeName.Replace('[', '_').Replace(']', '_');
                var indexOf = typeName.IndexOf('`');
                if (indexOf == -1)
                    return typeName;
                typeName = typeName.Substring(0, indexOf) + "_of_";
                foreach (var argument in type.GetGenericArguments())
                {
                    typeName += GetNamespacePrefixForXml(argument) + "_";
                }
                return typeName.Substring(0, typeName.Length - 1);
            }

            public string GetNamespacePrefixForXml(Type type)
            {
                string value;
                if (typeToWellKnownTypeName.TryGetValue(type, out value))
                    return value;
                if (type.IsArray)
                    return "array_of_" + GetNamespacePrefixForXml(type.GetElementType());

                if (type.Namespace == null && type.Name.StartsWith("<>"))
                    throw new InvalidOperationException("Anonymous types are not supported");

                if (type.Namespace == null) //global types?
                {
                    return type.Name
                        .ToLowerInvariant();
                }
                var typeName = type.Namespace.Split('.')
                              .Last().ToLowerInvariant() + "." + type.Name.ToLowerInvariant();
                var indexOf = typeName.IndexOf('`');
                if (indexOf == -1)
                    return typeName;
                typeName = typeName.Substring(0, indexOf) + "_of_";
                foreach (var argument in type.GetGenericArguments())
                {
                    typeName += GetNamespacePrefixForXml(argument) + "_";
                }
                return typeName.Substring(0, typeName.Length - 1);
            }


            public string GetNamespaceForXml(Type type)
            {
                string value;
                if (typeToWellKnownTypeName.TryGetValue(type, out value))
                    return value;

                Assembly assembly = type.Assembly;
                string fullName = assembly.FullName;
                if (type.IsGenericType)
                {
                    var builder = new StringBuilder();
                    int startOfGenericName = type.FullName.IndexOf('[');
                    builder.Append(type.FullName.Substring(0, startOfGenericName))
                        .Append("[")
                        .Append(String.Join(",",
                                        type.GetGenericArguments()
                                            .Select(t => "[" + GetNamespaceForXml(t) + "]")
                                            .ToArray()))
                        .Append("], ");
                    if (assembly.GlobalAssemblyCache)
                    {
                        builder.Append(fullName);
                    }
                    else
                    {
                        builder.Append(fullName.Split(',')[0]);
                    }
                    return builder.ToString();
                }

                if (assembly.GlobalAssemblyCache == false)
                {
                    return type.FullName + ", " + fullName.Split(',')[0];
                }
                return type.AssemblyQualifiedName;
            }

            public object CreateInstance(Type type, params object[] args)
            {
                return Activator.CreateInstance(type,
                                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                                                null, args, null);
            }

            public void Set(object instance, string name, Func<Type, object> generateValue)
            {
                Type type = instance.GetType();
                PropertyInfo property = type.GetProperty(name);
                if (property == null || property.CanWrite == false)
                {
                    return;
                }
                object value = generateValue(property.PropertyType);
                property.SetValue(instance, value, null);
            }
        }
    }
}