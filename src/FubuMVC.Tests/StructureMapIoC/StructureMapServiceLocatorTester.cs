using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Security;
using FubuMVC.Core.Web.Security;
using FubuMVC.StructureMap;
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
                x.For<ISecurityContext>().AddInstances(
                    s => s.Type<WebSecurityContext>().Named(_testInstanceKey));
            });
        }

        #endregion

        private string _testInstanceKey;
        private ISecurityContext _mockSecurityContext;
        private IContainer container;

        [Test]
        public void should_get_all_instances_for_a_given_type()
        {
            IEnumerable<object> instances = new StructureMapServiceLocator(container)
                .GetAllInstances(typeof (ISecurityContext));

            instances.Single(i => i.GetType() == typeof (WebSecurityContext));
            instances.Single(i => ReferenceEquals(i, _mockSecurityContext));
        }

        [Test]
        public void should_resolve_named_instances()
        {
            new StructureMapServiceLocator(container).GetInstance(typeof (ISecurityContext), _testInstanceKey)
                .ShouldBeOfType<WebSecurityContext>();
        }

        [Test]
        public void should_resolve_unnamed_instances()
        {
            new StructureMapServiceLocator(container).GetInstance(typeof (ISecurityContext))
                .ShouldBeTheSameAs(_mockSecurityContext);
        }
    }
}