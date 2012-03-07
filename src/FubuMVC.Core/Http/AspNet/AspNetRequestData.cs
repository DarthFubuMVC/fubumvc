using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using FubuCore.Binding.Values;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetRequestData : RequestData
    {
        public AspNetRequestData(RequestContext context)
        {
            /*
             *  1.) Route
             *  2.) Request
             *  3.) File
             *  4.) Header
             *  5.) RequestProperty
             */

            AddValues(new RouteDataValues(context.RouteData));

            var request = context.HttpContext.Request;
            addValues(RequestDataSource.Request, key => request[key], () => keysForRequest(request));

            var files = request.Files;
            addValues(RequestDataSource.File, key => files[key], () => files.AllKeys);

            addValues(RequestDataSource.Header, key => request.Headers[key], () => request.Headers.AllKeys);

            AddValues(new RequestPropertyValueSource(context.HttpContext.Request));


        }

        private void addValues(RequestDataSource source, Func<string, object> finder, Func<IEnumerable<string>> findKeys)
        {
            var valueSource = new GenericValueSource(source.ToString(), finder, findKeys);
            AddValues(valueSource);
        }

        private static IEnumerable<string> keysForRequest(HttpRequestBase request)
        {
            foreach (var key in request.QueryString.AllKeys)
            {
                yield return key;
            }

            foreach (var key in request.Form.AllKeys.Where(x => x != null))
            {
                yield return key;
            }
        }
    }
}