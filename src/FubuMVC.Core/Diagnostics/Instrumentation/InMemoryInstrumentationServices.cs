using StructureMap;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class InMemoryInstrumentationServices : Registry
    {
        public InMemoryInstrumentationServices()
        {
            ForSingletonOf<IExecutionLogStorage>().Use<InMemoryExecutionLogStorage>();

            For<IActivator>().Add<PerformanceHistoryQueueActivator>();

            ForSingletonOf<IPerformanceHistoryQueue>().Use<PerformanceHistoryQueue>();
            ForSingletonOf<IChainExecutionHistory>().Use<ChainExecutionHistory>();
        }
    }
}