using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class ActionCallBaseObjectDefCreationTester
    {
        private ActionCall theCall;

        [SetUp]
        public void SetUp()
        {
            theCall = ActionCall.For<Handler>(x => x.Go());
        }

        [Test]
        public void with_no_explicit_action_dependencies()
        {
            var def = theCall.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);
        
        
            def.FindDependencyDefinitionFor(typeof(Handler)).ShouldBeNull();
        }

        [Test]
        public void register_a_dependency_for_the_handler()
        {
            theCall.HandlerDef.DependencyByType<IService, Service2>();

            var def = theCall.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);
            def.FindDependencyDefinitionFor(typeof (Handler))
                .FindDependencyDefinitionFor(typeof (IService)).Type.ShouldEqual(typeof (Service2));
        }

        [Test]
        public void register_a_handler_dependency_by_value()
        {
            var theData = new Data();
            theCall.HandlerDef.DependencyByValue(theData);

            var def = theCall.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);
            def.FindDependencyDefinitionFor(typeof (Handler))
                .Dependencies.Single(x => x.DependencyType == typeof (Data))
                .ShouldBeOfType<ValueDependency>()
                .Value.ShouldBeTheSameAs(theData);
        }

        public interface IService{}
        public class Service1 : IService{}
        public class Service2 : IService { }

        public class Data{}

        public class Handler
        {
            private readonly IService _service;
            private readonly Data _data;

            public Handler(IService service, Data data)
            {
                _service = service;
                _data = data;
            }

            public Data Go()
            {
                return new Data();
            }
        }
    }


}