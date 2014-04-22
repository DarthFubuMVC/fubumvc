using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Registration.Conventions
{
    [Title("Behavior Ordering Rule")]
    [CanBeMultiples]
    public class ReorderBehaviorsPolicy : IConfigurationAction
    {
        public Func<BehaviorNode, bool> WhatMustBeBefore { get; set; }
        public Func<BehaviorNode, bool> WhatMustBeAfter { get; set; }

        private string _beforeDescription = "Before";
        private string _afterDescription = "After";

        public override string ToString()
        {
            return "Reordering, what must be before: {0}, and what must be after {1}".ToFormat(_beforeDescription,
                _afterDescription);
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.SelectMany(x => x).Where(WhatMustBeAfter).ToList().Each(shouldBeAfter =>
            {
                shouldBeAfter.ForFollowingBehavior(new BehaviorSearch(WhatMustBeBefore){
                    OnFound = shouldBeAfter.AddBefore
                });
            });
        }

        public BehaviorCategory CategoryMustBeBefore
        {
            set
            {
                WhatMustBeBefore = node => node.Category == value;
                _beforeDescription = "Category " + value;
            }
        }

        public BehaviorCategory CategoryMustBeAfter
        {
            set
            {
                WhatMustBeAfter = node => node.Category == value;
                _afterDescription = "Category " + value;
            }
        }

        public ReorderBehaviorsPolicy ThisNodeMustBeBefore<T>() where T : BehaviorNode
        {
            WhatMustBeBefore = node => node is T;
            _beforeDescription = "Node " + typeof (T).FullName;

            return this;
        }

        public ReorderBehaviorsPolicy ThisNodeMustBeAfter<T>() where T : BehaviorNode
        {
            WhatMustBeAfter = node => node is T;
            _afterDescription = "Node " + typeof(T).FullName;

            return this;
        }

        public static Func<BehaviorNode, bool> FuncForWrapper(Type wrapperType)
        {
            return node => node is Wrapper && node.As<Wrapper>().BehaviorType == wrapperType;
        }

        public void ThisWrapperBeBefore<T>() where T : IActionBehavior
        {
            WhatMustBeBefore = FuncForWrapper(typeof (T));
            _beforeDescription = "Behavior " + typeof (T).FullName;
        }

        public void ThisWrapperMustBeAfter<T>() where T : IActionBehavior
        {
            WhatMustBeAfter = FuncForWrapper(typeof(T));
            _afterDescription = "Behavior " + typeof(T).FullName;
        }
    }
}