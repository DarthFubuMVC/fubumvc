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
        }
    }

    // Tested manually against FubuTestApplication
}