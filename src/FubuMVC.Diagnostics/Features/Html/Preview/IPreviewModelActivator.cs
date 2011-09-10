using System;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public interface IPreviewModelActivator
    {
        object Activate(Type modelType);
    }
}