using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using StructureMap.AutoMocking;

namespace FubuTestingSupport
{
    public class InteractionContext<T> where T : class
    {
        private readonly MockMode _mode;

        public InteractionContext() : this(MockMode.AAA) { }
        public InteractionContext(MockMode mode)
        {
            _mode = mode;
        }

        public IContainer Container { get { return Services.Container; } }
        public RhinoAutoMocker<T> Services { get; private set; }
        public T ClassUnderTest { get { return Services.ClassUnderTest; } }

        [SetUp]
        public void SetUp()
        {
            Services = new RhinoAutoMocker<T>(_mode);
            beforeEach();
        }

        // Override this for context specific setup
        protected virtual void beforeEach() {}

        public SERVICE MockFor<SERVICE>() where SERVICE : class
        {
            return Services.Get<SERVICE>();
        }

        public void VerifyCallsFor<MOCK>() where MOCK : class
        {
            MockFor<MOCK>().VerifyAllExpectations();
        }
    }
}