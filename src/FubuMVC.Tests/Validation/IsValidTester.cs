using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class IsValidTester
    {
        private ValidationContext theContext;
        private Accessor theAccessor;

        public IsValidTester()
        {
            theAccessor = ReflectionHelper.GetAccessor<IsValidTarget>(x => x.Name);
            theContext = ValidationContext.For(new object());
        }

        [Fact]
        public void invalid()
        {
            theContext.Notification.RegisterMessage(theAccessor, StringToken.FromKeyString("Test", "Test"));
            new IsValid().Matches(theAccessor, theContext).ShouldBeFalse();
        }

        [Fact]
        public void valid()
        {
            var otherAccessor = ReflectionHelper.GetAccessor<IsValidTarget>(x => x.Other);
            theContext.Notification.RegisterMessage(otherAccessor, StringToken.FromKeyString("Test", "Test"));
            new IsValid().Matches(theAccessor, theContext).ShouldBeTrue();
        }

        public class IsValidTarget
        {
            public string Name { get; set; }
            public string Other { get; set; }
        }
    }
}