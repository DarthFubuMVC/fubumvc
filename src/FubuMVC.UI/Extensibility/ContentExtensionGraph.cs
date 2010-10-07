using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace FubuMVC.UI.Extensibility
{
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

        private static void apply<T>(IEnumerable<IContentExtension<T>> extensions, IFubuPage<T> page) where T : class
        {
            var writer = page.ServiceLocator.GetInstance<IOutputWriter>();
            GenericEnumerableExtensions.Each<object>(extensions
                                  .SelectMany(ex => ex.GetExtensions(page))
                                  .Where(o => o != null), o => OutputWriterExtensions.WriteHtml(writer, (object) o));
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
}