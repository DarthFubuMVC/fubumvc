using System;
using System.IO;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.UI;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace FubuMVC.GettingStarted
{
    public class StartingController
    {
        private readonly IServiceLocator _services;
        private readonly BehaviorGraph _graph;

        public StartingController(IServiceLocator services, BehaviorGraph graph)
        {
            _services = services;
            _graph = graph;
        }

        public HtmlDocument GettingStarted()
        {
            var document = new FubuHtmlDocument(_services);
            document.Title = "Getting Started with FubuMVC";
            document.Add(x => x.Image("logo.png"));

            document.Push("div");
            document.Add("hr");
            document.Add(x => x.LinkTo<BehaviorGraphWriter>(o => o.Index()).Text("Diagnostics for your application"));

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