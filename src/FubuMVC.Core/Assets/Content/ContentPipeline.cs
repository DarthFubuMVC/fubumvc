using FubuCore;


namespace FubuMVC.Core.Assets.Content
{
    public class ContentPipeline : IContentPipeline
    {
        private readonly IServiceLocator _services;
        private readonly IFileSystem _fileSystem;

        public ContentPipeline(IServiceLocator services, IFileSystem fileSystem)
        {
            _services = services;
            _fileSystem = fileSystem;
        }

        public string ReadContentsFrom(string file)
        {
            return _fileSystem.ReadStringFromFile(file);
        }

        public ITransformer GetTransformer<T>() where T : ITransformer
        {
            return _services.GetInstance<T>();
        }
    }
}