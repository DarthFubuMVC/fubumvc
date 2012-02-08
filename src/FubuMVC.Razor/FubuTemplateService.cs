using System;
using System.Collections.Generic;
using FubuMVC.Core.View;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Rendering;
using RazorEngine.Templating;
using RazorEngine.Text;
using ITemplate = RazorEngine.Templating.ITemplate;

namespace FubuMVC.Razor
{
    public interface IFubuTemplateService : ITemplateService
    {
        IFubuRazorView GetView(IRazorDescriptor descriptor);
    }

    public class FubuTemplateService : IFubuTemplateService
    {
        private readonly TemplateRegistry _templateRegistry;
        private readonly ITemplateService _inner;

        public FubuTemplateService(TemplateRegistry templateRegistry, ITemplateService inner)
        {
            _templateRegistry = templateRegistry;
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public void AddNamespace(string ns)
        {
            _inner.AddNamespace(ns);
        }

        public void Compile(string razorTemplate, string name)
        {
            _inner.Compile(razorTemplate, name);
        }

        public void Compile(string razorTemplate, Type modelType, string name)
        {
            _inner.Compile(razorTemplate, modelType, name);
        }

        public void Compile<T>(string razorTemplate, string name)
        {
            _inner.Compile<T>(razorTemplate, name);
        }

        public ITemplate CreateTemplate(string razorTemplate)
        {
            return _inner.CreateTemplate(razorTemplate);
        }

        public ITemplate CreateTemplate<T>(string razorTemplate, T model)
        {
            return _inner.CreateTemplate(razorTemplate, model);
        }

        public IEnumerable<ITemplate> CreateTemplates(IEnumerable<string> razorTemplates, bool parallel = false)
        {
            return _inner.CreateTemplates(razorTemplates, parallel);
        }

        public IEnumerable<ITemplate> CreateTemplates<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, bool parallel = false)
        {
            return _inner.CreateTemplates(razorTemplates, models, parallel);
        }

        public Type CreateTemplateType(string razorTemplate)
        {
            return _inner.CreateTemplateType(razorTemplate);
        }

        public Type CreateTemplateType(string razorTemplate, Type modelType)
        {
            return _inner.CreateTemplateType(razorTemplate, modelType);
        }

        public IEnumerable<Type> CreateTemplateTypes(IEnumerable<string> razorTemplates, bool parallel = false)
        {
            return _inner.CreateTemplateTypes(razorTemplates, parallel);
        }

        public IEnumerable<Type> CreateTemplateTypes(IEnumerable<string> razorTemplates, Type modelType, bool parallel = false)
        {
            return _inner.CreateTemplateTypes(razorTemplates, modelType, parallel);
        }

        public ITemplate GetTemplate(string razorTemplate, string name)
        {
            var template = _inner.GetTemplate(razorTemplate, name);
            template.TemplateService = this;
            return template;
        }

        public ITemplate GetTemplate<T>(string razorTemplate, T model, string name)
        {
            var template = _inner.GetTemplate<T>(razorTemplate, model, name);
            template.TemplateService = this;
            return template;
        }

        public IEnumerable<ITemplate> GetTemplates(IEnumerable<string> razorTemplates, IEnumerable<string> names, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITemplate> GetTemplates<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, IEnumerable<string> names, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        public bool HasTemplate(string name)
        {
            return _inner.HasTemplate(name);
        }

        public string Parse(string razorTemplate)
        {
            throw new NotImplementedException();
        }

        public string Parse(string razorTemplate, string name)
        {
            throw new NotImplementedException();
        }

        public string Parse(string razorTemplate, object model)
        {
            throw new NotImplementedException();
        }

        public string Parse<T>(string razorTemplate, T model)
        {
            throw new NotImplementedException();
        }

        public string Parse<T>(string razorTemplate, T model, string name)
        {
            throw new NotImplementedException();
        }

        public string Parse(string razorTemplate, object model, string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseMany(IEnumerable<string> razorTemplates, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseMany(IEnumerable<string> razorTemplates, IEnumerable<string> names, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseMany<T>(string razorTemplate, IEnumerable<T> models, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseMany<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseMany<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, IEnumerable<string> names, bool parallel = false)
        {
            throw new NotImplementedException();
        }

        public ITemplate Resolve(string name)
        {
            var fubuTemplate = _templateRegistry.FirstByName(name);
            return GetView(fubuTemplate.Descriptor);
        }

        public ITemplate Resolve(string name, object model)
        {
           throw new NotImplementedException();
        }

        public ITemplate Resolve<T>(string name, T model)
        {
            var fubuTemplate = _templateRegistry.FirstByName(name);
            var view = GetView(fubuTemplate.Descriptor);
            var template = _inner.Resolve(fubuTemplate.GeneratedViewId.ToString(), model);
            template.TemplateService = this;
            return template;
        }

        public string Run(string name)
        {
            throw new NotImplementedException();
        }

        public string Run(string name, object model)
        {
            throw new NotImplementedException();
        }

        public string Run<T>(string name, T model)
        {
            throw new NotImplementedException();
        }

        public IEncodedStringFactory EncodedStringFactory
        {
            get { return _inner.EncodedStringFactory; }
        }

        public IFubuRazorView GetView(IRazorDescriptor descriptor)
        {
            var viewId = descriptor.Template.GeneratedViewId.ToString();

            if (_inner.HasTemplate(viewId) && descriptor.IsCurrent())
            {
                return GetView(x => x.Resolve(viewId));
            }
            return GetView(x => x.GetTemplate(descriptor.ViewFile.GetSourceCode(), viewId));
        }

        public IFubuRazorView GetView(IRazorDescriptor descriptor, object model)
        {
            var viewId = descriptor.Template.GeneratedViewId.ToString();

            if (_inner.HasTemplate(viewId) && descriptor.IsCurrent())
            {
                return GetView(x => x.Resolve(viewId, model));
            }
            return GetView(x =>
            {
                x.GetTemplate(descriptor.ViewFile.GetSourceCode(), viewId);
                return x.Resolve(viewId, model);
            });
        }

        private IFubuRazorView GetView(Func<ITemplateService, ITemplate> templateAction)
        {
            var template = templateAction(_inner);
            template.TemplateService = this;
            return (IFubuRazorView)template;
        }
    }

    public interface ITemplateServiceWrapper
    {
        IFubuTemplateService TemplateService { get; }
    }

    public class TemplateServiceWrapper : ITemplateServiceWrapper
    {
        private readonly IFubuTemplateService _templateService;

        public TemplateServiceWrapper(IFubuTemplateService templateService)
        {
            _templateService = templateService;
        }

        public IFubuTemplateService TemplateService
        {
            get { return _templateService; }
        }
    }
}