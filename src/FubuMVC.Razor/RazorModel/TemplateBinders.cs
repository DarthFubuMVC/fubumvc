using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Registration;

namespace FubuMVC.Razor.RazorModel
{
    public class ViewDescriptorBinder : ITemplateBinder<IRazorTemplate>
    {
        public bool CanBind(IBindRequest<IRazorTemplate> request)
        {
            var template = request.Target;
            return !(template.Descriptor is ViewDescriptor<IRazorTemplate>) && template.IsRazorView();
        }

        public void Bind(IBindRequest<IRazorTemplate> request)
        {
            request.Target.Descriptor = new ViewDescriptor<IRazorTemplate>(request.Target);
        }
    }

    public class GenericViewModelBinder : ITemplateBinder<IRazorTemplate>
    {
        public bool CanBind(IBindRequest<IRazorTemplate> request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor<IRazorTemplate>;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && request.Parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(request.Parsing.ViewModelType);
        }

        public void Bind(IBindRequest<IRazorTemplate> request)
        {
            var logger = request.Logger;
            var template = request.Target;

            var genericParser = new GenericParser(request.Types.Assemblies);
            var viewModel = genericParser.Parse(request.Parsing.ViewModelType);

            if (viewModel != null)
            {
                var descriptor = template.Descriptor.As<ViewDescriptor<IRazorTemplate>>();
                descriptor.ViewModel = viewModel;
                logger.Log(template, "Generic view model type is : {0}", descriptor.ViewModel);
                return;
            }

            genericParser.ParseErrors.Each(error => logger.Log(template, error));
        }
    }

    public class ViewModelBinder : ITemplateBinder<IRazorTemplate>
    {
        public bool CanBind(IBindRequest<IRazorTemplate> request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor<IRazorTemplate>;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && request.Parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(request.Parsing.ViewModelType) == false;
        }

        public void Bind(IBindRequest<IRazorTemplate> request)
        {
            var logger = request.Logger;
            var template = request.Target;
            var descriptor = template.Descriptor.As<ViewDescriptor<IRazorTemplate>>();

            var types = request.Types.TypesWithFullName(request.Parsing.ViewModelType);
            var typeCount = types.Count();

            if (typeCount == 1)
            {
                descriptor.ViewModel = types.First();
                logger.Log(template, "View model type is : [{0}]", descriptor.ViewModel);

                return;
            }

            logger.Log(template, "Unable to set view model type : {0}", request.Parsing.ViewModelType);

            if (typeCount > 1)
            {
                var candidates = types.Select(x => x.AssemblyQualifiedName).Join(", ");
                logger.Log(template, "Type ambiguity on: {0}", candidates);
            }
        }
    }
}
