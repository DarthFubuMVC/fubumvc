using FubuMVC.Core.Assets.Content;

namespace FubuMVC.Tests.Assets.Content
{
    public interface IContentPlanScenario
    {
        void JsTransformer<T>(ActionType action, params string[] extensions) where T : ITransformer;
        void CssTransformer<T>(ActionType action, params string[] extensions) where T : ITransformer;
        string SingleAssetFileName { set; }
        void CombinationOfScriptsIs(string name, params string[] fileNames);
        void CombinationOfStyles(string name, params string[] fileNames);

        void TransformerPolicy<T>() where T : ITransformerPolicy, new();
    }
}