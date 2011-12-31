using System;
using FubuMVC.Razor.RazorModel;
using ITemplate = RazorEngine.Templating.ITemplate;

namespace FubuMVC.Razor.RazorEngine
{
    public interface IRazorViewEntry
    {
        Guid ViewId { get; }
        ITemplate CreateInstance();
        void ReleaseInstance(ITemplate view);
        bool IsCurrent();
    }
}