using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.PathBased;

namespace FubuMVC.Core.Runtime
{
    public class ModelBindingServicesRegistry : ServiceRegistry
    {
        public ModelBindingServicesRegistry()
        {
            AddService<IModelBinder, FubuTupleBinder>();
            AddService<IModelBinder>(new CurrentMimeTypeModelBinder());
            AddService<IModelBinder, ResourcePathBinder>();
            SetServiceIfNone<ISetterBinder, SetterBinder>();

            SetServiceIfNone<IObjectResolver, ObjectResolver>();

            // STrictly a standin for testing purposes
            AddService<IRequestData>(new RequestData());
            SetServiceIfNone<IBindingContext, BindingContext>();

            AddService<IConverterFamily, AspNetObjectConversionFamily>();
            SetServiceIfNone<IBindingLogger, NulloBindingLogger>();

            AddService<IConverterFamily, AspNetPassthroughConverter>();

            AddService<IPropertyBinder, CurrentRequestFullUrlPropertyBinder>();
            AddService<IPropertyBinder, CurrentRequestRelativeUrlPropertyBinder>();
        }
    }
}