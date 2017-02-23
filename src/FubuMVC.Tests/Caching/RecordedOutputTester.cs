using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using Xunit;
using Rhino.Mocks;
using Shouldly;
using FubuCore.Configuration;
using System.Linq;

namespace FubuMVC.Tests.Caching
{
    
    public class RecordedOutputTester
    {
        private IHttpResponse theHttpResponse;
        private RecordedOutput theRecordedOutput;

        public RecordedOutputTester()
        {
            theHttpResponse = MockRepository.GenerateMock<IHttpResponse>();
            theRecordedOutput = new RecordedOutput(new FileSystem());
        }


        [Fact]
        public void get_headers()
        {
            theRecordedOutput.AppendHeader("a", "1");
            theRecordedOutput.AppendHeader("b", "2");
            theRecordedOutput.AppendHeader("c", "3");

            theRecordedOutput.Write("stuff");
            theRecordedOutput.Write("more stuff");
            theRecordedOutput.Write("other stuff");

            theRecordedOutput.Headers().ShouldHaveTheSameElementsAs(new Header("a", "1"),new Header("b", "2"),new Header("c", "3"));
        }

        [Fact]
        public void the_replay_method_calls_all_the_recorded_outputs_in_order()
        {            
            var output1 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output2 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output3 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output4 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output5 = MockRepository.GenerateMock<IRecordedHttpOutput>();

            addOutputs(output1, output2, output3, output4, output5);
        
            theRecordedOutput.Replay(theHttpResponse);

            output1.AssertWasCalled(x => x.Replay(theHttpResponse));
            output2.AssertWasCalled(x => x.Replay(theHttpResponse));
            output3.AssertWasCalled(x => x.Replay(theHttpResponse));
            output4.AssertWasCalled(x => x.Replay(theHttpResponse));
            output5.AssertWasCalled(x => x.Replay(theHttpResponse));
        }

        [Fact]
        public void the_get_text_method_calls_relevant_outputs_in_order()
        {
            var output1 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output2 = MockRepository.GenerateMock<IRecordedHttpOutput, IRecordedTextOutput>();
            var output3 = MockRepository.GenerateMock<IRecordedHttpOutput>();
            var output4 = MockRepository.GenerateMock<IRecordedHttpOutput, IRecordedTextOutput>();

            addOutputs(output1, output2, output3, output4);
            theRecordedOutput.GetText();

            output2.As<IRecordedTextOutput>().AssertWasCalled(x => x.WriteText(Arg<StringWriter>.Is.NotNull));
            output4.As<IRecordedTextOutput>().AssertWasCalled(x => x.WriteText(Arg<StringWriter>.Is.NotNull));
        }

        [Fact]
        public void the_get_text_method_returns_empty_string_when_no_text()
        {
            var output = MockRepository.GenerateMock<IRecordedHttpOutput>();
            
            addOutputs(output);
            
            theRecordedOutput.GetText().ShouldBeEmpty();
        }

        [Fact]
        public void the_get_text_method_appends_text_output_in_order()
        {            
            addOutputs(new WriteTextOutput("hep"), new WriteTextOutput("hey"));
            theRecordedOutput.GetText().ShouldBe("hephey");
        }

        [Fact]
        public void write_stream()
        {
            var document = new XmlDocument();
            document.WithRoot("root");

            theRecordedOutput.Write("text/xml", document.Save);
            theRecordedOutput.Outputs.First().ShouldBe(new SetContentType("text/xml"));

            var writeStream = theRecordedOutput.Outputs.Last().ShouldBeOfType<WriteStream>();

            writeStream.ReadAll().ShouldBe(document.OuterXml);
        }

        [Fact]
        public void append_header_recording()
        {
            theRecordedOutput.AppendHeader("key", "value");

            theRecordedOutput.Outputs.ShouldHaveTheSameElementsAs(new Header("key", "value"));
        }

        [Fact]
        public void write_textual_content_adds_the_correct_recordings()
        {
            theRecordedOutput.Write("text/json", "{}");

            theRecordedOutput.Outputs.ShouldHaveTheSameElementsAs(
                new SetContentType("text/json"),
                new WriteTextOutput("{}")
                );
        }


        [Fact]
        public void write_only_textual_content_adds_the_correct_recordings()
        {
            theRecordedOutput.Write("{}");

            theRecordedOutput.Outputs.ShouldHaveTheSameElementsAs(
                new WriteTextOutput("{}")
                );
        }

        [Fact]
        public void SetContentType_replay_writes_the_content_type_to_the_HttpWriter()
        {
            var setContentType = new SetContentType("text/json");
            setContentType.Replay(theHttpResponse);

            theHttpResponse.AssertWasCalled(x => x.WriteContentType("text/json"));
        }

        [Fact]
        public void WriteTextOutput_replay_writes_the_text_to_the_HttpWriter()
        {
            var writeText = new WriteTextOutput("something");
            writeText.Replay(theHttpResponse);

            theHttpResponse.AssertWasCalled(x => x.Write("something"));
        }

        public void WriteTextOutput_writetext_writes_the_text_to_the_stringwriter()
        {
            var writer = new StringWriter();
            var writeText = new WriteTextOutput("jambalay");
            writeText.WriteText(writer);
            writer.ToString().ShouldBe("jambalay");
        }

        [Fact]
        public void WriteStream_Replay_copies_to_a_stream()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("Hello!");
            writer.Flush();

            var writeStream = new WriteStream(stream);

            var recordingWriter = new RecordingHttpResponse();

            writeStream.Replay(recordingWriter);

            recordingWriter.AllText().Trim().ShouldBe("Hello!");
        }

        [Fact]
        public void write_file()
        {
            new FileSystem().WriteStringToFile("text.txt", "some text");

            theRecordedOutput.WriteFile("text/plain", "text.txt", "This is cool");

            theRecordedOutput.Replay(theHttpResponse);

            theHttpResponse.AssertWasCalled(x => x.WriteContentType("text/plain"));
            theHttpResponse.AssertWasCalled(x => x.AppendHeader(HttpResponseHeaders.ContentDisposition, "attachment; filename=\"This is cool\""));
            theHttpResponse.AssertWasCalled(x => x.AppendHeader(HttpResponseHeaders.ContentLength, "9"));

            theHttpResponse.AssertWasCalled(x => x.WriteFile("text.txt"));
        }

        private void addOutputs(params IRecordedHttpOutput[] outputs)
        {
            outputs.Each(theRecordedOutput.AddOutput);
        }
    }
}