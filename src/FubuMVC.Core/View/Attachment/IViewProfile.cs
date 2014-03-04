using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.View.Attachment
{
    public interface IViewProfile
    {
        IConditional Condition { get; }
        ViewBag Filter(ViewBag bag);
    }
}