using FubuMVC.Core;
using NUnit.Framework;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Diagnostics;
using System.Collections.Generic;
using FubuTestingSupport;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class ObserverImporterTester
    {
        private FubuRegistry _parent;
        private FubuRegistry _import;

        private IConfigurationObserver _parentObserver;
        private IConfigurationObserver _importObserver;
        private IEnumerable<ActionCall> _importActions = Enumerable.Empty<ActionCall>();
            
        [SetUp]
        public void BeforeEach()
        {
            _parent = new FubuRegistry();
            _parent.IncludeDiagnostics(true);

            _import = new FubuRegistry();
            _import.IncludeDiagnostics(true);
            _import.Actions
                .IncludeType<Handler1>()
                .IncludeType<Handler2>();
            
            _import.Configure(x =>
            {
                _importObserver = x.Observer;
                _importActions = x.Actions()
                    .Where(a => a.HandlerType.Namespace == GetType().Namespace);
            });

            _parent.Import(_import, "import");
            _parentObserver = _parent.BuildGraph().Observer;
        }

        [Test]
        public void importing_registry_should_contain_records_for_imported_calls()
        {
            _parentObserver.RecordedCalls()
                .ShouldHaveCount(2)
                .ShouldHaveTheSameElementsAs(_importActions);
        }

        [Test]
        public void observer_for_importing_registry_should_contain_messages_for_imported_calls()
        {         
            _importActions.Each(a =>
            {
                _parentObserver.GetLog(a)
                    .ShouldHaveTheSameElementsAs(_importObserver.GetLog(a));                                                   
            });
        }
    }

    public class Handler1
    {
        public void Call(Input1 input1){}
    }
    public class Input1 {}

    public class Handler2
    {
        public void Call(Input2 input2) { }
    }
    public class Input2 { }
}