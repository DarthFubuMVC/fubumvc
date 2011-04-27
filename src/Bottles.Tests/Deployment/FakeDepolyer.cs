using Bottles.Deployment;

namespace Bottles.Tests.Deployment
{
    public class FakeDepolyer<T> : IDeployer<T> where T : IDirective
    {
        public void Deploy(HostManifest host, IDirective directive)
        {
            var dir = (T) directive;
            PassedInDirective = dir;
            DeployWasCalled = true;
        }

        public T PassedInDirective { get; private set; }
        public bool DeployWasCalled { get; private set; }
    }
}