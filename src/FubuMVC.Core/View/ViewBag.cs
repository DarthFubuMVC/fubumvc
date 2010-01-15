using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.View
{
    public class ViewBag
    {
        private readonly IEnumerable<IDiscoveredViewToken> _views;

        private readonly Cache<Type, IEnumerable<IDiscoveredViewToken>> _viewsByType
            = new Cache<Type, IEnumerable<IDiscoveredViewToken>>();

        public ViewBag(IEnumerable<IDiscoveredViewToken> views)
        {
            _views = views;
            _viewsByType.OnMissing = type => _views.Where(x => x.ViewModelType == type);
        }

        public IEnumerable<IDiscoveredViewToken> ViewsFor(Type viewModelType)
        {
            return _viewsByType[viewModelType];
        }

        public IEnumerable<IDiscoveredViewToken> Views { get { return _views; } }
    }
}