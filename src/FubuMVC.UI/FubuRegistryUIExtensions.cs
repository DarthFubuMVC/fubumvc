using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.UI.Configuration;

namespace FubuMVC.UI
{
    public static class FubuRegistryUIExtensions
    {
        public static void UseDefaultHtmlConventions(this FubuRegistry registry)
        {
            registry.Policies.Add<HtmlConventionCompiler>();
        }

        public static void HtmlConvention<T>(this FubuRegistry registry) where T : HtmlConventionRegistry, new()
        {
            registry.HtmlConvention(new T());
        }

        public static void HtmlConvention(this FubuRegistry registry, HtmlConventionRegistry conventions)
        {
            registry.Services(x => x.AddService(conventions));

            registry.Policies.Add<HtmlConventionCompiler>();
        }

        public static void HtmlConvention(this FubuRegistry registry, Action<HtmlConventionRegistry> configure)
        {
            var conventions = new HtmlConventionRegistry();
            configure(conventions);

            registry.HtmlConvention(conventions);
        }


        public static void StringConversions<T>(this FubuRegistry registry) where T : DisplayConversionRegistry, new()
        {
            var conversions = new T();

            addStringConversions(conversions, registry);
        }

        private static void addStringConversions(DisplayConversionRegistry conversions, FubuRegistry registry)
        {
            var policy = new LambdaConfigurationAction(graph =>
            {
                graph.Services.SetServiceIfNone<Stringifier>(new Stringifier());
                var stringifier = graph.Services.FindAllValues<Stringifier>().First();

                
                conversions.Configure(stringifier);
            });
            registry.Policies.Add(policy);
        }

        public static void StringConversions(this FubuRegistry registry, Action<DisplayConversionRegistry> configure)
        {
            var conversions = new DisplayConversionRegistry();
            configure(conversions);

            addStringConversions(conversions, registry);
        }
    }


}