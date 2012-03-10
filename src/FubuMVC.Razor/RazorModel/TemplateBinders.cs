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
            return !(template.Descriptor is ViewDescriptor) && template.IsRazorView();
        }

        public void Bind(IBindRequest<IRazorTemplate> request)
        {
            request.Target.Descriptor = new ViewDescriptor(request.Target);
        }
    }

    //public class ViewLoaderBinder : ITemplateBinder<IRazorTemplate>
    //{
    //    public bool CanBind(IBindRequest<IRazorTemplate> request)
    //    {
    //        var descriptor = request.Target.Descriptor as ViewDescriptor;
    //        return descriptor != null
    //               && descriptor.ViewFile == null;
    //    }

    //    public void Bind(IBindRequest<IRazorTemplate> request)
    //    {
    //        var descriptor = request.Target.Descriptor as ViewDescriptor;
    //        descriptor.ViewFile = request.ViewFile;
    //    }
    //}

    public class MasterPageBinder : ITemplateBinder<IRazorTemplate>
    {
        private readonly ISharedTemplateLocator _sharedTemplateLocator;
        private const string FallbackMaster = "_Layout";
        public string MasterName { get; set; }

        public MasterPageBinder()
        {
        }
        public MasterPageBinder(ISharedTemplateLocator sharedTemplateLocator)
        {
            _sharedTemplateLocator = sharedTemplateLocator;
            MasterName = FallbackMaster;
        }

        public bool CanBind(IBindRequest<IRazorTemplate> request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;

            return descriptor != null
                && descriptor.Master == null
                && (request.Parsing.ViewModelType.IsNotEmpty() || request.Parsing.Master.IsNotEmpty())
                && request.Parsing.Master != string.Empty;
        }

        public void Bind(IBindRequest<IRazorTemplate> request)
        {
            var template = request.Target;
            var tracer = request.Logger;
            var masterName = request.Parsing.Master ?? MasterName;

            var master = _sharedTemplateLocator.LocateMaster(masterName, template);

            if (master == null)
            {
                tracer.Log(template, "Expected master page [{0}] not found.", masterName);
                return;
            }

            template.Descriptor.As<ViewDescriptor>().Master = master;
            tracer.Log(template, "Master page [{0}] found at {1}", masterName, master.FilePath);
        }
    }

    public class GenericViewModelBinder : ITemplateBinder<IRazorTemplate>
    {
        public bool CanBind(IBindRequest<IRazorTemplate> request)
        {
            var descriptor = request.Target.Descriptor as ViewDescriptor;

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
                var descriptor = template.Descriptor.As<ViewDescriptor>();
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
            var descriptor = request.Target.Descriptor as ViewDescriptor;

            return descriptor != null
                   && !descriptor.HasViewModel()
                   && request.Parsing.ViewModelType.IsNotEmpty()
                   && GenericParser.IsGeneric(request.Parsing.ViewModelType) == false;
        }

        public void Bind(IBindRequest<IRazorTemplate> request)
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
