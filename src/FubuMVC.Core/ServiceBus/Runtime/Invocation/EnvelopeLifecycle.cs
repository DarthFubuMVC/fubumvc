using StructureMap;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation
{
    public class EnvelopeLifecycle<T> : IEnvelopeLifecycle where T : IEnvelopeContext
    {
        private readonly IContainer _container;

        // I don't care that this code is StructureMap specific right now
        // we'll deal w/ this later
        public EnvelopeLifecycle(IContainer container)
        {
            _container = container;
        }

        public IEnvelopeContext StartNew(IHandlerPipeline pipeline, Envelope envelope)
        {
            return _container.With(pipeline).With(envelope).GetInstance<T>();
        }
    }
}