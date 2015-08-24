using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.Resources.Conneg
{
    public class WriterChoice : LogRecord, DescribesItself
    {
        private readonly string _mimeType;
        private readonly Description _writer;

        public WriterChoice(string mimeType, object writer)
        {
            _mimeType = mimeType;
            _writer = Description.For(writer);
        }

        public void Describe(Description description)
        {
            description.Title = "Selected writer '{0}'".ToFormat(_writer.Title);

            if (_writer.HasExplicitShortDescription())
            {
                description.Properties["Writer"] = _writer.ShortDescription;
            }
            
            description.Properties["Mimetype"] = _mimeType;
        }
    }
}