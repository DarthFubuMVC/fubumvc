namespace FubuMVC.Core.Runtime.Conditionals
{
    public interface IConditional
    {
        bool ShouldExecute(IFubuRequestContext context);
    }
}