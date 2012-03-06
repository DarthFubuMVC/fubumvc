using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Razor.RazorModel
{
    public class ViewPathPolicy : ITemplatePolicy<IRazorTemplate>
    {
        private readonly Cache<string, string> _cache;
        public ViewPathPolicy()
        {
            _cache = new Cache<string, string>(getPrefix);
        }

        public bool Matches(IRazorTemplate template)
        {
            return template.ViewPath.IsEmpty();
        }

        public void Apply(IRazorTemplate template)
        {
            template.ViewPath = FubuCore.FileSystem.Combine(_cache[template.Origin], template.RelativePath());
        }

        private static string getPrefix(string origin)
        {
            return origin == FubuRazorConstants.HostOrigin ? string.Empty : "_{0}".ToFormat(origin);
        }
    }
}