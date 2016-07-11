using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Downloads
{
    public class DownloadFileNode : Process
    {
        public DownloadFileNode() : base(typeof(DownloadFileBehavior))
        {
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Output; }
        }
    }
}