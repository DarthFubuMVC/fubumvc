using System.Collections.Generic;
using System.Linq;
using System.Web;
using FubuMVC.Core.Security;
using FubuMVC.Core.Web.Security;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.Tests.StructureMapIoC
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
                x.For<HttpContextBase>().Use(new FakeHttpContext());
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