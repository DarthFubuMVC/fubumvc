using System;
using FubuCore;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation;
using HtmlTags;

namespace FubuMVC.Tests.Validation.Web.UI
{
    public class ValidationElementModifierScenario<T>
        where T : IElementModifier, new()
    {
        public ElementRequest Request { get; private set; }
        public HtmlTag Tag { get; private set; }
        public T Modifier { get; private set; }

        public static ValidationElementModifierScenario<T> For(Action<ScenarioDefinition> configure)
        {
            var definition = new ScenarioDefinition();
            configure(definition);

            if(definition.Request.CurrentTag == null)
            {
                definition.Request.ReplaceTag(definition.Tag);
            }

            definition.Request.Attach(definition.Services);

            definition.Modifier.Modify(definition.Request);

            return new ValidationElementModifierScenario<T>
            {
                Tag = definition.Tag,
                Request = definition.Request,
                Modifier = definition.Modifier
            };
        }

        public class ScenarioDefinition
        {
            public ScenarioDefinition()
            {
                Tag = new HtmlTag("input");
                Services = new InMemoryServiceLocator();
                Modifier = new T();
                Services.Add(ValidationGraph.BasicGraph());
                Services.Add<ITypeResolver>(new TypeResolver());
            }

            public InMemoryServiceLocator Services { get; private set; }
            public ElementRequest Request { get; set; }
            public HtmlTag Tag { get; set; }
            public T Modifier { get; set; }
        }
    }
}