using FubuCore;

namespace FubuMVC.Core.Diagnostics
{
    public interface IBehaviorDetails
    {
		// TODO -- Remove this in favor of partials in advanced diagnostics
		[MarkedForTermination]
        void AcceptVisitor(IBehaviorDetailsVisitor visitor);
    }
}