namespace FubuMVC.Core.Content
{
    public interface IContentRegistry
    {
        string ImageUrl(string name);
        string CssUrl(string name, bool optional);
        string ScriptUrl(string name, bool optional);
    }
}