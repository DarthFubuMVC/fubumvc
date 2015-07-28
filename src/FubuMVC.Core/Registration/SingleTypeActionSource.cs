using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public class SingleTypeActionSource : IActionSource, DescribesItself
    {
        private readonly Type _actionType;
        private readonly ActionMethodFilter _methodFilter;

        public SingleTypeActionSource(Type actionType, ActionMethodFilter methodFilter)
        {
            _actionType = actionType;
            _methodFilter = methodFilter;
        }

        public Task<ActionCall[]> FindActions(Assembly applicationAssembly)
        {
            return Task.Factory.StartNew(() =>
            {
                return _actionType.PublicInstanceMethods().Where(_methodFilter.Matches)
                    .Select(method => new ActionCall(_actionType, method)).ToArray();
            });
        }

        public void Describe(Description description)
        {
            description.Title = "Type: " + _actionType.FullName;
        }
    }
}