using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class StaticFileMiddleware : FubuMvcOwinMiddleware
    {
        private readonly IFubuApplicationFiles _files;
        private readonly OwinSettings _settings;

        public StaticFileMiddleware(AppFunc inner, IFubuApplicationFiles files, OwinSettings settings) : base(inner)
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

            if (request.IfMatchHeaderDoesNotMatchEtag(file))
            {
                return new WriteStatusCodeContinuation(writer, HttpStatusCode.PreconditionFailed, "If-Match test failed"); 
            }

            if (request.IfNoneMatchHeaderMatchesEtag(file))
            {
                return new WriteFileHeadContinuation(writer, file, HttpStatusCode.NotModified);
            }

            if (request.IfModifiedSinceHeaderAndNotModified(file))
            {
                return new WriteFileHeadContinuation(writer, file, HttpStatusCode.NotModified);
            }

            if (request.IfUnModifiedSinceHeaderAndModifiedSince(file))
            {
                return new WriteStatusCodeContinuation(writer, HttpStatusCode.PreconditionFailed, "File has been modified");
            }

            return new WriteFileContinuation(writer, file);
        }

        
    }

    public static class CurrentHttpRequestExtensions
    {
        public static bool IfUnModifiedSinceHeaderAndModifiedSince(this ICurrentHttpRequest request, IFubuFile file)
        {
            var ifUnModifiedSince = request.IfUnModifiedSince();
            return ifUnModifiedSince.HasValue && file.LastModified() > ifUnModifiedSince.Value;
        }

        public static bool IfModifiedSinceHeaderAndNotModified(this ICurrentHttpRequest request, IFubuFile file)
        {
            var ifModifiedSince = request.IfModifiedSince();
            return ifModifiedSince.HasValue && file.LastModified().ToUniversalTime() <= ifModifiedSince.Value;
        }

        public static bool IfNoneMatchHeaderMatchesEtag(this ICurrentHttpRequest request, IFubuFile file)
        {
            return request.IfNoneMatch().EtagMatches(file.Etag()) == EtagMatch.Yes;
        }

        public static bool IfMatchHeaderDoesNotMatchEtag(this ICurrentHttpRequest request, IFubuFile file)
        {
            return request.IfMatch().EtagMatches(file.Etag()) == EtagMatch.No;
        }
    }
}