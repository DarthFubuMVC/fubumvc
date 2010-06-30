using System;
using System.Collections;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using System.Linq;

namespace FubuMVC.UI.Extensibility
{
    public interface IContentExtension<T> where T : class
    {
        IEnumerable<object> GetExtensions(IFubuPage<T> page);
    }

   

    public class LambdaExtension<T> : IContentExtension<T> where T : class
    {
        private readonly Func<IFubuPage<T>, object> _func;

        public LambdaExtension(Func<IFubuPage<T>, object> func)
        {
            _func = func;
        }

        public IEnumerable<object> GetExtensions(IFubuPage<T> page)
        {
            yield return _func(page);
        }
    }

    public class ContentExtensionGraph
    {
        private readonly Cache<Type, object> _shelves = new Cache<Type, object>(t =>
        {
            var type = typeof (ExtensionShelf<>).MakeGenericType(t);
            return Activator.CreateInstance(type);
        });

        private ExtensionShelf<T> shelfFor<T>() where T : class
        {
            return (ExtensionShelf<T>) _shelves[typeof (T)];
        }

        public void Register<T>(string tag, IContentExtension<T> extension) where T : class
        {
            shelfFor<T>().Add(tag, extension);            
        }

        public void Register<T>(IContentExtension<T> extension) where T : class
        {
            shelfFor<T>().Add(string.Empty, extension);
        }

        // Deep in the bowells of FubuMVC, this is the actual code that writes extensions
        // into a FubuPage
        private static void apply<T>(IEnumerable<IContentExtension<T>> extensions, IFubuPage<T> page) where T : class
        {
            var writer = page.ServiceLocator.GetInstance<IOutputWriter>();
            extensions
                .SelectMany(ex => ex.GetExtensions(page))
                .Where(o => o != null)
                .Each(o => writer.WriteHtml(o));
        }

        public void ApplyExtensions<T>(IFubuPage<T> page) where T : class
        {
            apply(shelfFor<T>().AllExtensions(), page);
        }

        public void ApplyExtensions<T>(IFubuPage<T> page, string tag) where T : class
        {
            apply(shelfFor<T>().ExtensionsFor(tag), page);
        }

        public class ExtensionShelf<T> where T : class
        {
            private readonly Cache<string, List<IContentExtension<T>>> _cache = new Cache<string, List<IContentExtension<T>>>(key => new List<IContentExtension<T>>());

            public void Add(string tag, IContentExtension<T> extension)
            {
                _cache[tag].Add(extension);
            }

            public IEnumerable<IContentExtension<T>> ExtensionsFor(string tag)
            {
                return _cache[tag];
            }

            public IEnumerable<IContentExtension<T>> AllExtensions()
            {
                return _cache.GetAll().SelectMany(x => x);
            }
        }
    }

    public static class ContentExtensions
    {
        public static void WriteExtensions<T>(this IFubuPage<T> page) where T : class
        {
            page.Get<ContentExtensionGraph>().ApplyExtensions(page);
        }

        public static void WriteExtensions<T>(this IFubuPage<T> page, string tag) where T : class
        {
            page.Get<ContentExtensionGraph>().ApplyExtensions(page, tag);
        }

        public static ExtensionsExpression Extensions(this FubuRegistry registry)
        {
            return new ExtensionsExpression(registry);
        }
    }

    public class ExtensionsExpression
    {
        private readonly FubuRegistry _registry;

        public ExtensionsExpression(FubuRegistry registry)
        {
            _registry = registry;
        }

        private ExtensionsExpression register(Action<ContentExtensionGraph> configure)
        {
            _registry.Services(x =>
            {
                x.SetServiceIfNone<ContentExtensionGraph>(new ContentExtensionGraph());
                var graph = x.DefaultServiceFor<ContentExtensionGraph>().Value as ContentExtensionGraph;
                configure(graph);
            });

            return this;
        }

        public ExtensionsExpression For<T>(string tag, IContentExtension<T> extension) where T : class
        {
            return register(g => g.Register(tag, extension));
        }

        public ExtensionsExpression For<T>(IContentExtension<T> extension) where T : class
        {
            return register(g => g.Register(extension));
        }

        public ExtensionsExpression For<T>(Func<IFubuPage<T>, object> func) where T : class
        {
            return For(new LambdaExtension<T>(func));
        }

        public ExtensionsExpression For<T>(string tag, Func<IFubuPage<T>, object> func) where T : class
        {
            return For(tag, new LambdaExtension<T>(func));
        }
    }
}