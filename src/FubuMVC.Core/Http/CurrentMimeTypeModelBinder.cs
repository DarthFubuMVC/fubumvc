using System;
using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    // Run the Storyteller tests for Conneg to test this class
    public class CurrentMimeTypeModelBinder : IModelBinder
    {
        public bool Matches(Type type)
        {
            return type == typeof (CurrentMimeType);
        }

        public void Bind(Type type, object instance, IBindingContext context)
        {
            throw new NotSupportedException();
        }

        public object Bind(Type type, IBindingContext context)
        {
            var contentType = context.ValueAs<string>("Content-Type");
            var acceptType = context.ValueAs<string>("Accept");
            var currentMimeType = new CurrentMimeType(contentType, acceptType);

            // Life just doesn't get to be simple.  What if a jquery plugin, just pulling an example out of 
            // very thick air, posts json, but with an incorrect mimetype? 
            if (currentMimeType.ContentType == CurrentMimeType.HttpFormMimetype && context.IsAjaxRequest())
            {
                var streamingData = context.Service<IStreamingData>();
                if (streamingData.CouldBeJson())
                {
                    // TODO
                    currentMimeType.ContentType = "text/json";
                }
            }


            return currentMimeType;
        }
    }
}