using FubuCore;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class DownloadFileConvention : Policy
    {
        public DownloadFileConvention()
        {
            Where.ResourceTypeImplements<DownloadFileModel>();
            Add.NodeToEnd<DownloadFileNode>();
        }
    }
}