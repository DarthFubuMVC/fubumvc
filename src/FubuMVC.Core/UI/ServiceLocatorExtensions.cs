using System;
using FubuCore;
using FubuMVC.Core.UI.Tags;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.UI
{
    public static class ServiceLocatorExtensions
    {
        public static ITagGenerator TagsFor(this IServiceLocator services, object model)
        {
            if (model == null) throw new ArgumentNullException("model");

            var modelType = services.GetInstance<ITypeResolver>().ResolveType(model);
            var tagsType = typeof (ITagGenerator<>).MakeGenericType(modelType);
            return (ITagGenerator) services.GetInstance(tagsType);
        }
    }
}