namespace FubuMVC.Core.Diagnostics
{
    public interface IBehaviorDetails
    {
        void AcceptVisitor(IBehaviorDetailsVisitor visitor);
    }
}