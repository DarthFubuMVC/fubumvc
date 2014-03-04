using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Registration;

namespace FubuMVC.Core.View.Model
{
    public interface IBindRequest<T> where T : ITemplateFile
    {
        T Target { get; }
        Parsing Parsing { get; }

        TypePool Types { get; }
        ITemplateLogger Logger { get; }
    }

    public class BindRequest<T> : IBindRequest<T> where T : ITemplateFile
    {
        public T Target { get; set; }
        public Parsing Parsing { get; set; }
        public TypePool Types { get; set; }

        public ITemplateLogger Logger { get; set; }
    }

    public interface ITemplateBinder<T> where T : ITemplateFile
    {
        bool CanBind(IBindRequest<T> request);
        void Bind(IBindRequest<T> request);
    }

    public class ViewDescriptorBinder<T> : ITemplateBinder<T> where T : ITemplateFile
    {
        private readonly ITemplateSelector<T> _templateSelector;

        public ViewDescriptorBinder(ITemplateSelector<T> templateSelector)
        {
            _templateSelector = templateSelector;
        }

        public bool CanBind(IBindRequest<T> request)
        {
            var template = request.Target;
            return !(template.Descriptor is ViewDescriptor<T>) && _templateSelector.IsAppropriate(template);
        }

        public void Bind(IBindRequest<T> request)
        {
            request.Target.Descriptor = new ViewDescriptor<T>(request.Target);
        }
    }

    public class GenericViewModelBinder<T> : ITemplateBinder<T> where T : ITemplateFile
    {
        public bool CanBind(IBindRequest<T> request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor<T>;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && request.Parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(request.Parsing.ViewModelType);
        }

        public void Bind(IBindRequest<T> request)
        {
            var logger = request.Logger;
            var template = request.Target;

            var genericParser = new GenericParser(request.Types.Assemblies);
            var viewModel = genericParser.Parse(request.Parsing.ViewModelType);

            if (viewModel != null)
            {
                var descriptor = template.Descriptor.As<ViewDescriptor<T>>();
                descriptor.ViewModel = viewModel;
                logger.Log(template, "Generic view model type is : {0}", descriptor.ViewModel);
                return;
            }

            genericParser.ParseErrors.Each(error => logger.Log(template, error));
        }
    }

    public class ViewModelBinder<T> : ITemplateBinder<T> where T : ITemplateFile
    {
        public bool CanBind(IBindRequest<T> request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor<T>;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && request.Parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(request.Parsing.ViewModelType) == false;
        }

        public void Bind(IBindRequest<T> request)
        {
            var logger = request.Logger;
            var template = request.Target;
            var descriptor = template.Descriptor.As<ViewDescriptor<T>>();

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