using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core
{
    public class DeactivatorExecuted : LogRecord, DescribesItself
    {
        public string Deactivator { get; set; }
        public IActivationLog Log { get; set; }

        public void Describe(Description description)
        {
            description.Title = "Deactivator: " + Deactivator;
            description.LongDescription = Log.FullTraceText();
        }
    }
}