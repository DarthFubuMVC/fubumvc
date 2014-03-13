using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using FubuCore.Binding;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
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
            var mimetypeContext = context.Service<MimetypeContext>();


            var contentType = mimetypeContext.ContentType;
            var acceptType = mimetypeContext.Accepts;
            
            

            var currentMimeType = new CurrentMimeType(contentType, acceptType);
            mimetypeContext.Correct(currentMimeType);

            return currentMimeType;
        }



        public class MimetypeContext
        {
            private readonly ConnegSettings _settings;
            private readonly ICurrentChain _currentChain;
            private readonly IHttpRequest _request;

            public MimetypeContext(ConnegSettings settings, ICurrentChain currentChain, IHttpRequest request)
            {
                _settings = settings;
                _currentChain = currentChain;
                _request = request;
            }

            public string ContentType
            {
                get
                {
                    return _request.GetHeader(HttpRequestHeader.ContentType).FirstOrDefault() ??
                              MimeType.HttpFormMimetype;
                }
            }

            public string Accepts
            {
                get
                {
                    return readAcceptType(_request.GetHeader(HttpRequestHeader.Accept));
                }
            }

            private string readAcceptType(IEnumerable<string> header)
            {
                if (header == null || header.Count() == 0) return "*/*";

                if (header.Count() == 1) return header.Single();

                return header.Join(", ");
            }

            public void Correct(CurrentMimeType currentMimeType)
            {
                _settings.InterpretQuerystring(currentMimeType, _request);

                _settings.Corrections.Each(x => x.Correct(currentMimeType, _request, _currentChain.OriginatingChain));
            }
        }
    }
}