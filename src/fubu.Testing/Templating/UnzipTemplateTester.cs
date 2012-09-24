using System;
using System.IO;
using Bottles.Exploding;
using Bottles.Zipping;
using Fubu;
using Fubu.Templating;
using Fubu.Templating.Steps;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace fubu.Testing.Templating
{
    [TestFixture]
    public class UnzipTemplateTester : InteractionContext<UnzipTemplate>
    {
        private NewCommandInput _input;
        private TemplatePlanContext _context;

        protected override void beforeEach()
        {
            _input = new NewCommandInput();
            _context = new TemplatePlanContext
                           {
                               TargetPath = "Test",
                               Input = _input
                           };
        }

        [Test]
        public void should_unzip_from_default_template_if_none_is_specified()
        {
            MockFor<IZipFileService>()
                .Expect(s => s.ExtractTo(UnzipTemplate.TemplateZip,
                                         _context.TempDir, ExplodeOptions.PreserveDestination));

            ClassUnderTest.Execute(_context);

            VerifyCallsFor<IZipFileService>();
        }

        [Test]
        public void should_unzip_from_specified_file()
        {
            _input.ZipFlag = "test.zip";
            var input = Path.Combine(Environment.CurrentDirectory, _input.ZipFlag);
            MockFor<IZipFileService>()
                .Expect(s => s.ExtractTo(input, _context.TempDir, ExplodeOptions.PreserveDestination));

            ClassUnderTest.Execute(_context);

            VerifyCallsFor<IZipFileService>();
        }
    }
}