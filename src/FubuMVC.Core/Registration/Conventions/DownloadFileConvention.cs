using FubuCore;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DownloadFileConvention : ActionCallModification
    {
        public DownloadFileConvention()
            : base(call => call.AddToEnd(new DownloadFileNode()), "Adding download file behavior as the output node")
        {
            Filters.Includes.Add(call => call.OutputType().CanBeCastTo<DownloadFileModel>());
        }
    }
}