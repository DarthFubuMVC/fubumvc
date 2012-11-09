using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using TestPackage1;

namespace FubuMVC.Tests.Registration.Scanning
{
    [TestFixture]
    public class AssemblyScannerTester
    {
        private AssemblyScanner _scanner;
        private ServiceRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _scanner = new AssemblyScanner();
            _registry = new ServiceRegistry();
        }

        [Test]
        public void should_add_implementations_of_plugin_type()
        {
            _scanner
                .Applies
                .ToAssemblyContainingType<IMultiInstance>();
            
            _scanner
                .AddAllTypesOf<IMultiInstance>();

            _scanner
                .Configure(_registry);

            var services = _registry.ToGraph().ServicesFor<IMultiInstance>();
            services.ShouldContain(def => def.Type == typeof(Instance1));
            services.ShouldContain(def => def.Type == typeof(Instance2));
        }

        [Test]
        public void should_connect_implementations()
        {
            _scanner
                .Applies
                .ToAssemblyContainingType(typeof(IOpenType<>));

            _scanner
                .ConnectImplementationsToTypesClosing(typeof (IOpenType<>));

            _scanner
                .Configure(_registry);

            var services = _registry.ToGraph().ServicesFor(typeof(IOpenType<>));
            services.ShouldContain(def => def.Type == typeof(OpenType<>));
        }
    }
}