using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;

namespace AspNetApplication
{
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