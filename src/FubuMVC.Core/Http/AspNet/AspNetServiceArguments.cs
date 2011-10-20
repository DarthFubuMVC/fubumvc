using System;
using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetServiceArguments : ServiceArguments
    {
        public AspNetServiceArguments(RequestContext requestContext)
        {
            With<AggregateDictionary>(new AspNetAggregateDictionary(requestContext));
            With(requestContext.HttpContext);

            With<ICurrentRequest>(new AspNetCurrentRequest(requestContext.HttpContext.Request));

            With<IStreamingData>(new AspNetStreamingData(requestContext.HttpContext));

            With<IHttpWriter>(new AspNetHttpWriter(requestContext.HttpContext.Response));
        }
    }
}