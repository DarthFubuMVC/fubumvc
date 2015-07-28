using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Tests.StructureMap.Internals
{
    public class StubActivator : IActivator
    {
        private IActivationLog _log;

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            _log = log;
        }

        public IActivationLog Log
        {
            get { return _log; }
        }
    }
}