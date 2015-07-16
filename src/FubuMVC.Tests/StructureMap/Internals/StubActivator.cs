using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.StructureMap.Testing.Internals
{
    public class StubActivator : IActivator
    {
        private IActivationLog _log;

        public void Activate(IActivationLog log)
        {
            _log = log;
        }

        public IActivationLog Log
        {
            get { return _log; }
        }
    }
}