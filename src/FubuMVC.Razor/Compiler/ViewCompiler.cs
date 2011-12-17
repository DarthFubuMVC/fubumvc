using System;
using System.Collections.Generic;
using System.Web.Razor.Parser.SyntaxTree;
using RazorEngine.Templating;

namespace FubuMVC.Razor.Compiler
{
    public abstract class ViewCompiler
    {
        protected ViewCompiler()
        {
            GeneratedViewId = Guid.NewGuid();
        }

        public string BaseClass { get; set; }
        public RazorViewDescriptor Descriptor { get; set; }
        public string ViewClassFullName { get; set; }

        public string SourceCode { get; set; }
        public Type CompiledType { get; set; }
        public Guid GeneratedViewId { get; set; }

        public bool Debug { get; set; }
        public IEnumerable<string> UseNamespaces { get; set; }
        public IEnumerable<string> UseAssemblies { get; set; }

        public string TargetNamespace
        {
            get { return Descriptor == null ? null : Descriptor.TargetNamespace; }
        }

        public abstract void CompileView(IEnumerable<IList<Span>> viewTemplates, IEnumerable<IList<Span>> allResources);

        public abstract void GenerateSourceCode(IEnumerable<IList<Span>> viewTemplates,
                                                IEnumerable<IList<Span>> allResources);

        public ITemplate CreateInstance()
        {
            return (ITemplate) Activator.CreateInstance(CompiledType);
        }
    }
}