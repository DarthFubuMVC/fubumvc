namespace FubuMVC.Core.Runtime.Conditionals
{
    // TODO -- I think this should be in Runtime
    public class Always : IConditional
    {
        private Always()
        {
        }

        public bool ShouldExecute()
        {
            return true;
        }

        public static readonly Always Flyweight = new Always();
    }
}