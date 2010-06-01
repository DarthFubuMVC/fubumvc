using System;

namespace FubuMVC.Core.Urls
{
    public interface IModelUrl
    {
        string Category { get; }
        Type InputType { get; }
        string CreateUrl(object input);
        void RootUrlAt(string baseUrl);
    }
}