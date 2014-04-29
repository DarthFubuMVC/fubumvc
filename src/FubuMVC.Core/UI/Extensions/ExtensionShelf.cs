using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Extensions
{
    public interface IExtensionShelf
    {
        void FilterLast(Func<bool> filter);
    }

    public class ExtensionShelf<T> : IExtensionShelf where T : class
    {
        private readonly Cache<string, List<IContentExtension<T>>> _cache = new Cache<string, List<IContentExtension<T>>>(key => new List<IContentExtension<T>>());
        private List<IContentExtension<T>> _lastList;
        private IContentExtension<T> _lastExtension;

        public void Add(string tag, IContentExtension<T> extension)
        {
            _lastList = _cache[tag];
            _lastExtension = extension;

            _lastList.Add(extension);
        }

        public IEnumerable<IContentExtension<T>> ExtensionsFor(string tag)
        {
            return _cache[tag];
        }

        public IEnumerable<IContentExtension<T>> AllExtensions()
        {
            return _cache.GetAll().SelectMany(x => x);
        }

        public void FilterLast(Func<IFubuPage<T>,bool> filter)
        {
            var filteredExtension = new FilteredContentExtension<T>(filter, _lastExtension);
            _lastList.Remove(_lastExtension);
            _lastList.Add(filteredExtension);
        }

        public void FilterLast(Func<bool> filter)
        {
            FilterLast(page => filter());
        }
    }
}