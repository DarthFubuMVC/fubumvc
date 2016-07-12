using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core.Resources.Conneg
{

    public class FormatterReader<T> : IReader<T>, DescribesItself
    {
        private readonly IFormatter _formatter;

        public FormatterReader(IFormatter formatter)
        {
            _formatter = formatter;
        }

        public async Task<T> Read(string mimeType, IFubuRequestContext context)
        {
            var model = await _formatter.Read<T>(context).ConfigureAwait(false);

            context.Services.GetInstance<IBindingContext>().BindProperties(model);

            return model;
        }

        public IEnumerable<string> Mimetypes => _formatter.MatchingMimetypes;

        public void Describe(Description description)
        {
            var formatter = Description.For(_formatter);
            description.ShortDescription = null;
            description.Title = "Reading with " + formatter.Title;
            description.Children["Formatter"] = formatter;
        }

        public IFormatter Formatter => _formatter;
    }
}