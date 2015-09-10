using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.ServiceBus.Monitoring;
using FubuMVC.Core.ServiceBus.Subscriptions;
using OpenQA.Selenium.Safari.Internal;
using StoryTeller;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Monitoring
{
    public class FakePersistentTask : IPersistentTask
    {
        private IEnumerable<string> _preferredNodes = new string[0];
        public Exception ActivationException = null;
        public Exception AssertAvailableException = null;
        public Exception DeactivateException = null;

        public FakePersistentTask(Uri subject)
        {
            Subject = subject;
        }

        public void IsFullyFunctional()
        {
            Timesout = false;
            ActivationException = AssertAvailableException = null;
        }

        public IEnumerable<string> PreferredNodes
        {
            get { return _preferredNodes; }
            set { _preferredNodes = value; }
        }

        public Uri Subject { get; private set; }

        public void AssertAvailable()
        {
            if (Timesout)
            {
                Debug.WriteLine("Trying to timeout task " + Subject);
                Thread.Sleep(1.Minutes());
            }
            Thread.Sleep(10);
            if (AssertAvailableException != null) throw AssertAvailableException;
        }

        public void Activate()
        {
            Thread.Sleep(10);
            if (Timesout)
            {
                Thread.Sleep(1.Minutes());
            }
            if (ActivationException != null) throw ActivationException;

            IsActive = true;
        }

        public void Deactivate()
        {
            if (DeactivateException != null) throw DeactivateException;

            IsActive = false;
        }

        public bool IsActive { get; set; }

        public Task<ITransportPeer> SelectOwner(IEnumerable<ITransportPeer> peers)
        {
            var ordered = _preferredNodes.Select(x => peers.FirstOrDefault(_ => _.NodeId == x))
                .Where(x => x != null);

            StoryTellerAssert.Fail(!ordered.Any(), "No preferred nodes established for this test node");

            var assignment = new OrderedAssignment(Subject, ordered);

            return assignment.SelectOwner();
        }

        public void IsFullyFunctionalAndActive()
        {
            IsFullyFunctional();
            IsActive = true;
        }

        public void IsActiveButNotFunctional(Exception exception)
        {
            IsActive = true;
            AssertAvailableException = exception;
        }

        public void IsNotActive()
        {
            IsActive = false;
        }

        public void SetState(string state, ISubscriptionPersistence persistence, string nodeId)
        {
            Timesout = false;

            switch (state)
            {
                case MonitoredNode.HealthyAndFunctional:
                    IsFullyFunctionalAndActive();
                    persistence.Alter(nodeId, node => node.AddOwnership(Subject));

                    break;

                case MonitoredNode.ThrowsExceptionOnStartupOrHealthCheck:
                    ActivationException = new DivideByZeroException();
                    AssertAvailableException = new HandshakeException();
                    break;

                case MonitoredNode.TimesOutOnStartupOrHealthCheck:
                    Timesout = true;
                    break;

                case MonitoredNode.IsInactive:
                    IsFullyFunctional();
                    IsActive = false;
                    break;
            }
        }

        public bool Timesout { get; set; }
    }
}