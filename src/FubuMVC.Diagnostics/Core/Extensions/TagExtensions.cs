using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.Diagnostics.Core.Extensions
{
    public static class TagExtensions
    {
        private const string sourceControlUrlBase = "http://github.com/DarthFubuMVC/fubumvc/";
        private const string sourceControlUrlFormat = sourceControlUrlBase + "commit/{0}";

        // TODO -- there has to be a cleaner way to do this.
        public static HtmlTag FubuVersion(this IFubuPage page)
        {
            var fubuAssembly = typeof (FubuRegistry).Assembly;
            var attribute = fubuAssembly.GetAttribute<AssemblyDescriptionAttribute>();
            var version = (attribute == null) ? null : attribute.Description;
            var commitAttribute = fubuAssembly.GetAttribute<AssemblyTrademarkAttribute>();
            var commit = commitAttribute == null ? null : commitAttribute.Trademark;
            var versionUrl = commit.IsNotEmpty() ? sourceControlUrlFormat.ToFormat(commit) : sourceControlUrlBase;
            return
                new HtmlTag("span").Id("version-display").Text("version: ").Append(
                    new LinkTag(version, versionUrl).Attr("title", commit));
        }
    }
}