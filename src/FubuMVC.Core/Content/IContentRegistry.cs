using System;

namespace FubuMVC.Core.Content
{
    [Obsolete("Being rendered obsolete by newer asset pipeline")]
    public interface IContentRegistry
    {
        string ImageUrl(string name);
        string CssUrl(string name, bool optional);
        string ScriptUrl(string name, bool optional);
    }
}