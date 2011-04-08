using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ListDependencyTester
    {
        [Test]
        public void accepting_the_visitor()
        {
            var visitor = MockRepository.GenerateMock<IDependencyVisitor>();
            var dependency = new ListDependency(typeof (IList<ISomethingDoer>));

            dependency.AcceptVisitor(visitor);

            visitor.AssertWasCalled(x => x.List(dependency));
        }


        [Test]
        public void get_the_element_type()
        {
            new ListDependency(typeof (ISomethingDoer[])).ElementType.ShouldEqual(typeof (ISomethingDoer));
            new ListDependency(typeof (List<ISomethingDoer>)).ElementType.ShouldEqual(typeof (ISomethingDoer));
            new ListDependency(typeof (IList<ISomethingDoer>)).ElementType.ShouldEqual(typeof (ISomethingDoer));
            new ListDependency(typeof (IEnumerable<ISomethingDoer>)).ElementType.ShouldEqual(typeof (ISomethingDoer));
        }

        [Test]
        public void add_value()
        {
            var doer = new ASomethingDoer();

            var dependency = new ListDependency(typeof (ISomethingDoer[]));
            dependency.AddValue(doer);

            dependency.Items.Count().ShouldEqual(1);

            dependency.Items.First().Value.ShouldBeTheSameAs(doer);
        }

        [Test]
        public void add_type()
        {
            var dependency = new ListDependency(typeof(ISomethingDoer[]));
            dependency.AddType(typeof (ASomethingDoer)).Type.ShouldEqual(typeof (ASomethingDoer));

            dependency.Items.Count().ShouldEqual(1);
        }
    }

    public interface ISomethingDoer{}

    public class ASomethingDoer : ISomethingDoer{}
}