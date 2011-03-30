using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration.Conventions
{
    public class ReorderBehaviorsPolicy : IConfigurationAction
    {
        public Func<BehaviorNode, bool> WhatMustBeBefore { get; set; }
        public Func<BehaviorNode, bool> WhatMustBeAfter { get; set; }

        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.SelectMany(x => x).Where(WhatMustBeAfter).ToList().Each(shouldBeAfter =>
            {
                shouldBeAfter.ForFollowingBehavior(new BehaviorSearch(WhatMustBeBefore){
                    OnFound = shouldBeAfter.AddBefore
                });
            });
        }

        public void ThisNodeMustBeBefore<T>() where T : BehaviorNode
        {
            WhatMustBeBefore = node => node is T;
        }

        public void ThisNodeMustBeAfter<T>() where T : BehaviorNode
        {
            WhatMustBeAfter = node => node is T;
        }

        public static Func<BehaviorNode, bool> FuncForWrapper(Type wrapperType)
        {
            return node => node is Wrapper && node.As<Wrapper>().BehaviorType == wrapperType;
        }

        public void ThisWrapperBeBefore<T>() where T : IActionBehavior
        {
            WhatMustBeBefore = FuncForWrapper(typeof (T));
        }

        public void ThisWrapperMustBeAfter<T>() where T : IActionBehavior
        {
            WhatMustBeAfter = FuncForWrapper(typeof(T));
        }
    }
}