using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static void StringConversions(this FubuRegistry registry, Action<IStringifierConfiguration> configure)
        {
            registry.Policies.Add(new StringifierConfiguration(configure));
            registry.Policies.Add<HtmlConventionCompiler>();
        }

        public static void StringConversions<T>(this FubuRegistry registry) where T : StringConversionRegistry, new()
        {
            registry.Policies.Add(new T());
        }
    }

    public class StringifierConfiguration : IConfigurationAction
    {
        private readonly Action<IStringifierConfiguration> _configure;

        public StringifierConfiguration(Action<IStringifierConfiguration> configure)
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


    public class StringConversionRegistry : IConfigurationAction
    {
        private readonly IList<StringifierStrategy> _strategies = new List<StringifierStrategy>();

        void IConfigurationAction.Configure(BehaviorGraph graph)
        {
            graph.Services.SetServiceIfNone(new Stringifier());
            Stringifier stringifier = graph.Services.FindAllValues<Stringifier>().First();

            _strategies.Each(s => stringifier.AddStrategy(s));
        }

        private MakeDisplayExpression makeDisplay(Func<GetStringRequest, bool> filter)
        {
            return new MakeDisplayExpression(func =>
            {
                _strategies.Add(new StringifierStrategy()
                {
                    Matches = filter,
                    StringFunction = func
                });
            });
        }

        private MakeDisplayExpression<T> makeDisplay<T>(Func<GetStringRequest, bool> filter)
        {
            return new MakeDisplayExpression<T>(func =>
            {
                _strategies.Add(new StringifierStrategy()
                {
                    Matches = filter,
                    StringFunction = func
                });
            });
        }

        public MakeDisplayExpression IfTypeMatches(Func<Type, bool> filter)
        {
            return makeDisplay(request => filter(request.PropertyType));
        }

        public MakeDisplayExpression<T> IfIsType<T>()
        {
            return makeDisplay<T>(request => request.PropertyType == typeof (T));
        }

        public MakeDisplayExpression<T> IfCanBeCastToType<T>()
        {
            return makeDisplay<T>(t => t.PropertyType.CanBeCastTo<T>());
        }

        public MakeDisplayExpression IfPropertyMatches(Func<PropertyInfo, bool> matches)
        {
            return makeDisplay(request => matches(request.Property));
        }

        public MakeDisplayExpression<T> IfPropertyMatches<T>(Func<PropertyInfo, bool> matches)
        {
            return makeDisplay<T>(request => request.PropertyType == typeof(T) && matches(request.Property));
        }

        public abstract class MakeDisplayExpressionBase
        {
            protected Action<Func<GetStringRequest, string>> _callback;

            public MakeDisplayExpressionBase(Action<Func<GetStringRequest, string>> callback)
            {
                _callback = callback;
            }

            protected void apply(Func<GetStringRequest, string> func)
            {
                _callback(func);
            }
        }

        public class MakeDisplayExpression : MakeDisplayExpressionBase
        {
            public MakeDisplayExpression(Action<Func<GetStringRequest, string>> callback)
                : base(callback)
            {
            }

            public void ConvertBy(Func<GetStringRequest, string> display)
            {
                _callback(display);
            }

            public void ConvertWith<TService>(Func<TService, GetStringRequest, string> display)
            {
                apply(o => display(o.Get<TService>(), o));
            }
        }

        public class MakeDisplayExpression<T> : MakeDisplayExpressionBase
        {
            public MakeDisplayExpression(Action<Func<GetStringRequest, string>> callback)
                : base(callback)
            {
            }

            public void ConvertBy(Func<T, string> display)
            {
                apply(o => display((T) o.RawValue));
            }

            public void ConvertBy(Func<GetStringRequest, T, string> display)
            {
                apply(o => display(o, (T)o.RawValue));
            }

            public void ConvertWith<TService>(Func<TService, T, string> display)
            {
                apply(o => display(o.Get<TService>(), (T)o.RawValue));
            }
        }
    }
}