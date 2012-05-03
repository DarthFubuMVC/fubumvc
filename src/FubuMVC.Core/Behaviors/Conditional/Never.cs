namespace FubuMVC.Core.Behaviors.Conditional
{
    public class Never : IConditional
    {
        public bool ShouldExecute()
        {
            return false;
        }
    }
}