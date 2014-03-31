using System.Collections.Generic;
using FubuMVC.Core.Registration;
using HtmlTags;

namespace FubuApp
{
    public class RoutesEndpoint
    {
        private readonly BehaviorGraph _graph;

        public RoutesEndpoint(BehaviorGraph graph)
        {
            _graph = graph;
        }

        public HtmlDocument get_all_routes()
        {
            var document = new HtmlDocument
            {
                Title = "All the routes"
            };

            _graph.Behaviors.Each(x => {
                document.Add("p").Text(x.ToString());
            });

            return document;
        } 
    }
}