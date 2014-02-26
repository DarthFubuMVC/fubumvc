using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class actions_that_return_IDictionary_are_json
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<MyController>();
            });
        }

        [Test]
        public void methods_that_return_an_IDictionary_string_object_should_be_asymmetric_json()
        {
            theGraph.BehaviorFor<MyController>(x => x.ReturnsJson(null)).ShouldBeAsymmetricJson();
            var behaviorChain = theGraph.BehaviorFor<MyController>(x => x.ReturnOtherJson());

            behaviorChain.ResourceType().ShouldEqual(typeof (IDictionary<string, object>));
            behaviorChain.ShouldBeAsymmetricJson();
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

    public static class ConnegSpecifications
    {
        public static void ShouldBeAsymmetricJson(this BehaviorChain chain)
        {
            if (chain.ResourceType() != null)
            {
                chain.Output.MimeTypes()
                    .OrderBy(x => x)
                    .ShouldHaveTheSameElementsAs("application/json", "text/json");
            }

            if (chain.InputType() != null)
            {
                var mimetypes = chain.Input.Mimetypes.ToArray();
                mimetypes
                    .OrderBy(x => x)
                    .ShouldHaveTheSameElementsAs("application/json", MimeType.HttpFormMimetype.ToString(), MimeType.MultipartMimetype.ToString(), "text/json");
            }
        }
    }
}