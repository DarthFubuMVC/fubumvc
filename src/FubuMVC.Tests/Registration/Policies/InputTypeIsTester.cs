using System;
using System.Linq.Expressions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class InputTypeIsTester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeType<InputTypeEndpoints>();


            });
        }

        private BehaviorChain chainFor(Expression<Action<InputTypeEndpoints>> expression)
        {
            return theGraph.BehaviorFor(expression);
        }

        [Test]
        public void positive_case()
        {
            new InputTypeIs<InputTypeModel>().Matches(chainFor(x => x.post_foo(null)))
                .ShouldBeTrue();
        }

        [Test]
        public void negative_case()
        {
            new InputTypeIs<string>().Matches(chainFor(x => x.post_bar(null)))
                .ShouldBeFalse();
        }

        [Test]
        public void does_not_blow_up_if_input_type_is_null()
        {
            new InputTypeIs<string>().Matches(chainFor(x => x.get_foo()))
                .ShouldBeFalse();
        }
    }

    public class InputTypeEndpoints
    {
        public void post_foo(InputTypeModel number)
        {
            
        }

        public void post_bar(InputTypeModel number)
        {
            
        }

        public string get_foo()
        {
            return null;
        }
    }

    public class InputTypeModel{}
}