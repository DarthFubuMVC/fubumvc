using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using Xunit;
using FubuMVC.Core.Resources.Conneg;
using Shouldly;

namespace FubuMVC.Tests.Registration.Conventions
{
    
    public class actions_that_return_IDictionary_are_json
    {
        private BehaviorGraph theGraph;

        public actions_that_return_IDictionary_are_json()
        {
            theGraph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<MyController>();
            });
        }

        [Fact]
        public void methods_that_return_an_IDictionary_string_object_should_be_asymmetric_json()
        {
            var chain = theGraph.ChainFor<MyController>(x => x.ReturnsJson(null));
            chain.Output.Writes(MimeType.Json).ShouldBeTrue();

            var behaviorChain = theGraph.ChainFor<MyController>(x => x.ReturnOtherJson());
            behaviorChain.Output.Writes(MimeType.Json).ShouldBeTrue();
        }


        public class Input1{}
        public class MyController
        {
            public IDictionary<string, object> ReturnsJson(Input1 input)
            {
                return  new Dictionary<string, object>();
            }

            public IDictionary<string, object> ReturnOtherJson()
            {
                return new Dictionary<string, object>();
            }

            public Input1 NotJson()
            {
                return null;
            }
        }
    }

}