using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Diagnostics.Notifications;
using HtmlTags;

namespace FubuMVC.Diagnostics.Core.Extensions
{
    public static class PageExtensions
    {
        public static void Notification<T>(this IFubuPage page, T notification)
            where T : class, INotificationModel
        {
            page.Get<PartialInvoker>().InvokeObject(notification);
        }
    }

    public static class TagExtensions
    {
        private const string sourceControlUrlBase = "http://github.com/DarthFubuMVC/fubumvc/";
        private const string sourceControlUrlFormat = sourceControlUrlBase + "commit/{0}";

        public static HtmlTag ImageFor(this IFubuPage page, string path)
        {
            var url = page.Urls.UrlForAsset(AssetFolder.images, path);
            return new HtmlTag("img").Attr("src", url);
        }

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