using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewsForFilterLog : DescribesItself
    {
        private readonly IViewsForActionFilter _filter;
        private readonly IEnumerable<IViewToken> _views;

        public ViewsForFilterLog(IViewsForActionFilter filter, IEnumerable<IViewToken> views)
        {
            _filter = filter;
            _views = views.ToList();
        }

        public IViewsForActionFilter Filter
        {
            get { return _filter; }
        }

        public IEnumerable<IViewToken> Views
        {
            get { return _views; }
        }

        void DescribesItself.Describe(Description description)
        {
            var filterDesc = Description.For(_filter);
            description.Title = filterDesc.Title;
            description.AddList("Views", _views);
        }
    }
}