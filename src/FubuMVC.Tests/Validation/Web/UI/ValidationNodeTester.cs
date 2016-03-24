using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.UI;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    [TestFixture]
    public class ValidationNodeTester
    {
        private ValidationNode theValidationNode;

        [SetUp]
        public void SetUp()
        {
            theValidationNode = new ValidationNode();
        }

        [Test]
        public void no_duplicates()
        {
            var s1 = MockRepository.GenerateStub<IRenderingStrategy>();
            theValidationNode.RegisterStrategy(s1);

            theValidationNode.ShouldHaveTheSameElementsAs(s1);
        }

        [Test]
        public void clears_the_strategies()
        {
            var s1 = MockRepository.GenerateStub<IRenderingStrategy>();
            var s2 = MockRepository.GenerateStub<IRenderingStrategy>();
			
            theValidationNode.RegisterStrategy(s1);
            theValidationNode.RegisterStrategy(s2);

            theValidationNode.Clear();

            theValidationNode.ShouldHaveCount(0);
        }

        [Test]
        public void defaults()
        {
            var strategies = ValidationNode.Default();
            strategies.ShouldHaveTheSameElementsAs(RenderingStrategies.Summary, RenderingStrategies.Highlight);
        }
    }
}