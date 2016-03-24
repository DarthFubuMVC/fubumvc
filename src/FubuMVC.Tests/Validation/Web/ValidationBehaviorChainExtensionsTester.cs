using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Validation.Web;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Validation.Web
{
    [TestFixture]
    public class ValidationBehaviorChainExtensionsTester
    {
        [Test]
        public void finds_the_validation_node()
        {
            var stub = new StubValidationBehaviorNode
            {
                Validation = ValidationNode.Default()
            };

            var chain = new BehaviorChain();
            chain.AddToEnd(stub);

            chain.ValidationNode().ShouldBe(stub.Validation);
        }

        [Test]
        public void returns_empty_if_no_validation_node_exists()
        {
            var chain = new BehaviorChain();
            chain.ValidationNode().ShouldBe(ValidationNode.Empty());
        }

        public class StubValidationBehaviorNode : BehaviorNode, IHaveValidation
        {
            protected override IConfiguredInstance buildInstance()
            {
                throw new System.NotImplementedException();
            }

            public override BehaviorCategory Category
            {
                get { throw new System.NotImplementedException(); }
            }

            public ValidationNode Validation { get; set; }
        }
    }
}