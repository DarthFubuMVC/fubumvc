using FubuCore.Descriptions;

namespace FubuMVC.Core.Runtime.Conditionals
{
    [Title("Never")]
    public class Never : IConditional
    {
        public bool ShouldExecute(IFubuRequestContext context)
        {
            return false;
        }
    }
}