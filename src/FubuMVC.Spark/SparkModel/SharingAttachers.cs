using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class BindingsAttacher : ISharingAttacher<ITemplate>
    {
        private readonly ISharedTemplateLocator _templateLocator;

        private const string FallbackBindingsName = "bindings";
        public string BindingsName { get; set; }

        public BindingsAttacher(ISharedTemplateLocator templateLocator)
        {
            _templateLocator = templateLocator;
            BindingsName = FallbackBindingsName;
        }

        public bool CanAttach(IAttachRequest<ITemplate> request)
        {
            var descriptor = request.Template.Descriptor as SparkDescriptor;
            
            return descriptor != null 
                && descriptor.Bindings.Count() == 0;
        }

        public void Attach(IAttachRequest<ITemplate> request)
        {
            var target = request.Template;
            var logger = request.Logger;
            var descriptor = target.Descriptor.As<SparkDescriptor>();

            _templateLocator.LocateBindings(BindingsName, target).Each(template =>
            {
                descriptor.AddBinding(template);
                var msg = "Binding attached : {0}".ToFormat(template.FilePath);
                logger.Log(target, msg);
            });
        }
    }
}