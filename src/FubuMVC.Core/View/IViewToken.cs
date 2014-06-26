using System;
using System.Reflection;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Core.View
{
    public interface IViewToken
    {
        string Namespace { get; }
        Type ViewModel { get;  }
        string ProfileName { get; set; }

        /// <summary>
        /// The Bottle name or "Application" where this view is from
        /// </summary>
        string Origin { get; }
        string Name();
        void AttachViewModels(Assembly defaultAssembly, ViewTypePool types, ITemplateLogger logger);

        string FilePath { get; }
    }
}