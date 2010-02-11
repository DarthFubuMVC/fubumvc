using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Models;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Runtime;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.Diagnostics
{


    public class InMemoryBindingContext : BindingContext
    {
        private InMemoryRequestData _data;
        private IContainer _container;

        public InMemoryBindingContext() : this(new InMemoryRequestData(), new Container())
        {
        }

        private InMemoryBindingContext(InMemoryRequestData data, IContainer container)
            : base(data, new StructureMapServiceLocator(container))
        {
            _data = data;
            _container = container;
        }

        public InMemoryBindingContext WithData(string key, object value)
        {
            this[key] = value;
            return this;
        }

        public InMemoryBindingContext WithPropertyValue(object value)
        {
            PropertyValue = value;
            return this;
        }

        public InMemoryRequestData Data { get { return _data; } }
        public IContainer Container { get { return _container; } }

        public object this[string key] { get { return _data[key]; } set { _data[key] = value; } }
    }

    [TestFixture]
    public class when_resolving_successfully : InteractionContext<RecordingObjectResolver>
    {
        private BindResult result;
        private InMemoryBindingContext context;

        protected override void beforeEach()
        {
            result = new BindResult
            {
                Value = new object()
            };
            context = new InMemoryBindingContext();
            MockFor<IObjectResolver>().Expect(x => x.BindModel(typeof (BinderTarget), context)).Return(result);

            ClassUnderTest.BindModel(typeof (BinderTarget), context).ShouldBeTheSameAs(result);
        }

        [Test]
        public void should_report_the_end_of_the_binding()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.EndModelBinding(result.Value));
        }

        [Test]
        public void should_start_a_new_binding_report()
        {
            MockFor<IDebugReport>().AssertWasCalled(x => x.StartModelBinding(typeof (BinderTarget)));
        }
    }
}