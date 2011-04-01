using FubuCore;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class DownloadFileConvention : ActionCallModification
    {
        public DownloadFileConvention()
            : base(call => call.AddToEnd(new OutputNode(typeof (DownloadFileBehavior))), "Adding download file behavior as the output node")
        {
            Filters.Excludes.Add(call => call.HasAnyOutputBehavior());
            Filters.Includes.Add(call => call.OutputType().CanBeCastTo<DownloadFileModel>());
        }
    }
}