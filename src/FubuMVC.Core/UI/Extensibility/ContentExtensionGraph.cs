using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuCore;

namespace FubuMVC.Core.UI.Extensibility
{
    // TODO -- no unit tests on this at all.  How sad.
    public class ContentExtensionGraph
    {
        private readonly Cache<Type, object> _shelves = new Cache<Type, object>(t =>
        {
            var type = typeof (ExtensionShelf<>).MakeGenericType(t);
            return Activator.CreateInstance(type);
        });

        private IExtensionShelf _lastShelf;

        private ExtensionShelf<T> shelfFor<T>() where T : class
        {
            return (ExtensionShelf<T>) _shelves[typeof (T)];
        }

        public void Register<T>(string tag, IContentExtension<T> extension) where T : class
        {
            var extensionShelf = shelfFor<T>();
            extensionShelf.Add(tag, extension);
            _lastShelf = extensionShelf;
        }

        public void Register<T>(IContentExtension<T> extension) where T : class
        {
            shelfFor<T>().Add(string.Empty, extension);
        }

        public void FilterLast<T>(Func<IFubuPage<T>, bool> filter) where T : class
        {
            shelfFor<T>().FilterLast(filter);
        }

        public void FilterLast(Func<bool> filter)
        {
            _lastShelf.FilterLast(filter);
        }

        private static void apply<T>(IEnumerable<IContentExtension<T>> extensions, IFubuPage<T> page, string tag) where T : class
        {
            var writer = page.ServiceLocator.GetInstance<IOutputWriter>();
            var extensionOutput = extensions.SelectMany(ex => ex.GetExtensions(page)).Where(o => o != null).ToArray();
            if (extensionOutput.Length == 0 && page.Get<DiagnosticsIndicator>().IsDiagnosticsEnabled)
            {
                writer.WriteHtml("<!-- Content extensions '{1}' for {0} would be rendered here -->".ToFormat(typeof(T).Name, tag));
            }
            extensionOutput.Each(o => writer.WriteHtml(o));
        }

        public void ApplyExtensions<T>(IFubuPage<T> page) where T : class
        {
            apply(shelfFor<T>().AllExtensions(), page, "ALL");
        }

        public void ApplyExtensions<T>(IFubuPage<T> page, string tag) where T : class
        {
            apply(shelfFor<T>().ExtensionsFor(tag), page, tag);
        }
    }
}