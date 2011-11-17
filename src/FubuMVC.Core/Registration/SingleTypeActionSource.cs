using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public class SingleTypeActionSource : IActionSource
    {
        private readonly Type _actionType;
        private readonly ActionMethodFilter _methodFilter;

        public SingleTypeActionSource(Type actionType, ActionMethodFilter methodFilter)
        {
            _actionType = actionType;
            _methodFilter = methodFilter;
        }

        public IEnumerable<ActionCall> FindActions(TypePool types)
        {
            return _actionType.PublicInstanceMethods().Where(_methodFilter.Matches)
                .Select(method => new ActionCall(_actionType, method));
        }
    }
}