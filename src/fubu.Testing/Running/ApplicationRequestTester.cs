using Fubu.Running;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Running
{
    [TestFixture]
    public class ApplicationRequestTester
    {
        [Test]
        public void do_not_run_the_application_if_templates()
        {
            var request = new ApplicationRequest();

            request.TemplatesFlag = true;

            request.ShouldRunApp().ShouldBeFalse();

            request.TemplatesFlag = false;

            request.ShouldRunApp().ShouldBeTrue();
        }

        [Test]
        public void do_run_the_application_if_neither_templates_or_explode_is_set()
        {
            var request = new ApplicationRequest {TemplatesFlag = false};

            request.ShouldRunApp().ShouldBeTrue();
        }
    }
}