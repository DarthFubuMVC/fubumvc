using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Registration;

namespace FubuMVC.Spark.SparkModel
{
    public class ViewDescriptorBinder : ITemplateBinder<ITemplate>
    {
        public bool CanBind(IBindRequest<ITemplate> request)
        {
            var template = request.Target;
            return !(template.Descriptor is SparkDescriptor) && template.IsSparkView();
        }

        public void Bind(IBindRequest<ITemplate> request)
        {
            request.Target.Descriptor = new SparkDescriptor(request.Target);
        }
    }

    public class GenericViewModelBinder : ITemplateBinder<ITemplate>
    {
        public bool CanBind(IBindRequest<ITemplate> request)
        {
            var descriptor = request.Target.Descriptor as SparkDescriptor;
            var parsing = request.Parsing;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && !request.Target.IsPartial()
                   && parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(parsing.ViewModelType);
        }

        public void Bind(IBindRequest<ITemplate> request)
        {
            var logger = request.Logger;
            var template = request.Target;

            var genericParser = new GenericParser(request.Types.Assemblies);
            var viewModel = genericParser.Parse(request.Parsing.ViewModelType);

            if (viewModel != null)
            {
                var descriptor = template.Descriptor.As<SparkDescriptor>();
                descriptor.ViewModel = viewModel;
                logger.Log(template, "Generic view model type is : {0}", descriptor.ViewModel);
                return;
            }

            genericParser.ParseErrors.Each(error => logger.Log(template, error));
        }
    }

    public class ViewModelBinder : ITemplateBinder<ITemplate>
    {
        public bool CanBind(IBindRequest<ITemplate> request)
        {
            var descriptor = request.Target.Descriptor as SparkDescriptor;
            var parsing = request.Parsing;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && !request.Target.IsPartial()
                   && parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(parsing.ViewModelType) == false;
        }

        public void Bind(IBindRequest<ITemplate> request)
        {
            var logger = request.Logger;
            var template = request.Target;
            var descriptor = template.Descriptor.As<SparkDescriptor>();

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