using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Formatting;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Elements;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI.Scenarios
{
    public static class HtmlElementScenario<T> where T : class
    {
        public static ElementGenerator<T> For(Action<ScenarioDefinition> configure)
        {
            var definition = new ScenarioDefinition();
            configure(definition);

            var activators = definition.Activators().ToList();
            activators.Add(new ElementRequestActivator(new InMemoryFubuRequest(), definition.Naming));

            var stringifier = definition.Display.BuildStringifier();

            var formatter = new DisplayFormatter(definition.Services, stringifier);
            definition.Services.Add<IDisplayFormatter>(formatter);

            definition.Library.Import(new DefaultHtmlConventions().Library);


            var generator = ElementGenerator<T>.For(definition.Library, activators);
            generator.Model = definition.Model;

            return generator;
        }

        #region Nested type: ScenarioDefinition

        public class ScenarioDefinition
        {
            private readonly IList<ITagRequestActivator> _activators = new List<ITagRequestActivator>();
            private readonly DisplayConversionRegistry _display = new DisplayConversionRegistry();
            private readonly InMemoryServiceLocator _services = new InMemoryServiceLocator();
            private readonly HtmlConventionRegistry _registry = new HtmlConventionRegistry();

            public ScenarioDefinition()
            {
                _activators.Add(new ServiceLocatorTagRequestActivator(_services));
                Naming = new DefaultElementNamingConvention();

                _services.Add<ITypeResolver>(new TypeResolver());
            }

            public DisplayConversionRegistry Display
            {
                get { return _display; }
            }

            public IElementNamingConvention Naming { get; set; }

            public T Model { get; set; }

            public HtmlConventionRegistry Conventions
            {
                get
                {
                    return _registry;
                }
            }

            public InMemoryServiceLocator Services
            {
                get { return _services; }
            }

            public void Activator(ITagRequestActivator activator)
            {
                _activators.Add(activator);
            }

            public IEnumerable<ITagRequestActivator> Activators()
            {
                return _activators;
            }

            public HtmlConventionLibrary Library
            {
                get
                {
                    return _registry.Library;
                }
            }

            /// <summary>
            ///   Import an existing HtmlConventionRegistry
            /// </summary>
            /// <typeparam name = "T"></typeparam>
            public void Import<TRegistry>() where TRegistry : HtmlConventionRegistry, new()
            {
                var registry = new TRegistry();
                Library.Import(registry.Library);
            }

            /// <summary>
            ///   Use an HtmlConventionRegistry to add new html convention builders and modifiers
            /// </summary>
            /// <param name = "configure"></param>
            public void Configure(Action<HtmlConventionRegistry> configure)
            {
                var registry = new HtmlConventionRegistry();
                configure(registry);

                Library.Import(registry.Library);
            }
        }

        #endregion
    }
}