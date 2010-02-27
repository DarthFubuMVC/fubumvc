using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Util;
using FubuMVC.Core.View;
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

        public static void RegisterPartials(this FubuRegistry registry, Action<IPartialViewTypeRenderer> registration)
        {
            IPartialViewTypeRenderer renderer = new PartialViewTypeRenderer();
            registration(renderer);

            registry.Services(x => x.AddService(renderer));
        }
    }

    public interface IPartialViewTypeRenderer
    {
        PartialViewTypeExpression For<PARTIALMODEL>();
        Type GetPartialViewTypeFor<PARTIALMODEL>();
        bool HasPartialViewTypeFor<PARTIALMODEL>();
    }

    public class PartialViewTypeRenderer : IPartialViewTypeRenderer
    {
        private readonly IDictionary<Type, PartialViewTypeExpression> _viewModelTypes = new Dictionary<Type, PartialViewTypeExpression>();
        public PartialViewTypeExpression For<PARTIALMODEL>()
        {
            return new PartialViewTypeExpression(this, typeof(PARTIALMODEL));
        }

        public Type GetPartialViewTypeFor<PARTIALMODEL>()
        {
            return _viewModelTypes[typeof (PARTIALMODEL)].RenderType();
        }

        public bool HasPartialViewTypeFor<PARTIALMODEL>()
        {
            return _viewModelTypes.ContainsKey(typeof (PARTIALMODEL));
        }

        public void Register(Type modelType, PartialViewTypeExpression expression)
        {
            _viewModelTypes.Add(modelType, expression);
        }
    }

    public class PartialViewTypeExpression
    {
        private readonly PartialViewTypeRenderer _typeRenderer;
        private readonly Type _modelType;
        private Type _partialView;

        public PartialViewTypeExpression(PartialViewTypeRenderer typeRenderer, Type modelType)
        {
            _typeRenderer = typeRenderer;
            _modelType = modelType;
        }

        public PartialViewTypeExpression Use<TPartialView>()
            where TPartialView : IFubuPage
        {
            _partialView = typeof(TPartialView);
            _typeRenderer.Register(_modelType, this);
            return this;
        }

        public Type RenderType()
        {
            return _partialView;
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