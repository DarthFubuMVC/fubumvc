using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Diagnostics;
using HtmlTags;

namespace FubuMVC.Diagnostics.Conventions
{
    public class ConventionsFubuDiagnostics
    {
        private readonly ConfigGraph _graph;

        public ConventionsFubuDiagnostics(ConfigGraph graph)
        {
            _graph = graph;
        }

        [System.ComponentModel.Description("Conventions and Policies")]
        public ConventionsViewModel get_conventions()
        {
            var configTypes = new string[]
                   {
                       ConfigurationType.Settings,
                       ConfigurationType.Explicit,
                       ConfigurationType.Policy,
                       ConfigurationType.Reordering
                   };

            var tag = new HtmlTag("ul");
            configTypes.Each(configType => {
                tag.Add("li/a").Text(configType).Attr("href", "#" + configType);
            });


            return new ConventionsViewModel
            {
                Descriptions = new TagList(configTypes.Select(configType => new ConfigurationTypeTag(configType, _graph))),
                TableOfContents = tag
            };
        }
    }



    public class ConfigurationTypeTag : HtmlTag
    {
        public ConfigurationTypeTag(string configurationType, ConfigGraph graph) : base("div")
        {
            Add("a").Id(configurationType);
            Add("h2").Text(configurationType).Style("margin-bottom", "10px");
            

            graph.ActionsFor(configurationType).Each(action => {
                var desc = Description.For(action);
                var body = new DescriptionBodyTag(desc);
                body.Children.Insert(0, new HtmlTag("h4").Text(desc.Title));

                Append(body);
                Add("hr");
            });
        }
    }

    public class ConventionsViewModel
    {
        public TagList Descriptions { get; set; }
        public HtmlTag TableOfContents { get; set; }
    }
}