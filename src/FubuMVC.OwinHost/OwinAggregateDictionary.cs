using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    // TODO -- this whole thing is duplication w/ AspNetAggregateDictionary.  DRY it baby!
    public class OwinAggregateDictionary : AggregateDictionary
    {
        public OwinAggregateDictionary(RouteData routeData, OwinRequestBody body)
        {
            Func<string, object> locator = key =>
            {
                object found;
                routeData.Values.TryGetValue(key, out found);
                return found;
            };

            AddLocator(RequestDataSource.Route.ToString(), locator, () => routeData.Values.Keys);
            AddLocator(RequestDataSource.Request.ToString(), key => findRequestValue(key, body), () => valuesForRequest(body).Keys);

            addDictionaryLocator("Query string", body.Querystring());
            addDictionaryLocator("Form Post", body.FormData ?? new Dictionary<string, string>());
            addDictionaryLocator(RequestDataSource.Header.ToString(), body.Headers());
        }

        private void addDictionaryLocator(string name, IDictionary<string, string> dictionary)
        {
            Func<string, object> locator = key => dictionary.ContainsKey(key) ? dictionary[key] : null;

            AddLocator(name, locator, () => dictionary.Keys);
        }

        private static object findRequestValue(string key, OwinRequestBody body)
        {
            var values = valuesForRequest(body);
            string found;
            values.TryGetValue(key, out found);
            return found;
        }

        private static IDictionary<string, string> valuesForRequest(OwinRequestBody body)
        {
            var dictionary = new Dictionary<string, string>();
            var queryString = body.Querystring();
            foreach(var key in queryString.Keys)
            {
                dictionary.Add(key, queryString[key]);
            }

            IDictionary<string, string> formData = body.FormData ?? new Dictionary<string, string>();
            foreach (var key in formData.Keys.Where(x => x != null))
            {
                if(!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, formData[key]);
                }
            }

            return dictionary;
        }
    }
}