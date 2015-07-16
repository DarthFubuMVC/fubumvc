using System;
using System.Threading.Tasks;
using Bottles.Diagnostics;
using Bottles.Services.Messaging;
using Bottles.Services.Remote;
using FubuCore;

namespace Bottles.Services
{
    public interface IBottleService
    {
        void Start();
        void Stop();

        Task ToTask();
    }

    public class BottleService : IBottleService
    {
        private readonly IActivator _activator;
        private readonly IPackageLog _log;

        public BottleService(IActivator activator, IPackageLog log)
        {
            _activator = activator;
            _log = log;

            if(!IsBottleService(activator))
            {
                throw new ArgumentException("Activator must also implement {0}".ToFormat(typeof(IDeactivator).Name), "activator");
            }
        }

        public void Start()
        {
            _activator.Activate(new IPackageInfo[0], _log);
            EventAggregator.SendMessage(new ServiceStarted
            {
                ActivatorTypeName = _activator.GetType().AssemblyQualifiedName
            });

            Console.WriteLine("Started service " + _activator);
        }

        public void Stop()
        {
            var deactivator = _activator as IDeactivator;
            if (deactivator != null)
            {
                deactivator.Deactivate(_log);
            }
        }

        public Task ToTask()
        {
            return new Task(Start, TaskCreationOptions.LongRunning);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BottleService)) return false;
            return Equals((BottleService) obj);
        }

        public bool Equals(BottleService other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._activator.GetType() == _activator.GetType();
        }

        public override int GetHashCode()
        {
            return _activator.GetType().GetHashCode();
        }

        public static bool IsBottleService(Type type)
        {
            return type.IsConcreteTypeOf<IActivator>() && type.IsConcreteTypeOf<IDeactivator>();
        }

        public static bool IsBottleService(object service)
        {
            return IsBottleService(service.GetType());
        }

        public static BottleService For(IActivator service)
        {
            return new BottleService(service, new PackageLog());
        }

        public override string ToString()
        {
            return string.Format("Service: {0}", _activator);
        }
    }
}