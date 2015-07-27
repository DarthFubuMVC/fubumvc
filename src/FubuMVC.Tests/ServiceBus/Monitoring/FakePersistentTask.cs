using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.ServiceBus.Monitoring;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    public class FakePersistentTask : IPersistentTask
    {
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

        public Uri Subject { get; private set; }

        public void AssertAvailable()
        {
            if (Timesout) Thread.Sleep(1.Minutes());

            Thread.Sleep(10);
            if (AssertAvailableException != null) throw AssertAvailableException;
        }

        public void Activate()
        {
            Thread.Sleep(10);
            if (ActivationException != null) throw ActivationException;

            IsActive = true;
        }

        public void Deactivate()
        {
            Thread.Sleep(10);
            if (DeactivateException != null) throw DeactivateException;

            IsActive = false;
        }

        public bool IsActive { get; set; }

        public Task<ITransportPeer> SelectOwner(IEnumerable<ITransportPeer> peers)
        {
            // TODO -- need to make this thing be attached to a parent
            throw new NotImplementedException();
        }

        public void IsFullyFunctionalAndActive()
        {
            IsFullyFunctional();
            IsActive = true;
            Timesout = false;
        }

        public void IsActiveButNotFunctional(Exception exception)
        {
            IsActive = true;
            AssertAvailableException = exception;
            Timesout = false;
        }

        public void IsNotActive()
        {
            IsActive = false;
        }

        public bool Timesout { get; set; }
    }
}