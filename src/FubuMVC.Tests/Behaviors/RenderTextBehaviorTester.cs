using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Behaviors
{
    [TestFixture]
    public class RenderTextBehaviorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            request = new InMemoryFubuRequest();
            writer = MockRepository.GenerateMock<IOutputWriter>();


            request.Set("some text");

            var behavior = new RenderTextBehavior<string>(writer, request, MimeType.Html);
            behavior.Invoke();
        }

        #endregion

        private InMemoryFubuRequest request;
        private IOutputWriter writer;

        [Test]
        public void should_write_the_to_string_render_of_the_type_to_the_output_writer()
        {
            writer.AssertWasCalled(x => x.Write(MimeType.Html.ToString(), "some text"));
        }
    }
    [TestFixture]
    public class RenderHtmlBehaviorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            request = new InMemoryFubuRequest();
            writer = MockRepository.GenerateMock<IOutputWriter>();
            
            request.Set("some html");

            var behavior = new RenderHtmlBehavior(writer, request);
            behavior.Invoke();
        }

        #endregion

        private InMemoryFubuRequest request;
        private IOutputWriter writer;

        [Test]
        public void should_write_the_to_string_render_of_the_type_to_the_output_writer()
        {
            writer.AssertWasCalled(x => x.Write(MimeType.Html.ToString(), "some html"));
        }
    }



    public class ReportController
    {
        public Report BuildReport()
        {
            return new Report();
        }
    }

    public class Report
    {
        public override string ToString()
        {
            return "Got a Report";
        }
    }
}