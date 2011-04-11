using System.Linq;
using FubuCore;
using Spark.Parser.Markup;

namespace FubuMVC.Spark.Parsing
{
    public static class ElementNodeExtensions
    {
        public static string AttributeByName(this ElementNode elementNode, string name)
        {
            var attribute = elementNode.Attributes
                .Where(x => x.Name.EqualsIgnoreCase(name))
                .Select(x => x.Value)
                .FirstOrDefault();

            return attribute;
        }
    }
}