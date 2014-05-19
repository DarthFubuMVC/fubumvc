using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Http.Owin.Middleware.StaticFiles
{
    public class StaticFileMiddleware : FilteredOwinMiddleware
    {
        private readonly IFubuApplicationFiles _files;
        private readonly AssetSettings _settings;

        public StaticFileMiddleware(Func<IDictionary<string, object>, Task> inner, IFubuApplicationFiles files, AssetSettings settings) : base(inner)
        {
            _files = files;
            _settings = settings;
        }

        public override MiddlewareContinuation Invoke(IHttpRequest request, IHttpResponse response)
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
                return new WriteFileHeadContinuation(response, file, HttpStatusCode.OK);
            }

            if (request.IfMatchHeaderDoesNotMatchEtag(file))
            {
                return new WriteStatusCodeContinuation(response, HttpStatusCode.PreconditionFailed, "If-Match test failed"); 
            }

            if (request.IfNoneMatchHeaderMatchesEtag(file))
            {
                return new WriteFileHeadContinuation(response, file, HttpStatusCode.NotModified);
            }

            if (request.IfModifiedSinceHeaderAndNotModified(file))
            {
                return new WriteFileHeadContinuation(response, file, HttpStatusCode.NotModified);
            }

            if (request.IfUnModifiedSinceHeaderAndModifiedSince(file))
            {
                return new WriteStatusCodeContinuation(response, HttpStatusCode.PreconditionFailed, "File has been modified");
            }

            // Write headers here.

            return new WriteFileContinuation(response, file, _settings);
        }

        
    }

    public static class CurrentHttpRequestExtensions
    {
        public static bool IfUnModifiedSinceHeaderAndModifiedSince(this IHttpRequest request, IFubuFile file)
        {
            var ifUnModifiedSince = request.IfUnModifiedSince();
            return ifUnModifiedSince.HasValue && file.LastModified() > ifUnModifiedSince.Value;
        }

        public static bool IfModifiedSinceHeaderAndNotModified(this IHttpRequest request, IFubuFile file)
        {
            var ifModifiedSince = request.IfModifiedSince();
            return ifModifiedSince.HasValue && file.LastModified().ToUniversalTime() <= ifModifiedSince.Value;
        }

        public static bool IfNoneMatchHeaderMatchesEtag(this IHttpRequest request, IFubuFile file)
        {
            return request.IfNoneMatch().EtagMatches(file.Etag()) == EtagMatch.Yes;
        }

        public static bool IfMatchHeaderDoesNotMatchEtag(this IHttpRequest request, IFubuFile file)
        {
            return request.IfMatch().EtagMatches(file.Etag()) == EtagMatch.No;
        }
    }
}