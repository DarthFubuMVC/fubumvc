using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI;
using FubuMVC.Core.Validation.Web.Remote;
using FubuMVC.Core.Validation.Web.UI;

namespace FubuMVC.Core.Validation.Web
{
    public interface IApplyValidationFilter
    {
        bool Filter(BehaviorChain chain);
    }

    public interface IFormActivationFilter
    {
        bool ShouldActivate(BehaviorChain chain);
    }

    public class ValidationSettings : ValidationSettingsRegistry, IApplyValidationFilter, IFormActivationFilter, IFeatureSettings
    {
        private readonly IList<IRemoteRuleFilter> _remoteFilters = new List<IRemoteRuleFilter>();

        public ValidationSettings()
        {
            FailAjaxRequestsWith(HttpStatusCode.BadRequest);

            Remotes
                    .FindWith<RemoteRuleAttributeFilter>()
                    .FindWith<RemoteFieldValidationRuleFilter>();
        }

        public HttpStatusCode StatusCode { get; private set; }
        public RemoteRuleExpression Remotes { get { return new RemoteRuleExpression(_remoteFilters); } }
        public IEnumerable<IRemoteRuleFilter> Filters { get { return _remoteFilters; } }

        public Func<BehaviorChain, bool> ExcludeFormActivation { get; set; } = chain => false;

        public Func<BehaviorChain, bool> Where { get; set; } =
            chain => chain is RoutedChain && chain.As<RoutedChain>().MatchesCategoryOrHttpMethod("POST") && chain.InputType() != null && !chain.Calls.Any(x => x.HasAttribute<NotValidatedAttribute>());
        
        public void Import<T>()
            where T : ValidationSettingsRegistry, new()
        {
            var registry = new T();
            registry.Modifications.Each(addModification);
        }

        public void FailAjaxRequestsWith(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public bool Filter(BehaviorChain chain)
        {
            return Where(chain);
        }

        bool IFormActivationFilter.ShouldActivate(BehaviorChain chain)
        {
            return Where(chain) && !ExcludeFormActivation(chain);
        }

        public void Modify(BehaviorChain chain)
        {
            Modifications
                .Where(x => x.Matches(chain))
                .Each(x => x.Modify(chain));
        }

        public bool Enabled { get; set; }

        public void Apply(FubuRegistry registry)
        {
            registry.Services.IncludeRegistry<FubuValidationServiceRegistry>();

            if (!Enabled) return;

            registry.Services.IncludeRegistry<FubuMvcValidationServices>();
            registry.Actions.FindWith<RemoteRulesSource>();
            registry.Actions.FindWith<ValidationSummarySource>();

            registry.Import<HtmlConventionRegistry>(x =>
            {
                x.Editors.Add(new FieldValidationElementModifier());
                x.Editors.Add(new RemoteValidationElementModifier());
                x.Editors.Add(new DateElementModifier());
                x.Editors.Add(new NumberElementModifier());
                x.Editors.Add(new MaximumLengthModifier());
                x.Editors.Add(new MinimumLengthModifier());
                x.Editors.Add(new RangeLengthModifier());
                x.Editors.Add(new MinValueModifier());
                x.Editors.Add(new MaxValueModifier());
                x.Editors.Add(new LocalizationLabelModifier());
                x.Editors.Add(new RegularExpressionModifier());

                x.Forms.Add(new FormValidationSummaryModifier());
                x.Forms.Add(new FormValidationModifier());
                x.Forms.Add(new FieldEqualityFormModifier());
                x.Forms.Add(new NotificationSerializationModifier());
            });
        }
    }
}