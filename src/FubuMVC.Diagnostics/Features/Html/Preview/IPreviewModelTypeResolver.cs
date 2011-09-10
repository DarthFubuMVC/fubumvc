using System;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public interface IPreviewModelTypeResolver
    {
        Type TypeFor(string typeName);
    }
}