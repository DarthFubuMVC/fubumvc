using System.Collections.Generic;
using System.ComponentModel;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Json
{
    [Description("Uses model binding against the parsed JSON. Use with caution")]
    public class NewtonSoftBindingReader<T> : Core.Resources.Conneg.IReader<T>
    {
        public T Read(string mimeType, IFubuRequestContext context)
        {
            var json = context.Services.GetInstance<NewtonSoftJsonReader>().GetInputText();
            var values = new JObjectValues(json);

            return (T)context.Services.GetInstance<IObjectResolver>().BindModel(typeof(T), values).Value;
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return "application/json";
                yield return "text/json";
            }
        }
    }
}