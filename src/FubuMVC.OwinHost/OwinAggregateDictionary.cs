using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinAggregateDictionary : AggregateDictionary
    {
        public OwinAggregateDictionary(RouteData routeData, OwinRequestBody body)
        {
            // TODO -- this is duplication w/ AspNetAggregateDictionary.  DRY it baby!
            Func<string, object> locator = key =>
            {
                object found;
                routeData.Values.TryGetValue(key, out found);
                return found;
            };


            AddLocator(RequestDataSource.Route.ToString(), locator, () => routeData.Values.Keys);

            addDictionaryLocator("Query string", body.Querystring());
            addDictionaryLocator("Form Post", body.FormData ?? new Dictionary<string, string>());

            addDictionaryLocator(RequestDataSource.Header.ToString(), body.Headers());
        }

        private void addDictionaryLocator(string name, IDictionary<string, string> dictionary)
        {
            Func<string, object> locator = key => dictionary.ContainsKey(key) ? dictionary[key] : null;

            AddLocator(name, locator, () => dictionary.Keys);
        }
    }
}