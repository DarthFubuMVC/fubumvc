namespace FubuMVC.Docs
{
    public class FubuMvcMainTopicTopicRegistry : FubuDocs.TopicRegistry
    {
        public FubuMvcMainTopicTopicRegistry()
        {
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Ajax.AjaxEndpoints>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Architecture.FubumvcArchitecture>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Architecture.HttpRequestHandling>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Authorization.AuthorizationRules>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Asynch.AsynchronousRequestHandling>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Bootstrapping.BootstrappingAFubumvcApplication>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Bottles.ModularityWithBottles>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Caching.Caching>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Compression.ContentCompression>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Configuration.ConfiguringFubumvc>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Continuations.RedirectingTransferingOrStoppingARoute>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Conneg.ContentNegotiation>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Diagnostics.BuiltInDiagnostics>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.DisplayFormatting.DisplayFormatting>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Files.UploadingAndDownloadingFiles>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Glossary.GlossaryOfTerms>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.Hosts>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.AspNet.HostingInAspnet>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.Owin.Owin>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.SelfHost.WebApiSelfHost>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.UsingApplicationsettings>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.HowTo.FrequentlyAskedQuestions>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.IoC.IocContainerIntegration>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Migrating.MigratingFromAspnetMvc>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.ModelBinding.IntegrationWithModelBinding>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Routing.WorkingWithRouting>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Runtime.RuntimeServices>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Testing.TestingWithFubumvc>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Urls.UrlResolution>();

            For<FubuMVC.Docs.Ajax.AjaxEndpoints>().Append<FubuMVC.Docs.Ajax.DetectingAnAjaxRequest>();
            For<FubuMVC.Docs.Ajax.AjaxEndpoints>().Append<FubuMVC.Docs.Ajax.Ajaxcontinuation>();

            For<FubuMVC.Docs.Architecture.FubumvcArchitecture>().Append<FubuMVC.Docs.Architecture.RussianDollPattern>();
            For<FubuMVC.Docs.Architecture.FubumvcArchitecture>().Append<FubuMVC.Docs.Architecture.Behaviors.Behaviors>();
            For<FubuMVC.Docs.Architecture.FubumvcArchitecture>().Append<FubuMVC.Docs.Architecture.IocContainerIntegration>();
            For<FubuMVC.Docs.Architecture.FubumvcArchitecture>().Append<FubuMVC.Docs.Architecture.HowFubumvcBootstrapsItself>();

            For<FubuMVC.Docs.Architecture.Behaviors.Behaviors>().Append<FubuMVC.Docs.Architecture.Behaviors.>();
            For<FubuMVC.Docs.Architecture.Behaviors.Behaviors>().Append<FubuMVC.Docs.Architecture.Behaviors.ComposingBehaviors>();
            For<FubuMVC.Docs.Architecture.Behaviors.Behaviors>().Append<FubuMVC.Docs.Architecture.Behaviors.ConditionalBehaviors>();
            For<FubuMVC.Docs.Architecture.Behaviors.Behaviors>().Append<FubuMVC.Docs.Architecture.Behaviors.BuiltInBehaviors>();

            For<FubuMVC.Docs.Architecture.HowFubumvcBootstrapsItself>().Append<FubuMVC.Docs.Architecture.FindingBottles>();
            For<FubuMVC.Docs.Architecture.HowFubumvcBootstrapsItself>().Append<FubuMVC.Docs.Architecture.ApplyingIfuburegistryextension>();
            For<FubuMVC.Docs.Architecture.HowFubumvcBootstrapsItself>().Append<FubuMVC.Docs.Architecture.BuildingBehaviorgraph>();
            For<FubuMVC.Docs.Architecture.HowFubumvcBootstrapsItself>().Append<FubuMVC.Docs.Architecture.ServiceRegistration>();
            For<FubuMVC.Docs.Architecture.HowFubumvcBootstrapsItself>().Append<FubuMVC.Docs.Architecture.RunningActivators>();
            For<FubuMVC.Docs.Architecture.HowFubumvcBootstrapsItself>().Append<FubuMVC.Docs.Architecture.BuildingTheRoutetable>();

            For<FubuMVC.Docs.Architecture.BuildingBehaviorgraph>().Append<FubuMVC.Docs.Architecture.IconfigurationactionOrdering>();
            For<FubuMVC.Docs.Architecture.BuildingBehaviorgraph>().Append<FubuMVC.Docs.Architecture.FuburegistryImports>();
            For<FubuMVC.Docs.Architecture.BuildingBehaviorgraph>().Append<FubuMVC.Docs.Architecture.InternalTracing>();

            For<FubuMVC.Docs.Architecture.HttpRequestHandling>().Append<FubuMVC.Docs.Architecture.IntegrationWithRouting>();

            For<FubuMVC.Docs.Authorization.AuthorizationRules>().Append<FubuMVC.Docs.Authorization.RuntimeArchitecture>();
            For<FubuMVC.Docs.Authorization.AuthorizationRules>().Append<FubuMVC.Docs.Authorization.ConfiguringAuthorization>();
            For<FubuMVC.Docs.Authorization.AuthorizationRules>().Append<FubuMVC.Docs.Authorization.RoleBasedAuthorization>();
            For<FubuMVC.Docs.Authorization.AuthorizationRules>().Append<FubuMVC.Docs.Authorization.AuthorizationAgainstTheInputModel>();
            For<FubuMVC.Docs.Authorization.AuthorizationRules>().Append<FubuMVC.Docs.Authorization.AuthorizationAgainstRuntimeConditions>();
            For<FubuMVC.Docs.Authorization.AuthorizationRules>().Append<FubuMVC.Docs.Authorization.CombiningMultipleAuthorizationRules>();
            For<FubuMVC.Docs.Authorization.AuthorizationRules>().Append<FubuMVC.Docs.Authorization.Authorizationright>();
            For<FubuMVC.Docs.Authorization.AuthorizationRules>().Append<FubuMVC.Docs.Authorization.AuthorizationFailureHandling>();

            For<FubuMVC.Docs.Authorization.RuntimeArchitecture>().Append<FubuMVC.Docs.Authorization.Iauthorizationpolicy>();
            For<FubuMVC.Docs.Authorization.RuntimeArchitecture>().Append<FubuMVC.Docs.Authorization.Iauthorizationrule>();
            For<FubuMVC.Docs.Authorization.RuntimeArchitecture>().Append<FubuMVC.Docs.Authorization.Authorizationbehavior>();
            For<FubuMVC.Docs.Authorization.RuntimeArchitecture>().Append<FubuMVC.Docs.Authorization.Principalroles>();
            For<FubuMVC.Docs.Authorization.RuntimeArchitecture>().Append<FubuMVC.Docs.Authorization.Authorizationpolicyexecutor>();
            For<FubuMVC.Docs.Authorization.RuntimeArchitecture>().Append<FubuMVC.Docs.Authorization.Iauthorizationpreviewservice>();
            For<FubuMVC.Docs.Authorization.RuntimeArchitecture>().Append<FubuMVC.Docs.Authorization.Iendpointservice>();

            For<FubuMVC.Docs.Authorization.ConfiguringAuthorization>().Append<FubuMVC.Docs.Authorization.ByAttributes>();
            For<FubuMVC.Docs.Authorization.ConfiguringAuthorization>().Append<FubuMVC.Docs.Authorization.ByPoliciesOrConventions>();

            For<FubuMVC.Docs.Bootstrapping.BootstrappingAFubumvcApplication>().Append<FubuMVC.Docs.Bootstrapping.Fubuapplication>();
            For<FubuMVC.Docs.Bootstrapping.BootstrappingAFubumvcApplication>().Append<FubuMVC.Docs.Bootstrapping.ConventionsAndPolicies>();
            For<FubuMVC.Docs.Bootstrapping.BootstrappingAFubumvcApplication>().Append<FubuMVC.Docs.Bootstrapping.IocContainerIntegration>();
            For<FubuMVC.Docs.Bootstrapping.BootstrappingAFubumvcApplication>().Append<FubuMVC.Docs.Bootstrapping.CustomizingBottlesIntegration>();
            For<FubuMVC.Docs.Bootstrapping.BootstrappingAFubumvcApplication>().Append<FubuMVC.Docs.Bootstrapping.Iapplicationsource>();
            For<FubuMVC.Docs.Bootstrapping.BootstrappingAFubumvcApplication>().Append<FubuMVC.Docs.Bootstrapping.BestPractices>();

            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.FubumvcBottleLoading>();
            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.AssemblyBottles>();
            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.ZipBottles>();
            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.LinkedBottles>();
            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.IntegrationWithAFubumvcApplication>();

            For<FubuMVC.Docs.Bottles.IntegrationWithAFubumvcApplication>().Append<FubuMVC.Docs.Bottles.Ifuburegistryextension>();
            For<FubuMVC.Docs.Bottles.IntegrationWithAFubumvcApplication>().Append<FubuMVC.Docs.Bottles.ConfigurableBottles>();

            For<FubuMVC.Docs.Caching.Caching>().Append<FubuMVC.Docs.Caching.Architecture>();
            For<FubuMVC.Docs.Caching.Caching>().Append<FubuMVC.Docs.Caching.Configuration>();
            For<FubuMVC.Docs.Caching.Caching>().Append<FubuMVC.Docs.Caching.DoughnutCaching>();
            For<FubuMVC.Docs.Caching.Caching>().Append<FubuMVC.Docs.Caching.VaryByOptions>();
            For<FubuMVC.Docs.Caching.Caching>().Append<FubuMVC.Docs.Caching.EtagSupport>();

            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.TheConfigurationDsl>();
            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.RegisteringServices>();
            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.TheConfigurationModel>();
            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.Conventions.ConventionsAndPolicies>();
            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.Settings.WorkingWithSettings>();
            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.AccessorOverrides>();

            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.Fuburegistryactions>();
            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.Fuburegistryimport>();
            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.Fuburegistrymodels>();
            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.Fuburegistrypolicies>();
            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.Fuburegistryroute>();
            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.Fuburegistryroutes>();
            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.WorkingWithSettings>();
            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.ExplicitConfiguration>();
            For<FubuMVC.Docs.Configuration.TheConfigurationDsl>().Append<FubuMVC.Docs.Configuration.AddingServiceRegistrations>();

            For<FubuMVC.Docs.Configuration.RegisteringServices>().Append<FubuMVC.Docs.Configuration.Serviceregistry>();
            For<FubuMVC.Docs.Configuration.RegisteringServices>().Append<FubuMVC.Docs.Configuration.TheObjectdefModel>();

            For<FubuMVC.Docs.Configuration.TheConfigurationModel>().Append<FubuMVC.Docs.Configuration.BehaviorGraph.TheBehaviorgraphModel>();
            For<FubuMVC.Docs.Configuration.TheConfigurationModel>().Append<FubuMVC.Docs.Configuration.Iconfigurationaction>();
            For<FubuMVC.Docs.Configuration.TheConfigurationModel>().Append<FubuMVC.Docs.Configuration.Configurationpack>();

            For<FubuMVC.Docs.Configuration.BehaviorGraph.TheBehaviorgraphModel>().Append<FubuMVC.Docs.Configuration.BehaviorGraph.Behaviorchain>();
            For<FubuMVC.Docs.Configuration.BehaviorGraph.TheBehaviorgraphModel>().Append<FubuMVC.Docs.Configuration.BehaviorGraph.RoutedefinitionAndRouteinput>();
            For<FubuMVC.Docs.Configuration.BehaviorGraph.TheBehaviorgraphModel>().Append<FubuMVC.Docs.Configuration.BehaviorGraph.Behaviornode>();
            For<FubuMVC.Docs.Configuration.BehaviorGraph.TheBehaviorgraphModel>().Append<FubuMVC.Docs.Configuration.BehaviorGraph.InsertingBehaviornodes>();
            For<FubuMVC.Docs.Configuration.BehaviorGraph.TheBehaviorgraphModel>().Append<FubuMVC.Docs.Configuration.BehaviorGraph.ReorderingBehaviornodes>();

            For<FubuMVC.Docs.Configuration.Conventions.ConventionsAndPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.OutOfTheBoxConventions>();
            For<FubuMVC.Docs.Configuration.Conventions.ConventionsAndPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.CustomPolicies>();

            For<FubuMVC.Docs.Configuration.Conventions.CustomPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.UnderstandingConfigurationtype>();
            For<FubuMVC.Docs.Configuration.Conventions.CustomPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.UsingChainpredicates>();
            For<FubuMVC.Docs.Configuration.Conventions.CustomPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.ExtendingTheBasePolicyClass>();
            For<FubuMVC.Docs.Configuration.Conventions.CustomPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.Iactionsource>();
            For<FubuMVC.Docs.Configuration.Conventions.CustomPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.AssemblyTypeScanning>();

            For<FubuMVC.Docs.Continuations.RedirectingTransferingOrStoppingARoute>().Append<FubuMVC.Docs.Continuations.Fubucontinuation>();
            For<FubuMVC.Docs.Continuations.RedirectingTransferingOrStoppingARoute>().Append<FubuMVC.Docs.Continuations.UsingActionfilters>();
            For<FubuMVC.Docs.Continuations.RedirectingTransferingOrStoppingARoute>().Append<FubuMVC.Docs.Continuations.Iredirectable>();
            For<FubuMVC.Docs.Continuations.RedirectingTransferingOrStoppingARoute>().Append<FubuMVC.Docs.Continuations.Architecture>();

            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.HowItWorks>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.CustomizingMimetypeDetection>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.ConditionalOutputAtRuntime>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.DefaultConventions>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.ConfiguringConneg>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.DefaultConventions>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.ModelBinding>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.JsonEndpoints>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.HtmlEndpoints>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.ApplyingChromeToOutput>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.Architecture.ConnegArchitecture>();

            For<FubuMVC.Docs.Conneg.ConfiguringConneg>().Append<FubuMVC.Docs.Conneg.Reader>();
            For<FubuMVC.Docs.Conneg.ConfiguringConneg>().Append<FubuMVC.Docs.Conneg.Writer>();

            For<FubuMVC.Docs.Conneg.Architecture.ConnegArchitecture>().Append<FubuMVC.Docs.Conneg.Architecture.Imediawriter>();
            For<FubuMVC.Docs.Conneg.Architecture.ConnegArchitecture>().Append<FubuMVC.Docs.Conneg.Architecture.Outputbehavior>();
            For<FubuMVC.Docs.Conneg.Architecture.ConnegArchitecture>().Append<FubuMVC.Docs.Conneg.Architecture.Ireader>();
            For<FubuMVC.Docs.Conneg.Architecture.ConnegArchitecture>().Append<FubuMVC.Docs.Conneg.Architecture.Inputbehavior>();
            For<FubuMVC.Docs.Conneg.Architecture.ConnegArchitecture>().Append<FubuMVC.Docs.Conneg.Architecture.Iformatter>();
            For<FubuMVC.Docs.Conneg.Architecture.ConnegArchitecture>().Append<FubuMVC.Docs.Conneg.Architecture.Currentmimetype>();

            For<FubuMVC.Docs.Hosts.Owin.Owin>().Append<FubuMVC.Docs.Hosts.Owin.Katana>();

            For<FubuMVC.Docs.IoC.IocContainerIntegration>().Append<FubuMVC.Docs.IoC.Structuremap>();
            For<FubuMVC.Docs.IoC.IocContainerIntegration>().Append<FubuMVC.Docs.IoC.Autofac>();
            For<FubuMVC.Docs.IoC.IocContainerIntegration>().Append<FubuMVC.Docs.IoC.IntegratingANewIocContainer>();

            For<FubuMVC.Docs.ModelBinding.IntegrationWithModelBinding>().Append<FubuMVC.Docs.ModelBinding.BuiltInPropertyBinders>();
            For<FubuMVC.Docs.ModelBinding.IntegrationWithModelBinding>().Append<FubuMVC.Docs.ModelBinding.Cookies>();
            For<FubuMVC.Docs.ModelBinding.IntegrationWithModelBinding>().Append<FubuMVC.Docs.ModelBinding.HttpHeaders>();
            For<FubuMVC.Docs.ModelBinding.IntegrationWithModelBinding>().Append<FubuMVC.Docs.ModelBinding.HttpRequest>();
            For<FubuMVC.Docs.ModelBinding.IntegrationWithModelBinding>().Append<FubuMVC.Docs.ModelBinding.Files>();
            For<FubuMVC.Docs.ModelBinding.IntegrationWithModelBinding>().Append<FubuMVC.Docs.ModelBinding.RouteValues>();

            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.PartialOnlyEndpoints>();
            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.ExplicitRoutes>();
            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.RoutingConventions>();
            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.Sessionstaterequirement>();
            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.RouteInputs>();
            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.CustomizingRouteGeneration>();

            For<FubuMVC.Docs.Routing.RoutingConventions>().Append<FubuMVC.Docs.Routing.DefaultConventions>();
            For<FubuMVC.Docs.Routing.RoutingConventions>().Append<FubuMVC.Docs.Routing.RoutingAttributes>();

            For<FubuMVC.Docs.Runtime.RuntimeServices>().Append<FubuMVC.Docs.Runtime.PassingStateBetweenBehaviors>();
            For<FubuMVC.Docs.Runtime.RuntimeServices>().Append<FubuMVC.Docs.Runtime.Http.HttpAbstractions>();
            For<FubuMVC.Docs.Runtime.RuntimeServices>().Append<FubuMVC.Docs.Runtime.Conditionals>();
            For<FubuMVC.Docs.Runtime.RuntimeServices>().Append<FubuMVC.Docs.Runtime.WorkingWithStaticFiles>();
            For<FubuMVC.Docs.Runtime.RuntimeServices>().Append<FubuMVC.Docs.Runtime.Logging>();

            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.Icurrenthttprequest>();
            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.Cookies>();
            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.HttpHeaders>();
            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.WritingHttpOutput>();
            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.TheIfuburequestModelBag>();
            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.Irequestdata>();
            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.Icurrentchain>();
            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.Istreamingdata>();
            For<FubuMVC.Docs.Runtime.Http.HttpAbstractions>().Append<FubuMVC.Docs.Runtime.Http.QueryingTheHttpResponse>();

            For<FubuMVC.Docs.Runtime.WorkingWithStaticFiles>().Append<FubuMVC.Docs.Runtime.Ifubuapplicationfiles>();

            For<FubuMVC.Docs.Runtime.Logging>().Append<FubuMVC.Docs.Runtime.Iexceptionhandlingobserver>();

            For<FubuMVC.Docs.Testing.TestingWithFubumvc>().Append<FubuMVC.Docs.Testing.Endpointdriver>();
            For<FubuMVC.Docs.Testing.TestingWithFubumvc>().Append<FubuMVC.Docs.Testing.UnitTesting>();

            For<FubuMVC.Docs.Urls.UrlResolution>().Append<FubuMVC.Docs.Urls.Iurlregistry>();
            For<FubuMVC.Docs.Urls.UrlResolution>().Append<FubuMVC.Docs.Urls.Iendpointservice>();
            For<FubuMVC.Docs.Urls.UrlResolution>().Append<FubuMVC.Docs.Urls.ChainForwarders>();
            For<FubuMVC.Docs.Urls.UrlResolution>().Append<FubuMVC.Docs.Urls.ChainCategories>();
            For<FubuMVC.Docs.Urls.UrlResolution>().Append<FubuMVC.Docs.Urls.OpenUrlPatterns>();
            For<FubuMVC.Docs.Urls.UrlResolution>().Append<FubuMVC.Docs.Urls.IurlregistryInUnitTesting>();

        }
    }
}
