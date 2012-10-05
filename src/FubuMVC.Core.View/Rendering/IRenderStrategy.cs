namespace FubuMVC.Core.View.Rendering
{
    public interface IRenderStrategy
    {
        bool Applies();
        void Invoke(IRenderAction action);
    }
}