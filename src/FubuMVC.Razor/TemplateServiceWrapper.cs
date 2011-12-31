using RazorEngine.Templating;

namespace FubuMVC.Razor
{
    public class TemplateServiceWrapper : ITemplateServiceWrapper
    {
        private readonly ITemplateService _templateService;

        public TemplateServiceWrapper(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public ITemplateService TemplateService
        {
            get { return _templateService; }
        }
    }
}