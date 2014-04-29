using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Extensions
{
    [ApplicationLevel]
    public class ContentExtensionGraph
    {
        private readonly Cache<Type, object> _shelves = new Cache<Type, object>(t =>
        {
            var type = typeof (ExtensionShelf<>).MakeGenericType(t);
            return Activator.CreateInstance(type);
        });

        private IExtensionShelf _lastShelf;

        // Public for testing
        public ExtensionShelf<T> ShelfFor<T>() where T : class
        {
            return (ExtensionShelf<T>) _shelves[typeof (T)];
        }

        public void Register<T>(string tag, IContentExtension<T> extension) where T : class
        {
            var extensionShelf = ShelfFor<T>();
            extensionShelf.Add(tag, extension);
            _lastShelf = extensionShelf;
        }

        public void Register<T>(IContentExtension<T> extension) where T : class
        {
            ShelfFor<T>().Add(string.Empty, extension);
        }

        public void FilterLast<T>(Func<IFubuPage<T>, bool> filter) where T : class
        {
            ShelfFor<T>().FilterLast(filter);
        }

        public void FilterLast(Func<bool> filter)
        {
            _lastShelf.FilterLast(filter);
        }

        private static IHtmlString apply<T>(IEnumerable<IContentExtension<T>> extensions, IFubuPage<T> page, string tag)
            where T : class
        {
            var extensionOutput = extensions.SelectMany(ex => ex.GetExtensions(page)).Where(o => o != null).ToArray();
            if (extensionOutput.Length == 0 && FubuMode.InDevelopment())
            {
                page.Write("<!-- Content extensions '{1}' for {0} would be rendered here -->".ToFormat(typeof (T).Name,
                                                                                                       tag));
            }

            var output = new StringBuilder();
            extensionOutput.Each(o => output.Append(o).AppendLine());

            return new HtmlString(output.ToString());
        }

        public IHtmlString ApplyExtensions<T>(IFubuPage<T> page) where T : class
        {
            return apply(ShelfFor<T>().AllExtensions(), page, "ALL");
        }

        public IHtmlString ApplyExtensions<T>(IFubuPage<T> page, string tag) where T : class
        {
            return apply(ShelfFor<T>().ExtensionsFor(tag), page, tag);
        }
    }
}