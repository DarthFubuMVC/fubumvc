using System;
using System.IO;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Combination;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Diagnostics.Runtime.Assets
{

    public abstract class AssetDiagnosticRequest
    {
        public string Name { get; set; }
    }

    public class AssetNamesRequest : AssetDiagnosticRequest{}
    public class AssetFilesRequest : AssetDiagnosticRequest{}
    public class ContentPlanRequest : AssetDiagnosticRequest{}
    public class AssetSourcesRequest : AssetDiagnosticRequest{}

    public class BasicAssetDiagnostics
    {
        private readonly IAssetCombinationCache _cache;
        private readonly IContentPlanCache _contentPlanCache;
        private readonly IAssetPipeline _pipeline;

        public BasicAssetDiagnostics(IAssetCombinationCache cache, IContentPlanCache contentPlanCache, IAssetPipeline pipeline)
        {
            _cache = cache;
            _contentPlanCache = contentPlanCache;
            _pipeline = pipeline;
        }

        public string get_asset_Name(ContentPlanRequest request)
        {
            var plan = _contentPlanCache.PlanFor(request.Name);
            var description = new ContentPlanPreviewer();
            plan.AcceptVisitor(description);

            var writer = new StringWriter();
            description.Descriptions.Each(x => writer.WriteLine(x));

            return writer.ToString();
        }

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

        private string toCombinationReport(AssetDiagnosticRequest request, Func<AssetFile, string> description)
        {
            var plan = _contentPlanCache.PlanFor(request.Name);

            if (plan == null)
            {
                return "Not found";
            }

            var writer = new StringWriter();


            plan.GetAllSources().SelectMany(x => x.Files).Each(x => writer.WriteLine(description(x)));

            return writer.ToString();
        }

        public string get_asset_Name_contents(AssetNamesRequest request) 
        {
            return toCombinationReport(request, file => file.Name);
        }

        public string get_asset_Name_files(AssetFilesRequest request)
        {
            return toCombinationReport(request, file => file.FullPath);
        }

        public string get_asset_Name_sources(AssetSourcesRequest request)
        {
            return toCombinationReport(request, file =>
            {
                var path = _pipeline.AssetPathOf(file);
                return path.ToFullName();
            });
        }


    }
}