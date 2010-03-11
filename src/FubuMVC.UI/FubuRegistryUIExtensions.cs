using System;
using System.Linq;
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

        public static void StringConversions(this FubuRegistry registry, Action<Stringifier> configure)
        {
            registry.Policies.Add(new StringifierConfiguration(configure));
            registry.Policies.Add<HtmlConventionCompiler>();
        }

        public static void RegisterPartials(this FubuRegistry registry, Action<IPartialViewTypeRegistry> registration)
        {
            IPartialViewTypeRegistry partialViewTypeRegistry = new PartialViewTypeRegistry();
            registration(partialViewTypeRegistry);

            registry.Services(x => x.AddService(partialViewTypeRegistry));
        }
    }

    public class StringifierConfiguration : IConfigurationAction
    {
        private readonly Action<Stringifier> _configure;

        public StringifierConfiguration(Action<Stringifier> configure)
        {
            _configure = configure;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Services.SetServiceIfNone(new Stringifier());
            Stringifier stringifier = graph.Services.FindAllValues<Stringifier>().First();

            _configure(stringifier);
        }
    }
}