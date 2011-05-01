using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewBag
    {
        private readonly IEnumerable<IViewToken> _views;

        private readonly Cache<Type, IEnumerable<IViewToken>> _viewsByType
            = new Cache<Type, IEnumerable<IViewToken>>();

        public ViewBag(IEnumerable<IViewToken> views)
        {
            _views = views;
            _viewsByType.OnMissing = type => _views.Where(x => x.ViewModelType == type);
        }

        public IEnumerable<IViewToken> ViewsFor(Type viewModelType)
        {
            return _viewsByType[viewModelType];
        }

        public IEnumerable<IViewToken> Views { get { return _views; } }
    }
}