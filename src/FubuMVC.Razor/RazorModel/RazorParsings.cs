using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.Registration;

namespace FubuMVC.Razor.RazorModel
{
    public class RazorParsings : IParsingRegistrations<IRazorTemplate>
    {
        private readonly Cache<string, Parsing> _parsings = new Cache<string, Parsing>();
        private readonly IViewParser _viewParser;

        public RazorParsings() : this(new ViewParser())
        {
        }

        public RazorParsings(IViewParser viewParser)
        {
            _viewParser = viewParser;
        }

        public void Parse(IRazorTemplate template)
        {
            var chunks = _viewParser.Parse(template.FilePath).ToList();

            _parsings[template.FilePath] = new Parsing
            {
                Master = chunks.Master(),
                ViewModelType = chunks.ViewModel(),
                Namespaces = chunks.Namespaces()
            };
        }

        public Parsing ParsingFor(IRazorTemplate template)
        {
            return _parsings[template.FilePath];
        }
    }
}