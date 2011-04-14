using System.Collections.Generic;

namespace FubuMVC.Spark.Tokenization.Parsing
{
    public interface IViewModelTypeParser
    {
        string Parse(string templateContent);
    }

    // TODO: Generalize this
    public class ViewModelTypeParser : IViewModelTypeParser
    {
        private readonly IElementNodeExtractor _nodeExtractor;
        public ViewModelTypeParser(IElementNodeExtractor nodeExtractor)
        {
            _nodeExtractor = nodeExtractor;
        }

        public string Parse(string templateContent)
        {
            return _nodeExtractor
                .ExtractByName(templateContent, "viewdata")
                .FirstValue(n => n.AttributeByName("model"));
        }
    }
}