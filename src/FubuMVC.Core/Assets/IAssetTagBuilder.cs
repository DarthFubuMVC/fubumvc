using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.Assets
{
    public interface IAssetTagBuilder
    {
        IEnumerable<HtmlTag> BuildScriptTags(IEnumerable<string> scripts);
        IEnumerable<HtmlTag> BuildStylesheetTags(IEnumerable<string> scripts);
        string FindImageUrl(string urlOrFilename);

        void RequireScript(params string[] scripts);
    }
}