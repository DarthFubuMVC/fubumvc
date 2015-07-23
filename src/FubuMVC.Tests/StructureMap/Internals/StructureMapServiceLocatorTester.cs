using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.StructureMap;
using Shouldly;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.StructureMap.Internals
{
    [TestFixture]
    public class StructureMapServiceLocatorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
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

        #endregion

        private string _testInstanceKey;
        private ISecurityContext _mockSecurityContext;
        private IContainer container;

        [Test]
        public void should_resolve_unnamed_instances()
        {
            new StructureMapServiceLocator(container).GetInstance(typeof (ISecurityContext))
                .ShouldBeTheSameAs(_mockSecurityContext);
        }
    }

}