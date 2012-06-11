using System;
using FubuCore.Formatting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.UI;

namespace FubuMVC.Core
{
    public interface IFubuRegistry
    {
        RouteConventionExpression Routes { get; }
        ViewExpression Views { get; }
        PoliciesExpression Policies { get; }
        ModelsExpression Models { get; }
        AppliesToExpression Applies { get; }
        ActionCallCandidateExpression Actions { get; }
        MediaExpression Media { get; }
        string Name { get; }

        void Services(Action<IServiceRegistry> configure);

        void ApplyConvention<TConvention>()
            where TConvention : IConfigurationAction, new();

        void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction;

        ExplicitRouteConfiguration.ChainedBehaviorExpression Route(string pattern);
        void Import<T>(string prefix) where T : FubuRegistry, new();
        void Import(FubuRegistry registry, string prefix);
        void IncludeDiagnostics(bool shouldInclude);


        /// <summary>
        ///   This allows you to drop down to direct manipulation of the BehaviorGraph
        ///   produced by this FubuRegistry
        /// </summary>
        /// <param name = "alteration"></param>
        void Configure(Action<BehaviorGraph> alteration);

        BehaviorGraph BuildGraph();
    }
}