using FubuMVC.Core.Continuations;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication
{
    
    public class AuthResultTester
    {
        [Fact]
        public void is_not_deterministic_if_continuation_is_null_and_success_is_false()
        {
            new AuthResult
            {
                Continuation = null, Success = false
            }.IsDeterministic().ShouldBeFalse();
        }

        [Fact]
        public void is_deterministic_if_success_is_true()
        {
            new AuthResult{Continuation = null, Success = true}.IsDeterministic().ShouldBeTrue();
            new AuthResult{Continuation = FubuContinuation.NextBehavior(), Success = true}.IsDeterministic().ShouldBeTrue();

        }

        [Fact]
        public void is_deterministic_if_success_is_false_but_there_is_a_continuation()
        {
            new AuthResult{Continuation = FubuContinuation.RedirectTo("somewhere")}
                .IsDeterministic().ShouldBeTrue();
        }
    }
}