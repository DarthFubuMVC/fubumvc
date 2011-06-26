using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class RecordingValueConverterRegistryTester : InteractionContext<RecordingValueConverterRegistry>
    {
        [Test]
        public void should_resolve_binder_and_log_selection()
        {
            var property = ReflectionHelper.GetProperty<SimpleModel>(m => m.Name);

            MockFor<IDebugReport>()
                .Expect(r => r.AddBindingDetail(new ValueConverterSelection()
                                                    {
                                                        ConverterType = typeof(TypeDescriptorConverterFamily.BasicValueConverter),
                                                        PropertyName = "Name",
                                                        PropertyType = typeof(string)
                                                    }));

            ClassUnderTest
                .FindConverter(property)
                .ShouldBeOfType<TypeDescriptorConverterFamily.BasicValueConverter>();

            VerifyCallsFor<IDebugReport>();
        }

        public class SimpleModel
        {
            public string Name { get; set; }
        }
    }
}