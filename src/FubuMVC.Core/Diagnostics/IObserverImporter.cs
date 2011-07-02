using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics
{
    public interface IObserverImporter
    {
        void Import(IConfigurationObserver import);
    }

    public class ObserverImporter : IObserverImporter
    {
        private readonly IConfigurationObserver _importingObserver;
        public ObserverImporter(IConfigurationObserver importingObserver)
        {
            _importingObserver = importingObserver;
        }

        public void Import(IConfigurationObserver import)
        {
            import.RecordedCalls().Each(call => import.GetLog(call)
                .Each(log => _importingObserver.RecordCallStatus(call, log)));
        }
    }
}