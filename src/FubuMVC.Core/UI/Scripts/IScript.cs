using HtmlTags;

namespace FubuMVC.Core.UI.Scripts
{
    public interface IScript : IScriptObject
    {
        string ReadAll();
        HtmlTag CreateScriptTag();

        bool ShouldBeAfter(IScript script);
    }
}