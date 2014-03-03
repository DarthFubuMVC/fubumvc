using System;
using System.Web.Routing;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Resources.PathBased
{
    public class ResourcePath : IMakeMyOwnUrl, IRankMeLast
    {
        public static readonly string UrlSuffix =
            "{Part0}/{Part1}/{Part2}/{Part3}/{Part4}/{Part5}/{Part6}/{Part7}/{Part8}/{Part9}";

        private readonly string _path;

        public ResourcePath(string path)
        {
            _path = path;
        }

        public string Path
        {
            get { return _path; }
        }

        public virtual string ToUrlPart(string basePattern)
        {
            var baseUrl = basePattern.Contains(UrlSuffix) ? basePattern.Replace(UrlSuffix, "") : basePattern;
            return (baseUrl.TrimEnd('/') + "/" + _path.TrimStart('/')).Trim('/');
        }

        public static void AddResourcePathInputs(IRouteDefinition route)
        {
            route.Append(UrlSuffix);


            route.RegisterRouteCustomization(r =>
            {
                if (r.Defaults == null)
                {
                    r.Defaults = new RouteValueDictionary();
                }

                r.Defaults.Add("Part0", null);
                r.Defaults.Add("Part1", null);
                r.Defaults.Add("Part2", null);
                r.Defaults.Add("Part3", null);
                r.Defaults.Add("Part4", null);
                r.Defaults.Add("Part5", null);
                r.Defaults.Add("Part6", null);
                r.Defaults.Add("Part7", null);
                r.Defaults.Add("Part8", null);
                r.Defaults.Add("Part9", null);
            });
        }
    }
}