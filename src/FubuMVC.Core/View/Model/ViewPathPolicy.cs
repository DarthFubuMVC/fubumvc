using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    public class ViewPathPolicy<T> : ITemplatePolicy<T> where T : ITemplateFile
    {
        private readonly Cache<string, string> _cache;
        public ViewPathPolicy()
        {
            _cache = new Cache<string, string>(getPrefix);
        }

        public bool Matches(T template)
        {
            return template.ViewPath.IsEmpty();
        }

        public void Apply(T template)
        {
            template.ViewPath = FileSystem.Combine(_cache[template.Origin], template.RelativePath());
        }

        private static string getPrefix(string origin)
        {
            return origin == ContentFolder.Application ? string.Empty : "_{0}".ToFormat(origin);
        }
    }
}