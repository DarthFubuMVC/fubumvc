using System;
using FubuMVC.Razor.RazorEngine.Compiler;
using FubuMVC.Razor.RazorEngine.Parsing;
using RazorEngine.Compilation;
using RazorEngine.Templating;

namespace FubuMVC.Razor.RazorEngine
{
    public class CompiledViewEntry : IRazorViewEntry
    {
        public Guid ViewId { get { return Compiler.GeneratedViewId; } }
        public RazorViewDescriptor Descriptor { get; set; }
        public ViewLoader Loader { get; set; }
        public ViewCompiler Compiler { get; set; }
        public ITemplateService TemplateService { get; set; }
        public ICompilerServiceFactory CompilerFactory { get; set; }

        public string SourceCode
        {
            get { return Compiler.SourceCode; }
        }

        public ITemplate CreateInstance()
        {
            return TemplateService.Resolve(ViewId.ToString());
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