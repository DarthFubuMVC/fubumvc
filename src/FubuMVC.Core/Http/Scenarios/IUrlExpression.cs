using System;
using System.Linq.Expressions;
using FubuMVC.Core.Http.Owin;

namespace FubuMVC.Core.Http.Scenarios
{
    public interface IUrlExpression
    {
        SendExpression Action<T>(Expression<Action<T>> expression);
        SendExpression Url(string relativeUrl);
        SendExpression Input<T>(T input = null) where T : class;

        SendExpression Json<T>(T input) where T : class;
        SendExpression Xml<T>(T input) where T : class;

        SendExpression FormData<T>(T input) where T : class;
    }

    public class SendExpression
    {
        private readonly OwinHttpRequest _request;

        public SendExpression(OwinHttpRequest request)
        {
            _request = request;
        }

        public SendExpression ContentType(string contentType)
        {
            _request.ContentType(contentType);
            return this;
        }

        public SendExpression Accepts(string accepts)
        {
            _request.Accepts(accepts);
            return this;
        }

        public SendExpression Etag(string etag)
        {
            _request.ReplaceHeader(HttpRequestHeaders.IfNoneMatch, etag);
            return this;
        }
    }

}