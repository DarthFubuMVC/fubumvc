using System.IO;
using FubuCore;

namespace Fubu.Templating.Steps
{
    public class RemoveTemporaryContent : ITemplateStep
    {
        public string Describe(TemplatePlanContext context)
        {
            return "Remove temporary directory {0}".ToFormat(context.TempDir);
        }

        public void Execute(TemplatePlanContext context)
        {
            new DirectoryInfo(context.TempDir).SafeDelete();
        }
    }
}