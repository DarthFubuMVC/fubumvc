using System;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.UI.Forms;
using FubuMVC.Core.Urls;
using FubuMVC.Core.Validation.Web;
using FubuMVC.Core.Validation.Web.UI;
using HtmlTags;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class FormValidationModifierTester
    {
        private BehaviorGraph theGraph;
        private ValidationSettings theSettings;

        public FormValidationModifierTester()
        {
            theSettings = new ValidationSettings();
            theGraph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<FormValidationModeEndpoint>();
                x.Features.Validation.Enable(true);
                x.Policies.Local.Add<ValidationPolicy>();
            });
        }

        private FormRequest requestFor<T>() where T : class, new()
        {
            var services = new InMemoryServiceLocator();
            services.Add<IChainResolver>(new ChainResolutionCache(theGraph));
            services.Add<IChainUrlResolver>(new ChainUrlResolver(new OwinHttpRequest()));
            services.Add<ITypeResolver>(new TypeResolver());
            services.Add(new AccessorRules());
            services.Add<ITypeDescriptorCache>(new TypeDescriptorCache());
            services.Add(theSettings);

            var request = new FormRequest(new ChainSearch { Type = typeof(T) }, new T());
            request.Attach(services);
            request.ReplaceTag(new FormTag("test"));

            return request;
        }

        [Fact]
        public void modifies_the_form()
        {
            var theRequest = requestFor<AjaxTarget>();

            var modifier = new FormValidationModifier();
            modifier.Modify(theRequest);

            theRequest.CurrentTag.Data("validation-summary").ShouldBe(true);
            theRequest.CurrentTag.Data("validation-highlight").ShouldBe(true);
            theRequest.CurrentTag.HasClass("validated-form").ShouldBeTrue();
        }

        [Fact]
        public void excluded_from_activation()
        {
            theSettings.ExcludeFormActivation = chain => chain.InputType() == typeof(AjaxTarget);
                
            var theRequest = requestFor<AjaxTarget>();

            var modifier = new FormValidationModifier();
            modifier.Modify(theRequest);

            theRequest.CurrentTag.Data("validation-summary").ShouldBe(true);
            theRequest.CurrentTag.Data("validation-highlight").ShouldBe(true);
            theRequest.CurrentTag.HasClass("validated-form").ShouldBeTrue();
        }

        [Fact]
        public void no_strategies()
        {
            var theRequest = requestFor<AjaxTarget>();
            theRequest.Chain.ValidationNode().Clear();

            var modifier = new FormValidationModifier();
            modifier.Modify(theRequest);

            theRequest.CurrentTag.HasClass("validated-form").ShouldBeFalse();
        }

        [Fact]
        public void adds_the_validation_options()
        {
            var theRequest = requestFor<AjaxTarget>();
            var modifier = new FormValidationModifier();
            modifier.Modify(theRequest);

            var options = ValidationOptions.For(theRequest);

            theRequest.CurrentTag.Data(ValidationOptions.Data).ShouldBe(options);
        }

        [Fact]
        public void writes_the_validation_activator_requirement()
        {
            var theRequest = requestFor<AjaxTarget>();
            var modifier = new FormValidationModifier();
            modifier.Modify(theRequest);
        }
    }

    public class LoFiTarget
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }

    public class AjaxTarget { }
    [NotValidated]
    public class NoneTarget { }
    public class IgnoredTarget { }

    public class FormValidationModeEndpoint
    {
        public NoneTarget post_none(NoneTarget target)
        {
            throw new NotImplementedException();
        }

        [NotValidated]
        public IgnoredTarget post_ignored(IgnoredTarget target)
        {
            throw new NotImplementedException();
        }

        public LoFiTarget post_lofi(LoFiTarget target)
        {
            throw new NotImplementedException();
        }

        public AjaxContinuation post_ajax(AjaxTarget target)
        {
            throw new NotImplementedException();
        }
    }
}