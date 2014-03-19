using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using FubuCore.Util;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Core.View.Attachment
{
    /// <summary>
    /// Represent a collection of <see cref="IViewToken"/>s.
    /// </summary>
    public class ViewBag
    {
        private readonly IEnumerable<IViewToken> _views;

        private readonly Cache<Type, IEnumerable<IViewToken>> _viewsByType
            = new Cache<Type, IEnumerable<IViewToken>>();

        public static ViewBag Empty()
        {
            return new ViewBag(new IViewToken[0]);
        }

        public ViewBag(IEnumerable<IViewToken> views)
        {
            _views = views;
            _viewsByType.OnMissing = type => _views.Where(x => x.ViewModel == type);
        }

        public IEnumerable<IViewToken> ViewsFor(Type viewModelType)
        {
            return _viewsByType[viewModelType];
        }

        public IEnumerable<IViewToken> Views { get { return _views; } }

        public IEnumerable<T> Templates<T>() where T : ITemplateFile
        {
            return _views.OfType<T>();
        }
    }
}