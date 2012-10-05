using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Tests.Assets.Content
{
    public class ContentPlanScenario : IContentPlanScenario
    {
        private readonly IList<ITransformerPolicy> _policies = new List<ITransformerPolicy>();
        private string _name;
        private readonly IList<AssetFile> _files = new List<AssetFile>();
        private AssetFileCombination _combination;


        void IContentPlanScenario.JsTransformer<T>(ActionType action, params string[] extensions)
        {
            var policy = new JavascriptTransformerPolicy<T>(action, extensions);
            _policies.Add(policy);
        }

        void IContentPlanScenario.CssTransformer<T>(ActionType action, params string[] extensions)
        {
            var policy = new CssTransformerPolicy<T>(action, extensions);
            _policies.Add(policy);
        }

        private static AssetFile fileFor(string name)
        {
            return new AssetFile(name){
                FullPath = "somewhere on the path"
            };
        }

        string IContentPlanScenario.SingleAssetFileName
        {
            set
            {
                _name = value;
                _files.Add(fileFor(value));
            }
        }

        void IContentPlanScenario.CombinationOfScriptsIs(string name, params string[] fileNames)
        {
            _name = name;

            var files = fileNames.Select(fileFor);
            _combination = new ScriptFileCombination(name, files);
            _files.AddRange(files);
        }

        void IContentPlanScenario.CombinationOfStyles(string name, params string[] fileNames)
        {
            _name = name;

            var files = fileNames.Select(fileFor);
            _combination = new StyleFileCombination(name, files);
            _files.AddRange(files);
        }

        void IContentPlanScenario.TransformerPolicy<T>()
        {
            var policy = new T();
            _policies.Add(policy);
        }

        public static ContentPlanShouldBeExpression For(Action<IContentPlanScenario> configure)
        {
            var scenario = new ContentPlanScenario();
            configure(scenario);

            var plan = scenario.BuildPlan();

            return new ContentPlanShouldBeExpression(plan);
        }

        public ContentPlan BuildPlan()
        {
            var library = new TransformerPolicyLibrary(_policies);
            var combinations = new AssetCombinationCache();
            if (_combination != null)
            {
                combinations.StoreCombination(_combination.MimeType, _combination);
            }

            var pipeline = new AssetFileGraph();
            _files.Each(f =>
            {
                var path = new AssetPath(AssetFileGraph.Application, f.Name, f.Folder);
                pipeline.AddFile(path, f);
            });

            var planner = new ContentPlanner(combinations, pipeline, library);
            return planner.BuildPlanFor(_name);
        }
    }
}