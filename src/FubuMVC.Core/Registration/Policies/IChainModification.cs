using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Policies
{
    public interface IChainModification
    {
        void Modify(BehaviorChain chain);
    }

    public class LambdaChainModification : IChainModification, DescribesItself
    {
        private readonly Action<BehaviorChain> _modification;

        public LambdaChainModification(Action<BehaviorChain> modification)
        {
            _modification = modification;

            Title = "Lambda Modification";
        }

        public string Title { get; set; }
        public string Description { get; set; }

        public void Modify(BehaviorChain chain)
        {
            _modification(chain);
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title = Title;
            if (Description != null) description.ShortDescription = Description;
        }
    }
}