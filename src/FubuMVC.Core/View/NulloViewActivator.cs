namespace FubuMVC.Core.View
{
    public class NulloViewActivator : IViewActivator
    {
        public void Activate<T>(IFubuPage<T> page) where T : class
        {
            
        }

        public void Activate(IFubuPage page)
        {
            
        }
    }
}