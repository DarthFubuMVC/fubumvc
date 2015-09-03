using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Subscriptions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring.PermanentTaskController
{
    [TestFixture]
    public abstract class PersistentTaskControllerContext : ITaskMonitoringSource
    {
        protected RecordingLogger theLogger;
        private Lazy<PersistentTaskController> _controller; 

        protected readonly Cache<string, ITransportPeer> peers =
            new Cache<string, ITransportPeer>(name => MockRepository.GenerateMock<ITransportPeer>());

        protected readonly Cache<string, FakePersistentTaskSource> sources = 
            new Cache<string, FakePersistentTaskSource>(protocol => new FakePersistentTaskSource(protocol));

        protected TransportNode theCurrentNode;
        protected ChannelGraph theGraph;
        protected ISubscriptionRepository theSubscriptions;
        protected HealthMonitoringSettings settings;

        [SetUp]
        public void SetUp()
        {
            peers.ClearAll();
            sources.ClearAll();

            theGraph = new ChannelGraph
            {
                NodeId = "Test@Local"
            };

            theGraph.AddReplyChannel("memory", "memory://1".ToUri());

            theCurrentNode = new TransportNode(theGraph);

            theLogger = new RecordingLogger();

            theSubscriptions = new SubscriptionRepository(theGraph, new InMemorySubscriptionPersistence());
            theSubscriptions.Persist(theCurrentNode);

            settings = new HealthMonitoringSettings
            {
                TaskAvailabilityCheckTimeout = 5.Seconds(),

            };

            _controller = new Lazy<PersistentTaskController>(() => {

                var controller = new PersistentTaskController(theGraph, theLogger, this, sources);

                sources.SelectMany(x => x.FakeTasks()).Select(x => x.Subject)
                    .Each(subject => controller.FindAgent(subject));

                return controller;
            });

            theContextIs();
        }

        protected PersistentTaskController theController
        {
            get
            {
                return _controller.Value;
            }
        }

        protected virtual void theContextIs()
        {
            
        }

        IEnumerable<ITransportPeer> ITaskMonitoringSource.BuildPeers()
        {
            return peers;
        }

        IEnumerable<Uri> ITaskMonitoringSource.LocallyOwnedTasksAccordingToPersistence()
        {
            return theCurrentNode.OwnedTasks;
        }

        public IPersistentTaskAgent BuildAgentFor(IPersistentTask task)
        {
            return new PersistentTaskAgent(task, settings, theLogger, theSubscriptions);
        }

        public void RemoveOwnershipFromThisNode(IEnumerable<Uri> subjects)
        {
            throw new NotImplementedException();
        }


        public FakePersistentTask Task(string uriString)
        {
            var uri = uriString.ToUri();

            return sources[uri.Scheme][uri.Host];
        }

        protected void LoggedMessageForSubject<T>(string uriString) where T : PersistentTaskMessage
        {
            var hasIt = theLogger.InfoMessages.OfType<T>().Any(x => x.Subject == uriString.ToUri());
            if (!hasIt)
            {
                Assert.Fail("Did not have expected log message of type {0} for subject {1}".ToFormat(typeof(T).Name, uriString));
            }
        }

        protected void AssertTasksAreActive(params string[] uriStrings)
        {
            var inactive = uriStrings.Select(Task).Where(x => !x.IsActive).Select(x => x.Subject.ToString());

            if (inactive.Any())
            {
                Assert.Fail("Tasks {0} have not been activated\n" + inactive.Join(", "));
            }
        }

        protected void TheOwnedTasksByTheCurrentNodeShouldBe(params string[] uriStrings)
        {
            theCurrentNode.OwnedTasks.OrderBy(x => x.ToString())
                .ShouldHaveTheSameElementsAs(uriStrings.OrderBy(x => x).Select(x => x.ToUri()));
        }

        protected void ExceptionWasLogged(Exception ex)
        {
            theLogger.ErrorMessages.OfType<ErrorReport>().Any(x => x.ExceptionText.Contains(ex.ToString()));
        }

    }
}