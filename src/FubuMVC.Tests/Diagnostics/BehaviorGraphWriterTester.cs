using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Urls;
using HtmlTags;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture, Explicit]
    public class BehaviorGraphWriterTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry().BuildGraph();
            urls = MockRepository.GenerateMock<IUrlRegistry>();
            
            writer = new BehaviorGraphWriter(graph, urls, null);
        }

        #endregion

        private BehaviorGraph graph;
        private BehaviorGraphWriter writer;
        private IUrlRegistry urls;

        [Test]
        public void smoke_test_actions()
        {
            Debug.WriteLine(writer.PrintActions());
        }

        [Test]
        public void smoke_test_actions_table()
        {
            writer.Actions();
        }

        [Test]
        public void smoke_test_index()
        {
            HtmlDocument doc = writer.Index();
            Debug.WriteLine(doc.ToString());
        }

        [Test]
        public void smoke_test_input_models_table()
        {
            writer.Inputs();
        }

        [Test]
        public void smoke_test_routes()
        {
            Debug.WriteLine(writer.PrintRoutes());
        }

        [Test]
        public void smoke_test_routes_table()
        {
            HtmlDocument routes = writer.Routes();
        }

        [Test]
        public void smoke_test_chain()
        {
            Debug.WriteLine(writer.Chain(new ChainRequest {Id = graph.Behaviors.First().UniqueId}));
        }
    }
}