using System;
using System.Linq;

namespace FubuMVC.Core.View.Attachment
{
    public class ViewProfile : IViewProfile
    {
        private readonly Type _conditionType;
        private readonly Func<IViewToken, bool> _filter;
        private readonly Func<IViewToken, string> _getName;

        public ViewProfile(Type conditionType, Func<IViewToken, bool> filter, Func<IViewToken, string> getName)
        {
            _conditionType = conditionType;
            _filter = filter;
            _getName = getName;
        }

        public Type ConditionType
        {
            get { return _conditionType; }
        }

        public ViewBag Filter(ViewBag bag)
        {
            var views = bag.Views.Where(_filter).Select(x => new ProfileViewToken(x, _getName(x)));
            return new ViewBag(views);
        }
        
    }
}