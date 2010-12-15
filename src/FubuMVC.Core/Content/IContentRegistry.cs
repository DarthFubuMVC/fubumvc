namespace FubuMVC.Core.Content
{
    public interface IContentRegistry
    {
        string ImageUrl(string name);
        string CssUrl(string name);
    }
}