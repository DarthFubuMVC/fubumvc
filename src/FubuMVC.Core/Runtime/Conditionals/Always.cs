using FubuCore.Descriptions;

namespace FubuMVC.Core.Runtime.Conditionals
{
    [Title("Always")]
    public class Always : IConditional
    {
        private Always()
        {
        }

        public bool ShouldExecute(IFubuRequestContext context)
        {
            return true;
        }

        public static readonly Always Flyweight = new Always();
    }
}