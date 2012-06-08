using System.Web;
using FubuMVC.WebForms;

namespace AspNetApplication.FileUpload
{
    public class FileUploadTestInput{ }

    public class FileUploadInput
    {
        public HttpPostedFileBase File1 { get; set; }
    }

    public class FileUploadOutput
    {
        public string Text { get; set; }
    }

    public class FileUploadView : FubuPage<FileUploadOutput>{}

    public class FileUploadController
    {
        public FileUploadOutput get_file_upload_test(FileUploadTestInput input)
        {
            return new FileUploadOutput
            {
                Text = "Please upload a file"
            };
        }

        public FileUploadOutput post_file_upload_test(FileUploadInput input)
        {
            var text = (input.File1 == null) ? "File upload failure!" : "File upload success! " + input.File1.FileName;

            return new FileUploadOutput
            {
                Text = text
            };
        }
    
    }
}