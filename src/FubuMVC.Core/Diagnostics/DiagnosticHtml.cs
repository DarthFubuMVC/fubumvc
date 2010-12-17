using System;
using System.IO;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics
{
    public static class DiagnosticHtml
    {
        private const string sourceControlUrlBase = "http://github.com/DarthFubuMVC/fubumvc/";
        private const string sourceControlUrlFormat = sourceControlUrlBase + "commit/{0}";

        public static HtmlDocument BuildDocument(IUrlRegistry urls, string title, params HtmlTag[] tags)
        {
            string css = GetDiagnosticCss();

            var realTitle = "FubuMVC: " + title;

            var document = new HtmlDocument();
            document.Title = realTitle;

            var mainDiv = new HtmlTag("div").AddClass("main");
            mainDiv.Add("h2").Text("FubuMVC Diagnostics").Child(buildVersionTag());
            var navBar = mainDiv.Add("div").AddClass("homelink");
            if (urls != null) navBar.AddChildren(new LinkTag("Home", urls.UrlFor<BehaviorGraphWriter>(w => w.Index())));
            navBar.Add("span").Text(" > " + title);
            document.Add(mainDiv);

            mainDiv.AddChildren(tags);

            document.AddStyle(css);

            return document;
        }

        public static string GetDiagnosticCss()
        {
            return GetResourceText(typeof(BehaviorGraphWriter), "diagnostics.css");
        }

        public static string GetResourceText(Type type, string filename)
        {
            var stream = type.Assembly.GetManifestResourceStream(type, filename);
            if (stream == null) return String.Empty;
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static HtmlTag buildVersionTag()
        {
            var fubuAssembly = typeof(BehaviorGraphWriter).Assembly;
            var attribute = fubuAssembly.GetAttribute<AssemblyDescriptionAttribute>();
            var version = (attribute == null) ? null : attribute.Description;
            var commitAttribute = fubuAssembly.GetAttribute<AssemblyTrademarkAttribute>();
            var commit = commitAttribute == null ? null : commitAttribute.Trademark;
            var versionUrl = commit.IsNotEmpty() ? sourceControlUrlFormat.ToFormat(commit) : sourceControlUrlBase;
            return new HtmlTag("span").Id("version-display").Text("version: ").Child(new LinkTag(version, versionUrl).Attr("title", commit));
        }
    }
}