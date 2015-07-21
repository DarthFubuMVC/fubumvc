using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Runtime.Serializers
{
    /// <summary>
    /// Low performance Json serializer strictly for Testing!  
    /// </summary>
    public class BasicJsonMessageSerializer : IMessageSerializer
    {
        public void Serialize(object message, Stream stream)
        {
            var json = JsonUtil.ToJson(message);
            var dictionary = JsonUtil.Get(json).As<IDictionary<string, object>>();
            dictionary.Add("_type_", message.GetType().AssemblyQualifiedName);

            var writer = new StreamWriter(stream);
            writer.Write(JsonUtil.ToJson(dictionary));
            writer.Flush();
        }

        public object Deserialize(Stream message)
        {
            var reader = new StreamReader(message);
            var json = reader.ReadToEnd();
            var dictionary = JsonUtil.Get(json).As<IDictionary<string,object>>();
            var type = Type.GetType((string) dictionary["_type_"]);


            var invoker = typeof (Invoker<>).CloseAndBuildAs<IInvoker>(type);
            return invoker.Invoke(json);
        }

        public interface IInvoker
        {
            object Invoke(string json);
        }

        public class Invoker<T> : IInvoker
        {
            public object Invoke(string json)
            {
                return JsonUtil.Get<T>(json);
            }
        }

        public string ContentType
        {
            get { return "application/json"; }
        }
    }
}