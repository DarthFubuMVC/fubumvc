using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.UI;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class ValidationNodeTester
    {
        private ValidationNode theValidationNode = new ValidationNode();

        [Fact]
        public void no_duplicates()
        {
            var s1 = MockRepository.GenerateStub<IRenderingStrategy>();
            theValidationNode.RegisterStrategy(s1);

            theValidationNode.ShouldHaveTheSameElementsAs(s1);
        }

        [Fact]
        public void clears_the_strategies()
        {
            var s1 = MockRepository.GenerateStub<IRenderingStrategy>();
            var s2 = MockRepository.GenerateStub<IRenderingStrategy>();
			
            theValidationNode.RegisterStrategy(s1);
            theValidationNode.RegisterStrategy(s2);

            theValidationNode.Clear();

            theValidationNode.ShouldHaveCount(0);
        }

        [Fact]
        public void defaults()
        {
            var strategies = ValidationNode.Default();
            strategies.ShouldHaveTheSameElementsAs(RenderingStrategies.Summary, RenderingStrategies.Highlight);
        }
    }
}