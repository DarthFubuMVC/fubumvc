using System;
using HtmlTags;

namespace FubuMVC.Diagnostics.Shared
{
    public class TypeEndpoint
    {
        public HtmlTag VisualizePartial(TypeInput input)
        {
            var type = input.Type;

            var div = new HtmlTag("div");
            div.Text(type.Name);
            div.Title(type.AssemblyQualifiedName);

            return div;
        }
    }

    public class TypeInput
    {
        public Type Type { get; set; }
    }
}