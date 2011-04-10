using FubuCore.Util;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
    public class DebugReportTagWriter
    {
        private readonly Cache<BehaviorReport, HtmlTag> _behaviorTags = new Cache<BehaviorReport, HtmlTag>();
        private readonly HtmlTag _div = new HtmlTag("div");
        private HtmlTag _lastTag;

        public DebugReportTagWriter()
        {
            _lastTag = _div;

            _behaviorTags.OnMissing = b =>
            {
                HtmlTag tag = BuildBehaviorTag(b);
                _lastTag.Append(tag);
                _lastTag = tag;

                return tag;
            };
        }

        public HtmlTag Tag { get { return _div; } }

        public static HtmlTag BuildBehaviorTag(BehaviorReport report)
        {
            return new HtmlTag("div", x =>
            {
                x.AddClass("behavior");
                x.Add("h4").Text(report.BehaviorType.FullName);
                x.Add("p").Text("Execution Time:  " + report.ExecutionTime + " milliseconds");
                x.Add("hr");
            });
        }

        public void WriteStep(BehaviorStep step)
        {
            new DetailsTagWriter(_behaviorTags[step.Behavior]).Write(step.Details);
        }
    }
}