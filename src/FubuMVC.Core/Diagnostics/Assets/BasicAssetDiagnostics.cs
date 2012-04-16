using System;
using System.IO;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Runtime;
using System.Linq;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.Assets
{

    public abstract class AssetDiagnosticRequest
    {
        [QueryString]
        public string Name { get; set; }
    }

    public class ContentPlanRequest : AssetDiagnosticRequest{}
    public class AssetLogRequest : AssetDiagnosticRequest { }

    [FubuDiagnostics("Basic Asset Graph Information")]
    public class BasicAssetDiagnostics
    {
        private readonly IAssetCombinationCache _cache;
        private readonly IContentPlanCache _contentPlanCache;
        private readonly AssetLogsCache _logs;
        private readonly IUrlRegistry _urlRegistry;
        private readonly IAssetRequirements _requirements;
        private readonly IAssetTagPlanCache _plans;

        public BasicAssetDiagnostics(IAssetCombinationCache cache, 
            IContentPlanCache contentPlanCache,
            AssetLogsCache logs,
            IUrlRegistry urlRegistry,
            IAssetRequirements requirements,
            IAssetTagPlanCache plans)
        {
            _cache = cache;
            _contentPlanCache = contentPlanCache;
            _logs = logs;
            _urlRegistry = urlRegistry;
            _requirements = requirements;
            _plans = plans;
        }

        [FubuDiagnostics("View asset logs")]
        public HtmlDocument Assets()
        {
            var assetList = new HtmlTag("ul");

            _logs.Entries
                .OrderByDescending(l => l.Logs.Count)
                .ThenBy(l => l.Name)
                .Each(log => assetList.Append(new HtmlTag("li")
                                                   .Append("a", tag => tag.Attr("href", _urlRegistry.UrlFor(new AssetLogRequest { Name = log.Name }))
                                                       .Text(log.Name))));
            return DiagnosticHtml.BuildDocument(_urlRegistry, "Asset Logs", assetList);
        }

        [FubuDiagnostics("log for asset", ShownInIndex = false)]
        public HtmlDocument AssetLogs(AssetLogRequest request)
        {
            var logs = _logs.FindByName(request.Name);

            var assetList = new HtmlTag("ul");


            _requirements.Require(request.Name);
            var keys = _requirements.DequeueAssetsToRender();
            var plan = keys.Select(k => _plans.PlanFor(k));

            var dependencies = new HtmlTag("ul");

            plan.Each(p =>
            {
                p.Subjects.Each(s => dependencies.Append(new HtmlTag("li").Append(new LinkTag(s.Name, _urlRegistry.UrlFor(new AssetLogRequest() { Name = s.Name })))));
            });
            assetList.Add("li").Text("Order written").Append(dependencies);

            logs.Logs
                .Each(log => assetList.Append(new HtmlTag("li")
                                                       .Text(log.Message)).Append("ul/li", tag => tag.Text(log.Provenance)));

            return DiagnosticHtml.BuildDocument(_urlRegistry, "Logs for " + request.Name, assetList);

        }

        [FubuDiagnostics("View content plans")]
        public HtmlDocument ContentPlans()
        {
            var assetList = new HtmlTag("ul");

            _logs.Entries
                .OrderByDescending(l => l.Logs.Count)
                .ThenBy(l => l.Name)
                .Each(log => assetList.Append(new HtmlTag("li")
                                                   .Append("a", tag => tag.Attr("href", _urlRegistry.UrlFor(new ContentPlanRequest { Name = log.Name }))
                                                       .Text(log.Name))));
            return DiagnosticHtml.BuildDocument(_urlRegistry, "Content Plans", assetList);
        }

        [FubuDiagnostics("content plan for {asset}", ShownInIndex = false)]
        public string get_asset_Name(ContentPlanRequest request)
        {
            try
            {
                var plan = _contentPlanCache.PlanFor(request.Name);
                var description = new ContentPlanPreviewer();
                plan.AcceptVisitor(description);

                var writer = new StringWriter();
                description.Descriptions.Each(x => writer.WriteLine(x));

                return writer.ToString();
            }
            catch (ArgumentOutOfRangeException e)
            {
                return e.Message;
            }
          
 
        }

        [FubuDiagnostics("View combinations")]
        public string get_assets_combinations()
        {
            var writer = new StringWriter();

            writer.Write("*" + MimeType.Javascript + "*");
            _cache.OrderedListOfCombinations(MimeType.Javascript).Each(combo =>
            {
                writer.WriteLine(combo.Name + ":  " + combo.Files.Select(x => x.Name).Join(", "));
            });

            writer.WriteLine();
            writer.Write("*" + MimeType.Css + "*");
            _cache.OrderedListOfCombinations(MimeType.Css).Each(combo =>
            {
                writer.WriteLine(combo.Name + ":  " + combo.Files.Select(x => x.Name).Join(", "));
            });

            return writer.ToString();
        }

    }
}