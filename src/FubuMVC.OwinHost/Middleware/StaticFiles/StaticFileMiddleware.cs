using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{


    public class StaticFileMiddleware : FubuMvcOwinMiddleware
    {
        private readonly IFubuApplicationFiles _files;
        private readonly OwinSettings _settings;

        public StaticFileMiddleware(Func<IDictionary<string, object>, Task> inner, IFubuApplicationFiles files, OwinSettings settings) : base(inner)
        {
            _files = files;
            _settings = settings;
        }

        public override MiddlewareContinuation Invoke(ICurrentHttpRequest request, IHttpWriter writer)
        {
            if (request.IsNotHttpMethod("GET", "HEAD")) return MiddlewareContinuation.Continue();

            var file = _files.Find(request.RelativeUrl());
            if (file == null) return MiddlewareContinuation.Continue();

            if (_settings.DetermineStaticFileRights(file) != AuthorizationRight.Allow)
            {
                return MiddlewareContinuation.Continue();
            }


            if (request.IsHead())
            {
                return new WriteFileHeadContinuation(writer, file, HttpStatusCode.OK);
            }

            var ifMatch = request.IfMatch().EtagMatches(file.Etag());
            if (ifMatch == EtagMatch.No)
            {
                return new WriteStatusCodeContinuation(writer, HttpStatusCode.PreconditionFailed, "If-Match test failed");
            }

            var ifNoneMatch = request.IfNoneMatch().EtagMatches(file.Etag());
            if (ifNoneMatch == EtagMatch.Yes)
            {
                return new WriteFileHeadContinuation(writer, file, HttpStatusCode.NotModified);
            }
            
            if (ifNoneMatch == EtagMatch.No)
            {
                return new WriteFileContinuation(writer, file);
            }

            var ifModifiedSince = request.IfModifiedSince();
            if (ifModifiedSince.HasValue && file.LastModified().ToUniversalTime() <= ifModifiedSince.Value)
            {
                return new WriteFileHeadContinuation(writer, file, HttpStatusCode.NotModified);
            }

            var ifUnModifiedSince = request.IfUnModifiedSince();
            if (ifUnModifiedSince.HasValue && file.LastModified() > ifUnModifiedSince.Value)
            {
                return new WriteStatusCodeContinuation(writer, HttpStatusCode.PreconditionFailed, "File has been modified");
            }


            return new WriteFileContinuation(writer, file);
        }
    }
}