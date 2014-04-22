using FubuCore;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DownloadFileConvention : Policy
    {
        public DownloadFileConvention()
        {
            Where.ResourceTypeImplements<DownloadFileModel>();
            Add.NodeToEnd<DownloadFileNode>();
        }
    }
}