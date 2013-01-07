namespace FubuMVC.Docs
{
    public class FubuMvcMainTopicTopicRegistry : FubuDocs.TopicRegistry
    {
        public FubuMvcMainTopicTopicRegistry()
        {
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Ajax.AjaxEndpoints>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Architecture.FubumvcArchitecture>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Bottles.ModularityWithBottles>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Configuration.ConfiguringFubumvc>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Conneg.ContentNegotiation>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.Hosts>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.AspNet.HostingInAsp.Net>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.Owin.Owin>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.SelfHost.WebApiSelfHost>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.UsingApplicationsettings>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Routing.WorkingWithRouting>();

            For<FubuMVC.Docs.Ajax.AjaxEndpoints>().Append<FubuMVC.Docs.Ajax.DetectingAnAjaxRequest>();

            For<FubuMVC.Docs.Architecture.FubumvcArchitecture>().Append<FubuMVC.Docs.Architecture.RussianDollPattern>();
            For<FubuMVC.Docs.Architecture.FubumvcArchitecture>().Append<FubuMVC.Docs.Architecture.Behaviors.Behaviors>();

            For<FubuMVC.Docs.Architecture.Behaviors.Behaviors>().Append<FubuMVC.Docs.Architecture.Behaviors.Wrappers>();

            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.FubumvcBottleLoading>();
            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.AssemblyBottles>();
            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.ZipBottles>();
            For<FubuMVC.Docs.Bottles.ModularityWithBottles>().Append<FubuMVC.Docs.Bottles.LinkedBottles>();

            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.TheConfigurationDsl>();
            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.TheConfigurationModel>();
            For<FubuMVC.Docs.Configuration.ConfiguringFubumvc>().Append<FubuMVC.Docs.Configuration.Conventions.ConventionsAndPolicies>();

            For<FubuMVC.Docs.Configuration.TheConfigurationModel>().Append<FubuMVC.Docs.Configuration.Iconfigurationaction>();

            For<FubuMVC.Docs.Configuration.Conventions.ConventionsAndPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.OutOfTheBoxConventions>();
            For<FubuMVC.Docs.Configuration.Conventions.ConventionsAndPolicies>().Append<FubuMVC.Docs.Configuration.Conventions.CustomPolicies>();

            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.HowItWorks>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.DefaultConventions>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.ConfiguringConneg>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.DefaultConventions>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.JsonEndpoints>();
            For<FubuMVC.Docs.Conneg.ContentNegotiation>().Append<FubuMVC.Docs.Conneg.HtmlEndpoints>();

            For<FubuMVC.Docs.Conneg.ConfiguringConneg>().Append<FubuMVC.Docs.Conneg.[Conneg]Attribute>();

            For<FubuMVC.Docs.Hosts.Owin.Owin>().Append<FubuMVC.Docs.Hosts.Owin.Katana>();

            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.PartialOnlyEndpoints>();
            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.ExplicitRoutes>();
            For<FubuMVC.Docs.Routing.WorkingWithRouting>().Append<FubuMVC.Docs.Routing.RoutingConventions>();

            For<FubuMVC.Docs.Routing.RoutingConventions>().Append<FubuMVC.Docs.Routing.DefaultConventions>();
            For<FubuMVC.Docs.Routing.RoutingConventions>().Append<FubuMVC.Docs.Routing.RoutingAttributes>();

        }
    }
}
