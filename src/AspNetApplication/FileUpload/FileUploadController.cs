using System.Web;
using FubuMVC.Core.UI;
using HtmlTags;

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
        private readonly FubuHtmlDocument<FileUploadOutput> _document;

        public FileUploadController(FubuHtmlDocument<FileUploadOutput> document)
        {
            _document = document;
        }

        private HtmlDocument buildDocument(FileUploadOutput model)
        {
            _document.Model = model;

            _document.Title = "File Upload View";
            _document.Add("h1").Text(model.Text);

            _document.Add("form")
                .Attr("method", "post")
                .Attr("enctype", "multipart/form-data")
                .Attr("action", _document.Urls.UrlFor<FileUploadInput>(null));

            _document.Add("br");

            _document.Push("p");
            _document.Add("span").Text("File 1:  ");
            _document.Add("input").Attr("type", "file").Attr("name", "File1");

            
            _document.Add("br");
            _document.Add("input").Attr("type", "submit");

            return _document;
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