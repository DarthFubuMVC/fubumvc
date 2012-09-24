using Fubu.Templating;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Templating
{
    [TestFixture]
    public class TemplatePlanTester
    {
        private TemplatePlan _plan;

        [SetUp]
        public void before_each()
        {
            _plan = new TemplatePlan();
        }

        [Test]
        public void should_add_steps()
        {
            var step = new TestStep();
            _plan.AddStep(step);

            _plan.Steps.ShouldContain(step);
        }

        public class TestStep : ITemplateStep
        {
            public string Describe(TemplatePlanContext context)
            {
                return "";
            }

            public void Execute(TemplatePlanContext context)
            {
            }
        }
    }
}