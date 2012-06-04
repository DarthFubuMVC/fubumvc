using FubuCore;

namespace FubuMVC.Diagnostics.Runtime
{
    public interface IBehaviorDetails
    {
		// TODO -- Remove this in favor of partials in advanced diagnostics
		[MarkedForTermination]
        void AcceptVisitor(IBehaviorDetailsVisitor visitor);
    }
}