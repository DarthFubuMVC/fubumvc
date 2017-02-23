using System;
using FubuCore;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Tickets;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;

namespace FubuMVC.Tests.Security.Authentication.Tickets
{
    
    public class when_marking_authentication : InteractionContext<TicketAuthenticationSession>
    {
        private string theUserName;
        private AuthenticationTicket theResultingTicket;
        private AuthenticationSettings theSettings;

        protected override void beforeEach()
        {
            theSettings = new AuthenticationSettings();
            theSettings.ExpireInMinutes = 30;
            Services.Inject(theSettings);

            LocalSystemTime = DateTime.Today.AddHours(8);

            theUserName = "somebody";

            ClassUnderTest.MarkAuthenticated(theUserName);

            theResultingTicket = MockFor<ITicketSource>().GetArgumentsForCallsMadeOn(x => x.Persist(null))
                [0][0].As<AuthenticationTicket>();
        }

        [Fact]
        public void the_ticket_should_have_the_user_name()
        {
            theResultingTicket.UserName.ShouldBe(theUserName);
        }

        [Fact]
        public void last_accessed_should_be_now()
        {
            theResultingTicket.LastAccessed.ShouldBe(UtcSystemTime);
        }

        [Fact]
        public void expiration_is_now_plus_the_expiration_in_minutes_from_settings()
        {
            var expirationTime = UtcSystemTime.AddMinutes(30);
            theResultingTicket.Expiration.ShouldBe(expirationTime);
        }
    }


    
    public class TicketAuthenticationSession_IsExpired_Tester : InteractionContext<TicketAuthenticationSession>
    {
        private AuthenticationSettings theSettings;
        private AuthenticationTicket theTicket;

        protected override void beforeEach()
        {
            theSettings = new AuthenticationSettings();
            Services.Inject(theSettings);

            LocalSystemTime = DateTime.Today.AddHours(8);

            theTicket = new AuthenticationTicket();
        }

        [Fact]
        public void absolute_expiration()
        {
            theSettings.SlidingExpiration = false;

            theTicket.Expiration = UtcSystemTime.AddMinutes(1);
            ClassUnderTest.IsExpired(theTicket).ShouldBeFalse();

            theTicket.Expiration = UtcSystemTime;
            ClassUnderTest.IsExpired(theTicket).ShouldBeTrue();

            theTicket.Expiration = UtcSystemTime.AddMinutes(-1);
            ClassUnderTest.IsExpired(theTicket).ShouldBeTrue();

        }

        [Fact]
        public void sliding_expiration()
        {
            theSettings.SlidingExpiration = true;
            theSettings.ExpireInMinutes = 30;

            theTicket.LastAccessed = UtcSystemTime;
            ClassUnderTest.IsExpired(theTicket).ShouldBeFalse();

            theTicket.LastAccessed = UtcSystemTime.AddMinutes(-10);
            ClassUnderTest.IsExpired(theTicket).ShouldBeFalse();

            theTicket.LastAccessed = UtcSystemTime.AddMinutes(-30);
            ClassUnderTest.IsExpired(theTicket).ShouldBeTrue();

            theTicket.LastAccessed = UtcSystemTime.AddMinutes(-40);
            ClassUnderTest.IsExpired(theTicket).ShouldBeTrue();
        }
    }


    
    public class finding_a_currently_authenticated_user_when_there_is_none : InteractionContext<TicketAuthenticationSession>
    {
        protected override void beforeEach()
        {
            MockFor<ITicketSource>().Stub(x => x.CurrentTicket()).Return(null);
        }

        [Fact]
        public void should_return_the_null()
        {
            ClassUnderTest.PreviouslyAuthenticatedUser().ShouldBeNull();
        }
    }


    
    public class finding_a_currently_authenticated_user_that_is_not_expired : InteractionContext<TicketAuthenticationSession>
    {
        private AuthenticationTicket theTicket;

        protected override void beforeEach()
        {
            theTicket = new AuthenticationTicket{
                UserName = "somebody"
            };
            MockFor<ITicketSource>().Stub(x => x.CurrentTicket()).Return(theTicket); 

            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Stub(x => x.IsExpired(theTicket)).Return(false);
        }

        [Fact]
        public void should_return_the_username_of_the_current_logged_in_user()
        {
            ClassUnderTest.PreviouslyAuthenticatedUser().ShouldBe(theTicket.UserName);
        }
    }

    
    public class finding_a_currently_authenticated_user_that_is_expired : InteractionContext<TicketAuthenticationSession>
    {
        private AuthenticationTicket theTicket;

        protected override void beforeEach()
        {
            theTicket = new AuthenticationTicket
            {
                UserName = "somebody"
            };

            MockFor<ITicketSource>().Stub(x => x.CurrentTicket()).Return(theTicket);

            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Stub(x => x.IsExpired(theTicket)).Return(true);
        }

        [Fact]
        public void should_return_the_null()
        {
            ClassUnderTest.PreviouslyAuthenticatedUser().ShouldBeNull();
        }

        [Fact]
        public void should_delete_the_existing_ticket()
        {
            ClassUnderTest.PreviouslyAuthenticatedUser();
            MockFor<ITicketSource>().AssertWasCalled(x => x.Delete());
        }
    }


    
    public class when_marking_a_ticket_as_accessed : InteractionContext<TicketAuthenticationSession>
    {
        private AuthenticationTicket theTicket;

        protected override void beforeEach()
        {
            theTicket = new AuthenticationTicket();
            LocalSystemTime = DateTime.Today.AddHours(8);

            MockFor<ITicketSource>().Stub(x => x.CurrentTicket()).Return(theTicket);

            ClassUnderTest.MarkAccessed();
        }

        [Fact]
        public void should_set_the_last_accessed_time_to_now()
        {
            theTicket.LastAccessed.ShouldBe(UtcSystemTime);
        }

        [Fact]
        public void should_persist_the_ticket()
        {
            MockFor<ITicketSource>().AssertWasCalled(x => x.Persist(theTicket));
        }
    }
}