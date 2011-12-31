using FubuMVC.Razor.RazorEngine.Compiler;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.RazorEngine
{
    public class RazorViewEntryFactory : IRazorViewEntryFactory
    {
        private readonly ITemplateServiceWrapper _templateService;

        public RazorViewEntryFactory(ITemplateServiceWrapper templateService)
        {
            _templateService = templateService;
        }

        public IRazorViewEntry CreateEntry(ViewDescriptor descriptor)
        {
            var compiledView = new CompiledViewEntry
            {
                Compiler = new ViewCompiler
                {
                    TemplateService = _templateService.TemplateService,
                    Descriptor = descriptor
                },
                Loader = descriptor.ViewLoader,
            };

            return compiledView;
        }
    }
}