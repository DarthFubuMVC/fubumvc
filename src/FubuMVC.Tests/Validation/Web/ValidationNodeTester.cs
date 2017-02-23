using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Tests.Validation.Web.UI;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class ValidationNodeTester
    {
        [Fact]
        public void default_rendering_strategies()
        {
            new AjaxValidationNode(ActionCall.For<FormValidationModeEndpoint>(x => x.post_ajax(null)))
                .Validation
                .ShouldHaveTheSameElementsAs(ValidationNode.Default());

            new ValidationActionFilter(null, null)
                .Validation
                .ShouldHaveTheSameElementsAs(ValidationNode.Default());
        }

        [Fact]
        public void is_empty()
        {
            ValidationNode.Empty().IsEmpty().ShouldBeTrue();
        }

        [Fact]
        public void is_empty_negative()
        {
            ValidationNode.Default().IsEmpty().ShouldBeFalse();
        }

        [Fact]
        public void default_mode_is_live()
        {
            new ValidationNode().Mode.ShouldBe(ValidationMode.Live);
        }

        [Fact]
        public void override_the_default_mode()
        {
            var node = new ValidationNode();
            node.DefaultMode(ValidationMode.Triggered);
            node.Mode.ShouldBe(ValidationMode.Triggered);
        }

        [Fact]
        public void determine_the_mode_uses_the_matching_policy()
        {
            var accessor = SingleProperty.Build<ValidationModeTarget>(x => x.Property);
            var services = new InMemoryServiceLocator();
            services.Add(new AccessorRules());
            var mode = ValidationMode.Triggered;

            var p1 = MockRepository.GenerateStub<IValidationModePolicy>();
            p1.Stub(x => x.Matches(services, accessor)).Return(true);
            p1.Stub(x => x.DetermineMode(services, accessor)).Return(mode);

            var node = new ValidationNode();
            node.DefaultMode(ValidationMode.Live);
            node.RegisterPolicy(p1);

            node.As<IValidationNode>().DetermineMode(services, accessor).ShouldBe(mode);
        }

        [Fact]
        public void determine_the_mode_uses_the_last_matching_policy()
        {
            var accessor = SingleProperty.Build<ValidationModeTarget>(x => x.Property);
            var services = new InMemoryServiceLocator();
            services.Add(new AccessorRules());
            var mode = ValidationMode.Triggered;

            var p1 = MockRepository.GenerateStub<IValidationModePolicy>();
            p1.Stub(x => x.Matches(services, accessor)).Return(true);
            p1.Stub(x => x.DetermineMode(services, accessor)).Return(ValidationMode.Live);

            var p2 = MockRepository.GenerateStub<IValidationModePolicy>();
            p2.Stub(x => x.Matches(services, accessor)).Return(true);
            p2.Stub(x => x.DetermineMode(services, accessor)).Return(mode);

            var node = new ValidationNode();
            node.DefaultMode(ValidationMode.Live);
            node.RegisterPolicy(p1);
            node.RegisterPolicy(p2);

            node.As<IValidationNode>().DetermineMode(services, accessor).ShouldBe(mode);
        }

        [Fact]
        public void determine_the_mode_uses_the_default_when_no_policies_match()
        {
            var accessor = SingleProperty.Build<ValidationModeTarget>(x => x.Property);
            var services = new InMemoryServiceLocator();
            services.Add(new AccessorRules());

            var p1 = MockRepository.GenerateStub<IValidationModePolicy>();
            p1.Stub(x => x.Matches(services, accessor)).Return(false);
            p1.Stub(x => x.DetermineMode(services, accessor)).Return(ValidationMode.Triggered);

            var node = new ValidationNode();
            node.DefaultMode(ValidationMode.Live);
            node.RegisterPolicy(p1);

            node.As<IValidationNode>().DetermineMode(services, accessor).ShouldBe(ValidationMode.Live);
        }

        [Fact]
        public void default_element_timeout_is_500ms()
        {
            new ValidationNode().ElementTimeout.ShouldBe(ValidationNode.DefaultTimeout);
        }

        public class ValidationModeTarget
        {
            public string Property { get; set; }
        }
    }
}