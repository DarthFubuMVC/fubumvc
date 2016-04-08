using System.Threading.Tasks;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Web;
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
         
        public static HtmlConventionLibrary BuildHtmlConventions(BehaviorGraph graph)
        {
            var validation = graph.Settings.Get<ValidationSettings>();

            var rules = graph.Settings.Get<AccessorRules>();
            var library = graph.Settings.Get<HtmlConventionLibrary>();


            if (validation.Enabled)
            {
                library.Import(new ValidationHtmlConventions().Library);
            }

            BuildHtmlConventionLibrary(library, rules);

            return library;
        }
    }
}