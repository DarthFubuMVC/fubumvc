using FubuMVC.Core.Runtime;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewOutputTester : InteractionContext<ViewOutput>
    {
        private IOutputWriter _outputWriter;
        protected override void beforeEach()
        {
            _outputWriter = MockFor<IOutputWriter>();
            Services.Inject(_outputWriter);
        }

        [Test]
        public void writes_the_string_value_directly_to_the_output_writer()
        {
            _outputWriter.Expect(x => x.WriteHtml("Hello World"));
            ClassUnderTest.Write("Hello World");
            _outputWriter.VerifyAllExpectations();
        }
    }
}