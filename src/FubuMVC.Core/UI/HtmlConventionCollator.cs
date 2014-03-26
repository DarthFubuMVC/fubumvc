using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI
{
    public class HtmlConventionCollator
    {
        public static void BuildHtmlConventionLibrary(HtmlConventionLibrary library, AccessorRules rules)
        {
            library.Import(new DefaultHtmlConventions().Library);

            var visitor = new Visitor(rules);

            library.For<ElementRequest>().AcceptVisitor(visitor);
        }

        public class Visitor : ITagLibraryVisitor<ElementRequest>
        {
            private readonly AccessorRules _rules;
            private string _category;

            public Visitor(AccessorRules rules)
            {
                _rules = rules;
            }

            public void Category(string name, TagCategory<ElementRequest> category)
            {
                _category = name;
            }

            public void BuilderSet(string profile, BuilderSet<ElementRequest> builders)
            {
                var policy = new AccessorOverrideElementBuilderPolicy(_rules, _category, profile);
                builders.InsertFirst(policy);
            }
        }
    }
}