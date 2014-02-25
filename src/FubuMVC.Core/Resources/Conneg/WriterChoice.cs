using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Resources.Conneg
{
    public class WriterChoice : LogRecord, DescribesItself
    {
        private readonly string _mimeType;
        private readonly Description _writer;
        private readonly Description _condition;

        public WriterChoice(string mimeType, object writer, IConditional condition)
        {
            _mimeType = mimeType;
            _writer = Description.For(writer);
            _condition = Description.For(condition);
        }

        public void Describe(Description description)
        {
            description.Title = "Selected writer '{0}'".ToFormat(_writer.Title);

            if (_writer.HasExplicitShortDescription())
            {
                description.Properties["Writer"] = _writer.ShortDescription;
            }
            
            description.Properties["Mimetype"] = _mimeType;
            description.Properties["Condition"] = _condition.Title;
        }
    }
}