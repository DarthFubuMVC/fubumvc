using System;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public class PreviewModelActivator : IPreviewModelActivator
    {
        public object Activate(Type modelType)
        {
            return Activator.CreateInstance(modelType);
        }
    }
}