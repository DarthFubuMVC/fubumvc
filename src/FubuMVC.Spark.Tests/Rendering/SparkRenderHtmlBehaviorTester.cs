using System.IO;
using System.Text;
using FubuMVC.Core.Runtime;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class Invoking_SparkRenderHtmlBehavior: InteractionContext<SparkRenderHtmlBehavior<ActionOutputType>>
    {
        private ActionOutputType _outputType;

        protected override void beforeEach()
        {
            _outputType = new ActionOutputType();
            MockFor<IFubuRequest>().Stub(a => a.Get<ActionOutputType>()).Return(_outputType);

            ClassUnderTest.Invoke();
        }

        [Test]
        public void should_get_the_output_model()
        {
            MockFor<IFubuRequest>().AssertWasCalled(a => a.Get<ActionOutputType>());
        }

        [Test]
        public void should_write_to_the_the_output_writer()
        {
            MockFor<IOutputWriter>().AssertWasCalled(a => a.WriteHtml(_outputType));
        }
    }

    [TestFixture]
    public class InvokePartial_SparkRenderHtmlBehavior : InteractionContext<SparkRenderHtmlBehavior<ActionOutputType>>
    {
        private ActionOutputType _outputType;
        private FakeWriter _nestedOutputWriter;

        protected override void beforeEach()
        {
            _outputType = new ActionOutputType();
            MockFor<IFubuRequest>().Stub(a => a.Get<ActionOutputType>()).Return(_outputType);

            var nestedOutput = new NestedOutput();
            _nestedOutputWriter = new FakeWriter();
            nestedOutput.SetWriter(()=>_nestedOutputWriter);
            Services.Inject(nestedOutput);

            ClassUnderTest.InvokePartial();
        }

        [Test]
        public void should_get_the_output_model()
        {
            MockFor<IFubuRequest>().AssertWasCalled(a => a.Get<ActionOutputType>());
        }

        [Test]
        public void should_write_to_the_the_output_writer()
        {
            _nestedOutputWriter.Written.ShouldBeTheSameAs(_outputType);
        }
    }

    public class FakeWriter : TextWriter
    {
        public object Written { get; set; }

        public override void Write(object value)
        {
            Written = value;
        }

        public override Encoding Encoding
        {
            get { return new ASCIIEncoding(); }
        }
    }

    public class ActionOutputType
    {
        
    }
}