using FubuMVC.Core;
using FubuMVC.Core.Validation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class ValidationCompilerTester
    {
        [Fact]
        public void picks_up_all_the_validation_registrations()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                var graph = runtime.Get<ValidationGraph>();

                FakeValidationRegistration.Configured.ShouldBeSameAs(graph);
                FakeValidationRegistration2.Configured.ShouldBeSameAs(graph);
                FakeValidationRegistration3.Configured.ShouldBeSameAs(graph);
            }
        }
    }

    public class FakeValidationRegistration : IValidationRegistration
    {
        public static ValidationGraph Configured;

        public void Register(ValidationGraph graph)
        {
            Configured = graph;
        }
    }

    public class FakeValidationRegistration2 : IValidationRegistration
    {
        public static ValidationGraph Configured;

        public void Register(ValidationGraph graph)
        {
            Configured = graph;
        }
    }

    public class FakeValidationRegistration3 : IValidationRegistration
    {
        public static ValidationGraph Configured;

        public void Register(ValidationGraph graph)
        {
            Configured = graph;
        }
    }
}