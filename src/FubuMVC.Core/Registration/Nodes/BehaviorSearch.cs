using System;

namespace FubuMVC.Core.Registration.Nodes
{

    public class BehaviorSearch
    {
        public BehaviorSearch(Func<BehaviorNode, bool> matching)
        {
            Matching = matching;

            OnFound = n => { };
            OnMissing = () => { };
        }

        public Func<BehaviorNode, bool> Matching { get; set; }
        public Action<BehaviorNode> OnFound { get; set; }
        public Action OnMissing { get; set; }
    }
}