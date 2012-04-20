using System;
using FubuCore;
using FubuCore.Formatting;
using FubuMVC.Core.Diagnostics;
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
        DiagnosticLevel DiagnosticLevel { get; }
        string Name { get; }

        void UsingObserver(IConfigurationObserver observer);
        void Services(Action<IServiceRegistry> configure);

        void ApplyConvention<TConvention>()
            where TConvention : IConfigurationAction, new();

        void ApplyConvention<TConvention>(TConvention convention)
            where TConvention : IConfigurationAction;

        ExplicitRouteConfiguration.ChainedBehaviorExpression Route(string pattern);
        void Import<T>(string prefix) where T : FubuRegistry, new();
        void Import(FubuRegistry registry, string prefix);
        void IncludeDiagnostics(bool shouldInclude);
        void IncludeDiagnostics(Action<IDiagnosticsConfigurationExpression> configure);


        /// <summary>
        ///   This allows you to drop down to direct manipulation of the BehaviorGraph
        ///   produced by this FubuRegistry
        /// </summary>
        /// <param name = "alteration"></param>
        void Configure(Action<BehaviorGraph> alteration);

        void HtmlConvention<T>() where T : HtmlConventionRegistry, new();
        void HtmlConvention(HtmlConventionRegistry conventions);
        void HtmlConvention(Action<HtmlConventionRegistry> configure);
        void StringConversions<T>() where T : DisplayConversionRegistry, new();
        void StringConversions(Action<DisplayConversionRegistry> configure);
        BehaviorGraph BuildGraph();
    }
}