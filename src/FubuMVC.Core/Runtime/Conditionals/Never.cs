namespace FubuMVC.Core.Runtime.Conditionals
{
    public class Never : IConditional
    {
        public bool ShouldExecute()
        {
            return false;
        }
    }
}