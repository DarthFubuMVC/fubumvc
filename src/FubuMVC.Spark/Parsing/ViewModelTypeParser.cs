using System;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.Parsing
{
    public interface IViewModelTypeParser
    {
        Type Parse(string templateContent);
    }

    public class ViewModelTypeParser : IViewModelTypeParser
    {
        private readonly IElementNodeExtractor _nodeExtractor;
        private readonly TypePool _typePool;

        public ViewModelTypeParser(IElementNodeExtractor nodeExtractor, TypePool typePool)
        {
            _nodeExtractor = nodeExtractor;
            _typePool = typePool;
        }

        public Type Parse(string templateContent)
        {
            var typeName = extractTypeName(templateContent);
            return tryGetType(typeName);
        }


        // Log ambiguity or return "potential types"?
        private Type tryGetType(string fullName)
        {
            var matchingTypes = _typePool.TypesWithFullName(fullName);            
            return matchingTypes.Count() == 1 ? matchingTypes.First() : null;
        }

        private string extractTypeName(string content)
        {
            return _nodeExtractor
                .ExtractByName(content, "viewdata")
                .Select(n => n.AttributeByName("model"))
                .Where(v => v != null)
                .FirstOrDefault();
        }
    }
}