using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.Registration;

namespace FubuMVC.Razor.RazorModel
{
    public class RazorParsings : IParsingRegistrations<IRazorTemplate>
    {
        private readonly Cache<string, Parsing> _parsings = new Cache<string, Parsing>();
        private readonly IViewLoaderLocator _viewLoader;
        private readonly IViewParser _viewParser;

        public RazorParsings() : this(new ViewLoaderLocator(), new ViewParser())
        {
        }

        public RazorParsings(IViewLoaderLocator viewLoader, IViewParser viewParser)
        {
            _viewLoader = viewLoader;
            _viewParser = viewParser;
        }

        public void Parse(IRazorTemplate template)
        {
            var viewFile = _viewLoader.Locate(template);
            var chunks = _viewParser.Parse(viewFile).ToList();

            _parsings[template.FilePath] = new Parsing
            {
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel()
            };
        }

        public Parsing ParsingFor(IRazorTemplate template)
        {
            return _parsings[template.FilePath];
        }
    }
}