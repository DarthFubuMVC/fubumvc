using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.Resources.Conneg
{
    public class ReaderChoice : LogRecord, DescribesItself
    {
        private readonly string _mimeType;
        private readonly Description _reader;

        public ReaderChoice(string mimeType, object reader)
        {
            _mimeType = mimeType;
            if (reader != null) _reader = Description.For(reader);
        }

        public void Describe(Description description)
        {
            description.Title = _reader == null
                ? "Unable to select a reader for content-type '{0}'".ToFormat(_mimeType)
                : "Selected reader '{0}' for content-type '{1}'".ToFormat(_reader.Title, _mimeType);
        }
    }
}