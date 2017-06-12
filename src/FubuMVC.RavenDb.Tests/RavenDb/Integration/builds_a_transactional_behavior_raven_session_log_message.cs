using System.Security.Principal;
using System.Threading;
using FubuMVC.Core.Http.Owin;
using Raven.Client;
using Rhino.Mocks;
using Shouldly;
using Xunit;

namespace FubuMVC.RavenDb.Tests.RavenDb.Integration
{
    public class builds_a_transactional_behavior_raven_session_log_message
    {
        private TransactionalBehaviorRavenSessionUsageMessage theMessage;

        public builds_a_transactional_behavior_raven_session_log_message()
        {
            var advanced = MockRepository.GenerateMock<ISyncAdvancedSessionOperation>();
            advanced.Stub(x => x.NumberOfRequests).Return(10);
            var session = MockRepository.GenerateMock<IDocumentSession>();
            session.Stub(x => x.Advanced).Return(advanced);

            session.Advanced.NumberOfRequests.ShouldBe(10);

            var request = new OwinHttpRequest();
            request.FullUrl("http://something/somethingelse");
            request.HttpMethod("GET");

            var currentPrincipal = new GenericPrincipal(new GenericIdentity("bob"), new string[0]);
            Thread.CurrentPrincipal = currentPrincipal;

            theMessage = TransactionalBehaviorRavenSessionUsageMessage.For(session, request);
        }

        [Fact]
        public void the_url_is_correct()
        {
            theMessage.Url.ShouldBe("http://something/somethingelse");
        }

        [Fact]
        public void the_username_is_correct()
        {
            theMessage.UserName.ShouldBe("bob");
        }

        [Fact]
        public void the_request_number_is_correct()
        {
            theMessage.Requests.ShouldBe(10);
        }

        [Fact]
        public void the_http_method_is_correct()
        {
            theMessage.HttpMethod.ShouldBe("GET");
        }
    }
}
