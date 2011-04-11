using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using Spark.Parser;
using Spark.Parser.Markup;

namespace FubuMVC.Spark.Parsing
{
    public interface IElementNodeExtractor
    {
        IEnumerable<ElementNode> Extract(string content);
        IEnumerable<ElementNode> ExtractBy(string content, Func<ElementNode, bool> predicate);
        IEnumerable<ElementNode> ExtractByName(string content, string name);
    }

    public class ElementNodeExtractor : IElementNodeExtractor
    {
        public IEnumerable<ElementNode> Extract(string content)
        {
            var grammar = new MarkupGrammar();
            var position = source(content);
            var parseResult = grammar.Nodes(position);

            return parseResult.Value.OfType<ElementNode>();
        }

        // TODO: Rip out
        public IEnumerable<ElementNode> ExtractBy(string content, Func<ElementNode, bool> predicate)
        {
            return Extract(content).Where(predicate);
        }

        // TODO: Rip out
        public IEnumerable<ElementNode> ExtractByName(string content, string name)
        {
            return ExtractBy(content, n => n.Name.EqualsIgnoreCase(name));
        }

        private static Position source(string content)
        {
            return new Position(new SourceContext(content));
        }
    }
}