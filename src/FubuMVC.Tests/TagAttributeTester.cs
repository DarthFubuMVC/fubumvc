﻿using FubuMVC.Core;
using FubuMVC.Core.Registration;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests
{
    
    public class TagAttributeTester
    {
        [Fact]
        public void applies_the_tags()
        {
            var graph = BehaviorGraph.BuildFrom(x => x.Actions.IncludeType<TaggedEndpoint>());
            graph.ChainFor<TaggedEndpoint>(x => x.get_tags())
                .Tags.ShouldHaveTheSameElementsAs("foo", "bar");
        }
    }

    public class TaggedEndpoint
    {
        [Tag("foo", "bar")]
        public string get_tags()
        {
            return "some tags";
        }
    }
}