using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Visualization;
using HtmlTags;

namespace FubuMVC.Diagnostics.ModelBinding
{
    public class ModelBindingFubuDiagnostics
    {
        private readonly BindingRegistry _bindingRegistry;

        public ModelBindingFubuDiagnostics(BindingRegistry bindingRegistry)
        {
            _bindingRegistry = bindingRegistry;
        }

        [System.ComponentModel.Description("Model Binding Explorer:Visualization of all the model binding strategies in order or precedence")]
        [UrlPattern("binding/all")]
        public ModelBindingExplorerViewModel get_binding_all()
        {
            var description = Description.For(_bindingRegistry);

            return new ModelBindingExplorerViewModel{
                ModelBindingGraphTag = new DescriptionBodyTag(description)
            };
        }
    }

    public class ModelBindingExplorerViewModel
    {
        public HtmlTag ModelBindingGraphTag { get; set; }
    }
}