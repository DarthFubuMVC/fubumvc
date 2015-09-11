using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Services;
using FubuMVC.Core.Services.Messaging;
using FubuMVC.Core.Services.Remote;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace SampleService
{
    public class Foo
    {
        public Foo()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }


    public class SampleRegistry : Registry
    {
        public SampleRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });
        }
    }

    public interface IDependency
    {
    }

    public class Dependency : IDependency
    {
    }

    public class SampleBootstrapper : IApplicationLoader, IDisposable
    {
        public IDisposable Load(Dictionary<string, string> properties)
        {
            //ObjectFactory.Initialize(x => x.AddRegistry<SampleRegistry>());
            EventAggregator.SendMessage(new ServiceStarted{ActivatorTypeName = typeof(SampleService).AssemblyQualifiedName});
            EventAggregator.SendMessage(new ServiceStarted{ActivatorTypeName = typeof(RemoteService).AssemblyQualifiedName});

            return this;
        }

        public void Dispose()
        {
            
        }
    }


    public class SampleService : IActivator, IDeactivator, IListener<Foo>
    {
        private readonly IDependency _dependency;

        public SampleService(IDependency dependency)
        {
            _dependency = dependency;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            Write("Starting SampleService...");

            EventAggregator.Messaging.AddListener(this);
        }

        public void Deactivate(IActivationLog log)
        {
            Write("Stopping SampleService...");
        }

        public void Write(string message)
        {
            Console.WriteLine("From {0}: {1}", GetType().Name, message);
        }

        public void Receive(Foo message)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                EventAggregator.ReceivedMessage(message);
            });
        }
    }

    public class RemoteService : IActivator, IDeactivator, IListener<TestSignal>
    {
        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            EventAggregator.Messaging.AddListener(this);
        }


        public void Deactivate(IActivationLog log)
        {
        }

        public void Receive(TestSignal message)
        {
            EventAggregator.SendMessage(new TestResponse {Number = message.Number});
        }
    }

    public class TestSignal
    {
        public int Number { get; set; }

        protected bool Equals(TestSignal other)
        {
            return Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TestSignal) obj);
        }

        public override int GetHashCode()
        {
            return Number;
        }

        public override string ToString()
        {
            return string.Format("Signal: {0}", Number);
        }
    }

    public class TestResponse
    {
        public int Number { get; set; }

        protected bool Equals(TestResponse other)
        {
            return Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TestResponse) obj);
        }

        public override int GetHashCode()
        {
            return Number;
        }

        public override string ToString()
        {
            return string.Format("Response: {0}", Number);
        }
    }
}