namespace FubuMVC.Core.Http
{
    public class CurrentMimeType
    {
        public CurrentMimeType()
        {
        }

        public CurrentMimeType(string contentType, string acceptType)
        {
            ContentType = new MimeTypeList(contentType);
            AcceptTypes = new MimeTypeList(acceptType);
        }

        public MimeTypeList ContentType { get; set; }
        public MimeTypeList AcceptTypes { get; set; }
    }
}