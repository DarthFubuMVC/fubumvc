using System;
using System.Collections.Generic;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI.Extensions
{
    public class FilteredContentExtension<T> : IContentExtension<T> where T : class
    {
        private readonly Func<IFubuPage<T>, bool> _filter;
        private readonly IContentExtension<T> _inner;

        public FilteredContentExtension(Func<IFubuPage<T>, bool> filter, IContentExtension<T> inner)
        {
            _filter = filter;
            _inner = inner;
        }

        public IEnumerable<object> GetExtensions(IFubuPage<T> page)
        {
            return _filter(page) ? _inner.GetExtensions(page) : new object[0];
        }

        public Func<IFubuPage<T>, bool> Filter
        {
            get { return _filter; }
        }

        public IContentExtension<T> Inner
        {
            get { return _inner; }
        }
    }
}