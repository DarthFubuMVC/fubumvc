namespace FubuMVC.Core.View.Rendering
{
    public interface IRenderableView //: IFubuPage
    {
        void Render();

        IFubuPage Page { get; } 
    }
}