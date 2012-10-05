using System;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Resources.PathBased
{
    public class ResourcePath : IMakeMyOwnUrl
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

    }
}