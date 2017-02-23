using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.StructureMap;
using Shouldly;
using Xunit;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.StructureMap.Internals
{
    
    public class StructureMapServiceLocatorTester
    {
        public StructureMapServiceLocatorTester()
        {
            _testInstanceKey = "test";

            _mockSecurityContext = MockRepository.GenerateStub<ISecurityContext>();

            container = new Container(x =>
            {
                x.For<ISecurityContext>().Use(_mockSecurityContext);
                x.For<ISecurityContext>().AddInstances(
                    s => s.Type<WebSecurityContext>().Named(_testInstanceKey));
            });
        }


        private string _testInstanceKey;
        private ISecurityContext _mockSecurityContext;
        private IContainer container;

        [Fact]
        public void should_resolve_unnamed_instances()
        {
            new StructureMapServiceLocator(container).GetInstance(typeof (ISecurityContext))
                .ShouldBeTheSameAs(_mockSecurityContext);
        }
    }

}