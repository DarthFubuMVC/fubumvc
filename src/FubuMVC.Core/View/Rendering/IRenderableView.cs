namespace FubuMVC.Core.View.Rendering
{
    public interface IRenderableView //: IFubuPage
    {
        void Render(IFubuRequestContext context);

        IFubuPage Page { get; } 
    }
}