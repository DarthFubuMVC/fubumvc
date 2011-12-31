using System;
using FubuMVC.Razor.RazorEngine.Compiler;
using FubuMVC.Razor.RazorEngine.Parsing;
using ITemplate = RazorEngine.Templating.ITemplate;

namespace FubuMVC.Razor.RazorEngine
{
    public class CompiledViewEntry : IRazorViewEntry
    {
        public Guid ViewId { get { return Compiler.GeneratedViewId; } }
        public IViewLoader Loader { get; set; }
        public ViewCompiler Compiler { get; set; }

        public ITemplate CreateInstance()
        {
            return Compiler.CreateInstance();
        }

        public void ReleaseInstance(ITemplate view)
        {
        }

        public bool IsCurrent()
        {
            return Loader.IsCurrent();
        }
    }
}