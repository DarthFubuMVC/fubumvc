using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using FubuCore.Binding;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    // Run the Storyteller tests for Conneg to test this class
    [Description("Custom model binder for FubuMVC's CurrentMimeType class")]
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
            var request = context.Service<ICurrentHttpRequest>();

            var contentType = request.GetHeader(HttpRequestHeader.ContentType).FirstOrDefault() ??
                              MimeType.HttpFormMimetype;
            
            var acceptType = ReadAcceptType(request.GetHeader(HttpRequestHeader.Accept));
            
            

            var currentMimeType = new CurrentMimeType(contentType, acceptType);


            return currentMimeType;
        }

        private string ReadAcceptType(IEnumerable<string> header)
        {
            if (header == null || header.Count() == 0) return "*/*";

            if (header.Count() == 1) return header.Single();

            return header.Join(", ");
        }
    }
}