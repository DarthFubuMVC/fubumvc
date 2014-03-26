using System.ComponentModel;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.UI.Elements;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI
{
    [Description("Compiles and activates the Html Conventions")]
    [ConfigurationType(ConfigurationType.Policy)]
    public class HtmlConventionActivation : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var rules = graph.Settings.Get<AccessorRules>();
            var library = graph.Settings.Get<HtmlConventionLibrary>();
            library.Import(new DefaultHtmlConventions().Library);

            var visitor = new Visitor(rules);

            library.For<ElementRequest>().AcceptVisitor(visitor);

            graph.Services.SetServiceIfNone(typeof(HtmlConventionLibrary), ObjectDef.ForValue(library));
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