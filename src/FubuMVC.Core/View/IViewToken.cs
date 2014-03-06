using System;

namespace FubuMVC.Core.View
{
    public interface IViewToken
    {
        string Namespace { get; }
        Type ViewModel { get;  }
        string ProfileName { get; set; }
        string Name();
    }
}