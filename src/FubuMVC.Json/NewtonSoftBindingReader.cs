using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core;

namespace FubuMVC.Json
{
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