using FubuCore.Configuration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ObjectDefTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            visitor = MockRepository.GenerateMock<IDependencyVisitor>();
        }

        #endregion

        private IDependencyVisitor visitor;

        [Test]
        public void configured_dependency_accept_visitor()
        {
            var dependency = new ConfiguredDependency(GetType(), this);
            dependency.AcceptVisitor(visitor);

            visitor.AssertWasCalled(x => x.Configured(dependency));
        }

        [Test]
        public void value_dependency_accept_visitor()
        {
            var dependency = new ValueDependency(GetType(), this);
            dependency.AcceptVisitor(visitor);

            visitor.AssertWasCalled(x => x.Value(dependency));
        }

        [Test]
        public void as_singleton()
        {
            var def = ObjectDef.ForType<SettingsProvider>();
            def.AsSingleton().ShouldBeTheSameAs(def);

            def.IsSingleton.ShouldBeTrue();
        }
    }
}