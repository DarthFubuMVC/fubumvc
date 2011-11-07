using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace Serenity.Jasmine
{
    public class SpecHierarchyBuilder
    {
        private readonly SpecificationGraph _graph;
        private readonly IUrlRegistry _urls;

        public SpecHierarchyBuilder(SpecificationGraph graph, IUrlRegistry urls)
        {
            _graph = graph;
            _urls = urls;
        }

        public HtmlTag TopTag()
        {
            var topUrl = _urls.UrlFor<JasminePages>(x => x.AllSpecs());
            return new HtmlTag("ul", tag =>
            {
                var link = new LinkTag("All Specs", topUrl, "all-specs");
                tag.Add("li").Append(link);
            });
        }

        public HtmlTag CompleteHierarchy()
        {
            var top = TopTag();

            var builder = new ChildTagBuilder(this, top.Add("ul"));
            builder.AddChildren(_graph);

            return top;
        }

        public HtmlTag BuildInPlaceHierarchyFor(ISpecNode node)
        {
            var active = BuildActiveTag(node);

            var builder = new ChildTagBuilder(this, active.Add("ul"));
            builder.AddChildren(node);

            var currentTag = active;
            while (node.Parent() != null)
            {
                var parentTag = BuildLeafTag(node.Parent());
                parentTag.Add("ul").Append(currentTag);

                currentTag = parentTag;
                
                node = node.Parent();
            }

            if (node is SpecificationGraph)
            {
                return new HtmlTag("ul", x => x.Append(currentTag));
            }

            var topTag = TopTag();
            topTag.Add("ul").Append(currentTag);

            return topTag;
        }

        public HtmlTag BuildLeafTag(ISpecNode node)
        {
            return new HtmlTag("li", tag =>
            {
                var url = _urls.UrlFor(node.Path());
                var link = new LinkTag(node.Path().Parts.Last(), url);

                tag.Append(link);
            });
        }

        public HtmlTag BuildActiveTag(ISpecNode node)
        {
            return new HtmlTag("li", x =>
            {
                string text = node.Path().Parts.LastOrDefault() ?? "All Specs";
                x.Add("span").AddClass("active").Text(text);
            });
        }

        public HtmlTag BuildFolderTag(SpecificationFolder folder)
        {
            var folderTag = new HtmlTag("li");
            var specPath = folder.Path();
            var url = _urls.UrlFor(specPath);

            var link = new LinkTag(specPath.Parts.Last(), url, "folder");
            folderTag.Append(link);

            var ul = folderTag.Add("ul");
            var builder = new ChildTagBuilder(this, ul);
            folder.ImmediateChildren.Each(x => x.AcceptVisitor(builder));


            return folderTag;
        }

        #region Nested type: ChildTagBuilder

        public class ChildTagBuilder : ISpecVisitor
        {
            private readonly SpecHierarchyBuilder _builder;
            private readonly HtmlTag _parent;

            public ChildTagBuilder(SpecHierarchyBuilder builder, HtmlTag parent)
            {
                _builder = builder;
                _parent = parent;
            }

            public void Specification(Specification spec)
            {
                var tag = _builder.BuildLeafTag(spec);
                _parent.Append(tag);
            }

            public void Folder(SpecificationFolder folder)
            {
                var tag = _builder.BuildFolderTag(folder);
                _parent.Append(tag);
            }

            public void Graph(SpecificationGraph graph)
            {
                throw new NotImplementedException();
            }

            public void AddChildren(ISpecNode node)
            {
                node.ImmediateChildren.Each(child => child.AcceptVisitor(this));
            }
        }

        #endregion
    }
}