using System;
using FubuCore;
using FubuMVC.Core.UI.Tags;


namespace FubuMVC.Diagnostics.Features.Html.Preview.Decorators
{
    public interface ITagGeneratorFactory
    {
        ITagGenerator GeneratorFor(Type modelType);
    }

    public class TagGeneratorFactory : ITagGeneratorFactory
    {
        private readonly IServiceLocator _services;

        public TagGeneratorFactory(IServiceLocator services)
        {
            _services = services;
        }

        public ITagGenerator GeneratorFor(Type modelType)
        {
            var tagGeneratorType = typeof(TagGenerator<>).MakeGenericType(modelType);
            return (ITagGenerator)_services.GetInstance(tagGeneratorType);
        }
    }
}