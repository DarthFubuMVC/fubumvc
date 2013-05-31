using System.Web;

using Autofac;

using FubuMVC.Core.Security;
using FubuMVC.Core.Web.Security;

using FubuTestingSupport;

using NUnit.Framework;

using Rhino.Mocks;


namespace FubuMVC.Autofac.Testing.Internals
{
    [TestFixture]
    public class AutofacServiceLocatorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _testInstanceKey = "test";

            _mockSecurityContext = MockRepository.GenerateStub<ISecurityContext>();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(_mockSecurityContext).As<ISecurityContext>();
            builder.RegisterType<WebSecurityContext>().Named<ISecurityContext>(_testInstanceKey);
            context = builder.Build();
        }

        #endregion

        private string _testInstanceKey;
        private ISecurityContext _mockSecurityContext;
        private IComponentContext context;

        [Test]
        public void should_resolve_unnamed_instances()
        {
            new AutofacServiceLocator(context).GetInstance(typeof(ISecurityContext))
                                              .ShouldBeTheSameAs(_mockSecurityContext);
        }
    }

}