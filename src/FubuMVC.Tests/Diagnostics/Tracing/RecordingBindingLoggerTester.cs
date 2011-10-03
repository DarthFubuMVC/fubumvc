using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics.Tracing
{
    [TestFixture]
    public class RecordingBindingLoggerTester : InteractionContext<RecordingBindingLogger>
    {
        [Test]
        public void should_resolve_binder_and_log_selection()
        {
            var property = ReflectionHelper.GetProperty<SimpleModel>(m => m.Name);
            var converter = MockFor<ValueConverter>();

            MockFor<IDebugReport>()
                .Expect(r => r.AddBindingDetail(new ValueConverterSelection()
                {
                    ConverterType = converter.GetType(),
                    PropertyName = "Name",
                    PropertyType = typeof(string)
                }));

            
            ClassUnderTest.ChoseValueConverter(property, converter);

            VerifyCallsFor<IDebugReport>();
        }

        [Test]
        public void should_log_property_binder_selection()
        {
            var property = ReflectionHelper.GetProperty<SimpleModel>(m => m.Name);

            MockFor<IDebugReport>()
                .Expect(r => r.AddBindingDetail(new PropertyBinderSelection
                {
                    BinderType = typeof(NestedObjectPropertyBinder),
                    PropertyName = "Name",
                    PropertyType = typeof(string)
                }));

            ClassUnderTest.ChosePropertyBinder(property, new NestedObjectPropertyBinder());

            VerifyCallsFor<IDebugReport>();
        }

        [Test]
        public void should_log_model_binder_usage()
        {
            var theModelBinder = StandardModelBinder.Basic();
            MockFor<IModelBinderCache>()
                .Expect(c => c.BinderFor(typeof(SimpleModel)))
                .Return(theModelBinder);

            MockFor<IDebugReport>()
                .Expect(r => r.AddBindingDetail(new ModelBinderSelection
                {
                    BinderType = typeof(StandardModelBinder),
                    ModelType = typeof(SimpleModel)
                }));

            ClassUnderTest.ChoseModelBinder(typeof(SimpleModel), theModelBinder);

            VerifyCallsFor<IDebugReport>();
        }

        public class SimpleModel
        {
            public string Name { get; set; }
        }
    }
}