namespace FubuMVC.Docs
{
    public class FubuMvcMainTopicTopicRegistry : FubuDocs.TopicRegistry
    {
        public FubuMvcMainTopicTopicRegistry()
        {
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Ajax.AjaxEndpoints>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Behaviors.RussianDollPattern>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Behaviors.Behaviors>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Conneg.ContentNegotiation>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.Hosts>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.AspNet.HostingInAsp.Net>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.Owin.Owin>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.SelfHost.WebApiSelfHost>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Hosts.UsingApplicationsettings>();
            For<FubuMVC.Docs.FubuMvcMainTopic>().Append<FubuMVC.Docs.Routing.WorkingWithRouting>();

            For<FubuMVC.Docs.Ajax.AjaxEndpoints>().Append<FubuMVC.Docs.Ajax.DetectingAnAjaxRequest>();

            For<FubuMVC.Docs.Behaviors.Behaviors>().Append<FubuMVC.Docs.Behaviors.Wrappers>();

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
