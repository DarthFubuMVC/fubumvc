using System.IO;
using System.Reflection;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.UI;
using HtmlTags;


namespace FubuMVC.GettingStarted
{
    public class StartingController
    {
        private readonly IServiceLocator _services;
        private readonly ICurrentHttpRequest _request;

        public StartingController(IServiceLocator services, ICurrentHttpRequest request)
        {
            _services = services;
            _request = request;
        }

        public HtmlDocument GettingStarted()
        {
            var document = new FubuHtmlDocument(_services);
            document.Title = "Getting Started with FubuMVC";
            document.Add(x => x.Image("logo.png"));

            document.Push("div");
            document.Add("hr");
            
            // we're gonna cheat here since the diagnostics markers changed
            // TODO -- Share input models for the dashboard
            document.Add(new LinkTag("Diagnostics for your application", _request.ToFullUrl("_fubu")));

            document.Add("div").Encoded(false).Text(getContent());

            return document;
        }

        private string getContent()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), "Content.htm");
            return new StreamReader(stream).ReadToEnd();
        }
    }

    public class GettingStartedExtensions : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Route("_fubu/getting_started").Calls<StartingController>(x => x.GettingStarted());

            registry.Configure(graph =>
            {
                var chain = graph.FindHomeChain();
                if (chain == null)
                {
                    graph.BehaviorFor<StartingController>(x => x.GettingStarted()).Route = new RouteDefinition(string.Empty);
                }
            });
        }
    }
}