using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.StructureMap.Testing.Internals
{
    public class StubActivator : IActivator
    {
        private IPackageLog _log;

        public void Activate(IPackageLog log)
        {
            _log = log;
        }

        public IPackageLog Log
        {
            get { return _log; }
        }
    }
}