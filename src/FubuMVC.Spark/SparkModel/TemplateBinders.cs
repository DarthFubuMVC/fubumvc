using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Registration;

namespace FubuMVC.Spark.SparkModel
{

    public interface IBindRequest
    {
        ITemplate Target { get; }
        Parsing Parsing { get; }

        TypePool Types { get; }
        ITemplateRegistry TemplateRegistry { get; }
        ISparkLogger Logger { get; }
    }

    public class BindRequest : IBindRequest
    {
        public ITemplate Target { get; set; }
        public Parsing Parsing { get; set; }
        public TypePool Types { get; set; }

        public ITemplateRegistry TemplateRegistry { get; set; }
        public ISparkLogger Logger { get; set; }
    }

    public interface ITemplateBinder
    {
        bool CanBind(IBindRequest request);
        void Bind(IBindRequest request);
    }

    public class ViewDescriptorBinder : ITemplateBinder
    {
        public bool CanBind(IBindRequest request)
        {
            var template = request.Target;
            return !(template.Descriptor is ViewDescriptor) && template.IsSparkView();
        }

        public void Bind(IBindRequest request)
        {
            request.Target.Descriptor = new ViewDescriptor(request.Target);
        }
    }

    public class GenericViewModelBinder : ITemplateBinder
    {
        public bool CanBind(IBindRequest request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;
            var parsing = request.Parsing;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && !request.Target.IsPartial()
                   && parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(parsing.ViewModelType);
        }

        public void Bind(IBindRequest request)
        {
            var logger = request.Logger;
            var template = request.Target;

            var genericParser = new GenericParser(request.Types.Assemblies);
            var viewModel = genericParser.Parse(request.Parsing.ViewModelType);

            if (viewModel != null)
            {
                var descriptor = template.Descriptor.As<ViewDescriptor>();
                descriptor.ViewModel = viewModel;
                logger.Log(template, "Generic view model type is : {0}", descriptor.ViewModel);
                return;
            }

            genericParser.ParseErrors.Each(error => logger.Log(template, error));
        }
    }

    public class ViewModelBinder : ITemplateBinder
    {
        public bool CanBind(IBindRequest request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;
            var parsing = request.Parsing;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && !request.Target.IsPartial()
                   && parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(parsing.ViewModelType) == false;
        }

        public void Bind(IBindRequest request)
        {
            var logger = request.Logger;
            var template = request.Target;
            var descriptor = template.Descriptor.As<ViewDescriptor>();

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