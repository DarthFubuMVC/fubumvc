using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Visualization
{
    public class BehaviorNodeViewModel
    {
        public BehaviorNode Node { get; set; }
        public Description Description { get; set; }
        public string VisualizationHtml { get; set; }
    }
}