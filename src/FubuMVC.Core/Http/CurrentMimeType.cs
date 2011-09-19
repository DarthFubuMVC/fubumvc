namespace FubuMVC.Core.Http
{
    public class CurrentMimeType
    {
        // TODO -- really, really have to make sure this works in model binding
        // cause it doesn't right now
        // May need a custom model binder for this.

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