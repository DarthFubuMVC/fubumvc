using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Services.Remote;

namespace Serenity
{
    public partial class SerenitySystem<T>
    {
        private readonly IList<ISubSystem> _subSystems = new List<ISubSystem>();
        private readonly Cache<string, RemoteSubSystem> _remoteSubSystems = new Cache<string, RemoteSubSystem>();

        public RemoteSubSystem RemoteSubSystemFor(string name)
        {
            return _remoteSubSystems[name];
        }

        public IEnumerable<RemoteSubSystem> RemoteSubSystems
        {
            get { return _remoteSubSystems; }
        }

        public void AddRemoteSubSystem(string name, Action<RemoteDomainExpression> configuration)
        {

            var system = new RemoteSubSystem(() => new RemoteServiceRunner(x =>
            {
                x.Properties["Mode"] = "testing";
                configuration(x);
            }));

            _remoteSubSystems[name] = system;

            _subSystems.Add(system);
        }

        public void AddSubSystem<TSubsystem>() where TSubsystem : ISubSystem, new()
        {
            AddSubSystem(new TSubsystem());
        }

        public void AddSubSystem(ISubSystem subSystem)
        {
            _subSystems.Add(subSystem);
        }

    }
}