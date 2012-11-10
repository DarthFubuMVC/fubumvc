using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class AddWriterTester
    {
        [Test]
        public void add_a_writer()
        {
            var modification = new AddWriter<WriteString>();

            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<StringEndpoint>(x => x.get_output()));

            modification.Modify(chain);

            chain.Output.Writers.Single().ShouldBeOfType<WriteString>();
        }
    }

    public class StringEndpoint
    {
        public string get_output()
        {
            return "nothing";
        }
    }
}