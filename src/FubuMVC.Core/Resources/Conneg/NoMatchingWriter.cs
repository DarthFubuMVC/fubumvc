using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Resources.Conneg
{
    public class NoMatchingWriter : LogRecord, DescribesItself
    {
        private readonly CurrentMimeType _mimeType;

        public NoMatchingWriter(CurrentMimeType mimeType)
        {
            _mimeType = mimeType;
        }

        public void Describe(Description description)
        {
            description.Title = "No writers matched the runtime conditions and accept-type: " + _mimeType.AcceptTypes;
        }
    }
}