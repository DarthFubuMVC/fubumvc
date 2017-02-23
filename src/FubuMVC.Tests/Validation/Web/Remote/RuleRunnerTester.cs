using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using FubuMVC.Core.Validation.Web.Remote;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.Remote
{
    
    public class RuleRunnerTester : InteractionContext<RuleRunner>
    {
        private RecordingFieldValidationRule theRule;
        private ValidationContext theContext;
        private RunnerTarget theTarget;
        private Accessor theAccessor;
        private RemoteFieldRule theRemoteRule;
        private string theValue;

        protected override void beforeEach()
        {
            theValue = "Testing";
            theAccessor = ReflectionHelper.GetAccessor<RunnerTarget>(x => x.Name);
            theTarget = new RunnerTarget();
            theContext = ValidationContext.For(theTarget);
            theRule = new RecordingFieldValidationRule();

            theRemoteRule = RemoteFieldRule.For(theAccessor, theRule);

            MockFor<IValidationTargetResolver>().Stub(x => x.Resolve(theAccessor, theValue)).Return(theTarget);

            MockFor<IValidator>().Stub(x => x.ContextFor(Arg<object>.Is.Same(theTarget), Arg<Notification>.Is.NotNull)).Return(theContext);

            ClassUnderTest.Run(theRemoteRule, theValue);
        }

        [Fact]
        public void invokes_the_rule()
        {
            theRule.Accessor.ShouldBe(theAccessor);
            theRule.Context.ShouldBeTheSameAs(theContext);
        }

        public class RunnerTarget
        {
            public string Name { get; set; }
        }

        public class RecordingFieldValidationRule : IFieldValidationRule
        {
            public Accessor Accessor;
            public ValidationContext Context;

	        public StringToken Token { get; set; }

			public ValidationMode Mode { get; set; }

	        public void Validate(Accessor accessor, ValidationContext context)
            {
                Accessor = accessor;
                Context = context;
            }
        }
    }
}