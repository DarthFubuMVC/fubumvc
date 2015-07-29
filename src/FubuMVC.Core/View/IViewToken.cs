using System;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Core.View
{
    public interface IViewToken
    {
        string Namespace { get; }
        Type ViewModel { get;  }

        string Name();
        void AttachViewModels(ViewTypePool types, ITemplateLogger logger);

        string FilePath { get; }
    }
}