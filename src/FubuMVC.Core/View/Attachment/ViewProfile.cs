using System;
using System.Linq;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewProfile : IViewProfile
    {
        private readonly IConditional _condition;
        private readonly Func<IViewToken, bool> _filter;
        private readonly Func<IViewToken, string> _getName;

        public ViewProfile(IConditional condition, Func<IViewToken, bool> filter, Func<IViewToken, string> getName)
        {
            _condition = condition;
            _filter = filter;
            _getName = getName;
        }

        public IConditional Condition
        {
            get { return _condition; }
        }

        public ViewBag Filter(ViewBag bag)
        {
            var views = bag.Views.Where(_filter).Select(x => new ProfileViewToken(x, _getName(x)));
            return new ViewBag(views);
        }
        
    }
}