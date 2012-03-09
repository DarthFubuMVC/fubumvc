using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuMVC.Core.Urls;
using HtmlTags;
using System.Linq;
using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    [FubuDiagnostics("Model Binding")]
    public class ModelBindingExplorerWriter
    {
        private readonly BindingRegistry _registry;
        private readonly IUrlRegistry _urls;

        public ModelBindingExplorerWriter(BindingRegistry registry, IUrlRegistry urls)
        {
            _registry = registry;
            _urls = urls;
        }

        [FubuDiagnostics("Everything", ShownInIndex = true)]
        public HtmlDocument Everything()
        {
            var tag = new DescriptionTag(Description.For(_registry));


            HtmlDocument document = DiagnosticHtml.BuildDocument(_urls, "Everything", tag);

            return document;
        }
    }

    public class DescriptionTag : HtmlTag
    {
        public DescriptionTag(Description description, string tagName = "div") : base(tagName)
        {
            AddClass("description");
            Add("b").AddClass("title").Text(description.Title);
            Add("span").AddClass("short-description").Text(description.ShortDescription);

            description.BulletLists.Each(list =>
            {
                Append(new BulletListTag(list));
            });
        }
    }

    public class BulletListTag : HtmlTag
    {
        public BulletListTag(BulletList list) : base("div")
        {
            AddClass("list");

            Add("b").AddClass("list-header").Text(list.Label);

            var holder = list.IsOrderDependent ? Add("ol") : Add("ul");

            list.Children.Each(x =>
            {
                holder.Append(new DescriptionTag(x, "li"));
            });
        }
    }

    public class OrderedBulletListTag : HtmlTag
    {
        public OrderedBulletListTag(BulletList list) : base("div")
        {
            AddClass("list");
        }
    }
}