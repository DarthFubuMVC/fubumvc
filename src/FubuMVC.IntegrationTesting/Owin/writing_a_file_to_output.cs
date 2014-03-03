using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class writing_a_file_to_output
    {
        [Test]
        public void can_write_the_contents_of_a_file_to_the_output()
        {
            HarnessApplication.Run(x => {
                var response = x.GetByInput(new FileInput
                {
                    Name = "Test.txt"
                });

                response.ContentTypeShouldBe(MimeType.Text);
                response.ReadAsText().ShouldContain("Some text here");
            });


        }
    }

    public class FileWriterEndpoint
    {
        private readonly IOutputWriter _writer;
        private readonly IFubuApplicationFiles _files;

        public FileWriterEndpoint(IOutputWriter writer, IFubuApplicationFiles files)
        {
            _writer = writer;
            _files = files;
        }

        public void get_file_contents_Name(FileInput input)
        {
            var file = _files.Find(input.Name);
            _writer.WriteFile(MimeType.Text, file.Path, input.Name);
        }
    }    

    public class FileInput
    {
        public string Name { get; set;}
    }
}