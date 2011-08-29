namespace FubuMVC.Core.Assets.Content
{
    public interface IContentPipeline
    {
        string ReadContentsFrom(string file);
        ITransformer GetTransformer<T>() where T : ITransformer;

    }
}