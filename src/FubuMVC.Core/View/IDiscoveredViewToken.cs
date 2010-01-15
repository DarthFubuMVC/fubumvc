using System;

namespace FubuMVC.Core.View
{
    public interface IDiscoveredViewToken
    {
        Type ViewType { get; }
        Type ViewModelType { get; }
        IViewToken ToViewToken();
    }
}