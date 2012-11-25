using System.Web;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using HtmlTags;
using FubuCore;

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

    public class FileUploadController
    {
        private readonly IUrlRegistry _urls;

        public FileUploadController(IUrlRegistry urls)
        {
            _urls = urls;
        }

        private HtmlDocument buildDocument(FileUploadOutput model)
        {
            var document = new HtmlDocument();
            document.Title = "File Upload View";

            document.Add("h1").Text(model.Text);

            document.Add("form")
                .Attr("method", "post")
                .Attr("enctype", "multipart/form-data")
                .Attr("action", _urls.UrlFor<FileUploadInput>());

            document.Add("br");

            document.Push("p");
            document.Add("span").Text("File 1:  ");
            document.Add("input").Attr("type", "file").Attr("name", "File1");

            
            document.Add("br");
            document.Add("input").Attr("type", "submit");

            return document;
        }

        public HtmlDocument get_file_upload_test(FileUploadTestInput input)
        {

            return buildDocument(new FileUploadOutput(){
                Text = "Please upload a file"
            });
        }

        public HtmlDocument post_file_upload_test(FileUploadInput input)
        {
            var text = (input.File1 == null) ? "File upload failure!" : "File upload success! " + input.File1.FileName;

            return buildDocument(new FileUploadOutput
            {
                Text = text
            });
        }
    
    }
}