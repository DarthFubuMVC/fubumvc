using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TagAttribute : ModifyChainAttribute
    {
        private readonly string[] _tags;

        public TagAttribute(params string[] tags)
        {
            _tags = tags;
        }

        public override void Alter(ActionCall call)
        {
            var chain = call.ParentChain();
            chain.Tags.Fill(_tags);
        }
    }
}