using System.IO;
using System.Xml;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;
using FubuCore.Configuration;
using System.Linq;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class RecordedOutputTester
    {
        private IHttpWriter theHttpWriter;
        private RecordedOutput theRecordedOutput;

        [SetUp]
        public void SetUp()
        {
            theHttpWriter = MockRepository.GenerateMock<IHttpWriter>();
            theRecordedOutput = new RecordedOutput();
        }

        [Test]
        public void the_replay_method_calls_all_the_recorded_outputs_in_order()
        {
            var output1 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output2 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output3 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output4 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output5 = MockRepository.GenerateMock<IRecordedHttpOutput>();
        
            theRecordedOutput.AddOutput(output1);
            theRecordedOutput.AddOutput(output2);
            theRecordedOutput.AddOutput(output3);
            theRecordedOutput.AddOutput(output4);
            theRecordedOutput.AddOutput(output5);
        
            theRecordedOutput.Replay(theHttpWriter);

            output1.AssertWasCalled(x => x.Replay(theHttpWriter));
            output2.AssertWasCalled(x => x.Replay(theHttpWriter));
            output3.AssertWasCalled(x => x.Replay(theHttpWriter));
            output4.AssertWasCalled(x => x.Replay(theHttpWriter));
            output5.AssertWasCalled(x => x.Replay(theHttpWriter));
        }

        [Test]
        public void write_stream()
        {
            var document = new XmlDocument();
            document.WithRoot("root");

            theRecordedOutput.Write("text/xml", stream =>
            {
                document.Save(stream);
            });

            theRecordedOutput.Outputs.First().ShouldEqual(new SetContentType("text/xml"));

            var writeStream = theRecordedOutput.Outputs.Last().ShouldBeOfType<WriteStream>();

            writeStream.ReadAll().ShouldEqual(document.OuterXml);
        }

        [Test]
        public void append_header_recording()
        {
            theRecordedOutput.AppendHeader("key", "value");

            theRecordedOutput.Outputs.ShouldHaveTheSameElementsAs(new Header("key", "value"));
        }

        [Test]
        public void write_textual_content_adds_the_correct_recordings()
        {
            theRecordedOutput.Write("text/json", "{}");

            theRecordedOutput.Outputs.ShouldHaveTheSameElementsAs(
                new SetContentType("text/json"),
                new WriteText("{}")
                );
        }

        [Test]
        public void SetContentType_replay_writes_the_content_type_to_the_HttpWriter()
        {
            var setContentType = new SetContentType("text/json");
            setContentType.Replay(theHttpWriter);

            theHttpWriter.AssertWasCalled(x => x.WriteContentType("text/json"));
        }

        [Test]
        public void WriteText_replay_writes_the_text_to_the_HttpWriter()
        {
            var writeText = new WriteText("something");
            writeText.Replay(theHttpWriter);

            theHttpWriter.AssertWasCalled(x => x.Write("something"));
        }

        [Test]
        public void WriteStream_Replay_copies_to_a_stream()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("Hello!");
            writer.Flush();

            var writeStream = new WriteStream(stream);

            var recordingWriter = new RecordingHttpWriter();

            writeStream.Replay(recordingWriter);

            recordingWriter.AllText().Trim().ShouldEqual("Hello!");
        }
    }
}