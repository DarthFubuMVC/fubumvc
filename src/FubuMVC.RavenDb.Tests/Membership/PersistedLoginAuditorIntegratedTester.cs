using System;
using System.Linq;
using FubuCore.Dates;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Auditing;
using FubuMVC.RavenDb.Membership;
using FubuMVC.RavenDb.RavenDb;
using NUnit.Framework;
using Raven.Client;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.Membership
{
    [TestFixture]
    public class PersistedLoginAuditorIntegratedTester
    {
        private SettableClock theTime;
        private Container theContainer;

        [SetUp]
        public void SetUp()
        {
            theTime = new SettableClock();
            theTime.LocalNow(LocalTime.AtMachineTime("1200")); // doesn't matter what, only needs to be constant

            theContainer = new Container(x =>
            {
                x.IncludeRegistry<RavenDbRegistry>();
                x.For<RavenDbSettings>()
                    .Use(new RavenDbSettings
                    {
                        RunInMemory = true,
                        DataDirectory = null,
                        Url = null,
                        ConnectionString = null
                    });

                x.For<ISystemTime>().Use(theTime);
            });
        }

        [TearDown]
        public void TearDown()
        {
            theContainer.Dispose();
        }

        [Test]
        public void write_audit_message()
        {
            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            auditor.Audit(new Something {UserName = "the something"});

            var theAudit =
                theContainer.GetInstance<IEntityRepository>().All<Audit>().Where(x => x.Type == "Something").Single();
            theAudit.Message.ShouldBeOfType<Something>().UserName.ShouldBe("the something");
            theAudit.Timestamp.ShouldBe(theTime.UtcNow());
            theAudit.Username.ShouldBe("the something");
        }

        [Test]
        public void write_login_success()
        {
            var request = new LoginRequest
            {
                Status = LoginStatus.Succeeded,
                UserName = "somebody"
            };

            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            auditor.Audit(request);

            var theAudit = theContainer.GetInstance<IEntityRepository>().All<Audit>()
                .Where(x => x.Type == "LoginSuccess").Single();

            theAudit.Message.ShouldBeOfType<LoginSuccess>();
            theAudit.Timestamp.ShouldBe(theTime.UtcNow());
            theAudit.Username.ShouldBe("somebody");
        }

        [Test]
        public void write_login_failure()
        {
            var request = new LoginRequest
            {
                Status = LoginStatus.Failed,
                UserName = "FailedGuy"
            };

            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            auditor.Audit(request);

            var theAudit = theContainer.GetInstance<IEntityRepository>().All<Audit>()
                .Where(x => x.Type == "LoginFailure").Single();

            theAudit.Message.ShouldBeOfType<LoginFailure>();
            theAudit.Timestamp.ShouldBe(theTime.UtcNow());
            theAudit.Username.ShouldBe(request.UserName);
        }


        [Test]
        public void when_logging_success_wipe_clean_the_login_failure_history()
        {
            var history = new LoginFailureHistory
            {
                Id = "doofus",
                Attempts = 3
            };

            theContainer.GetInstance<ITransaction>().Execute<IDocumentSession>(repo => { repo.Store(history); });


            var request = new LoginRequest
            {
                Status = LoginStatus.Succeeded,
                UserName = history.Id
            };

            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            auditor.Audit(request);
        }

        [Test]
        public void when_logging_failure_for_a_user_that_has_no_prior_failure_history()
        {
            var request = new LoginRequest
            {
                Status = LoginStatus.Failed,
                UserName = "NeverFailedBefore",
                NumberOfTries = 1,
                LockedOutUntil = null
            };

            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            auditor.Audit(request);


            var history = theContainer.GetInstance<IDocumentSession>().Load<LoginFailureHistory>(request.UserName);


            history.ShouldNotBeNull();
            history.Attempts.ShouldBe(1);
            history.LockedOutTime.ShouldBeNull();
        }

        [Test]
        public void when_logging_failure_for_a_user_that_is_locked_out()
        {
            var request = new LoginRequest
            {
                Status = LoginStatus.Failed,
                UserName = "NeverFailedBefore",
                NumberOfTries = 1,
                LockedOutUntil = DateTime.Today.ToUniversalTime()
            };

            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            auditor.Audit(request);


            var history = theContainer.GetInstance<IDocumentSession>()
                .Load<LoginFailureHistory>(request.UserName);


            history.LockedOutTime.ShouldBe(request.LockedOutUntil);
        }

        [Test]
        public void update_an_existing_history()
        {
            var history = new LoginFailureHistory
            {
                Id = "AlreadyFailed",
                Attempts = 2
            };

            theContainer.GetInstance<ITransaction>().Execute<IDocumentSession>(repo => { repo.Store(history); });


            var request = new LoginRequest
            {
                Status = LoginStatus.Failed,
                UserName = history.Id,
                NumberOfTries = 3,
                LockedOutUntil = DateTime.Today.ToUniversalTime()
            };

            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            auditor.Audit(request);


            var history2 = theContainer.GetInstance<IDocumentSession>()
                .Load<LoginFailureHistory>(request.UserName);

            history2.Attempts.ShouldBe(request.NumberOfTries);
            history2.LockedOutTime.ShouldBe(request.LockedOutUntil);
        }

        [Test]
        public void apply_history_when_there_is_no_history()
        {
            var request = new LoginRequest
            {
                NumberOfTries = 5,
                UserName = "NoHistoryGuy"
            };


            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            auditor.ApplyHistory(request);

            request.NumberOfTries.ShouldBe(5); // Nothing gets replaced
        }

        [Test]
        public void apply_history_when_there_is_prior_history()
        {
            var history = new LoginFailureHistory
            {
                Id = "AlreadyLockedOut",
                Attempts = 3,
                LockedOutTime = DateTime.Today.AddMinutes(30)
            };

            theContainer.GetInstance<ITransaction>().Execute<IDocumentSession>(repo => repo.Store(history));

            var auditor = theContainer.GetInstance<PersistedLoginAuditor>();
            var request = new LoginRequest
            {
                UserName = history.Id
            };

            auditor.ApplyHistory(request);

            request.NumberOfTries.ShouldBe(history.Attempts);
            request.LockedOutUntil.ShouldBe(history.LockedOutTime.Value);
        }
    }

    public class Something : AuditMessage
    {
    }
}