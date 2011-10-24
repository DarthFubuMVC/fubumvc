using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class OwinAggregateDictionary : AggregateDictionary
    {
        public OwinAggregateDictionary(RouteData routeData, Request request)
        {
            // TODO -- this is duplication w/ AspNetAggregateDictionary.  DRY it baby!
            Func<string, object> locator = key =>
            {
                object found;
                routeData.Values.TryGetValue(key, out found);
                return found;
            };


            AddLocator(RequestDataSource.Route.ToString(), locator, () => routeData.Values.Keys);

            addDictionaryLocator("Query string", request.Query);
            addDictionaryLocator("Form Post", request.Post);

            addDictionaryLocator(RequestDataSource.Header.ToString(), request.Headers);
        }

        private void addDictionaryLocator(string name, IDictionary<string, string> dictionary)
        {
            Func<string, object> locator = key => dictionary.ContainsKey(key) ? dictionary[key] : null;

            AddLocator(name, locator, () => dictionary.Keys);
        }
    }
}