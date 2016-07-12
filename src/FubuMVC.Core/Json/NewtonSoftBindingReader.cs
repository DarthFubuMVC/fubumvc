using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Json
{
    [Description("Uses model binding against the parsed JSON. Use with caution")]
    public class NewtonSoftBindingReader<T> : IReader<T>
    {
        public async Task<T> Read(string mimeType, IFubuRequestContext context)
        {
            var json = await context.Services.GetInstance<NewtonSoftJsonReader>().GetInputText().ConfigureAwait(false);
            var values = new JObjectValues(json);

            var value = context.Services.GetInstance<IObjectResolver>().BindModel(typeof(T), values).Value.As<T>();
            return value;
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