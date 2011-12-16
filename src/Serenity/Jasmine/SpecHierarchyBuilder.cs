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

        public HtmlTag TopTag(HtmlTag topChild)
        {
            if (topChild.TagName() != "ul")
            {
                throw new ArgumentOutOfRangeException("Only ul tags are valid here:  \n" + topChild.ToString());
            }

            var topUrl = _urls.UrlFor<JasminePages>(x => x.AllSpecs(), null);
            return new HtmlTag("ul", tag =>
            {
                tag.Id("all-specs-node").AddClass("filetree");

                var link = new LinkTag("All Specs", topUrl, "all-specs");
                var li = tag.Add("li");
                li.Add("span").AddClass("folder").Append(link);

                li.Append(topChild);
            });
        }

        public HtmlTag CompleteHierarchy()
        {
            var topChild = new HtmlTag("ul");
            var builder = new ChildTagBuilder(this, topChild);
            builder.AddChildren(_graph);

            return TopTag(topChild);
        
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
                return new HtmlTag("ul").Append(currentTag).Id("all-specs-node").AddClass("filetree");
            }

            var topTag = TopTag(new HtmlTag("ul").Append(currentTag) );

            return topTag;
        }

        public HtmlTag BuildLeafTag(ISpecNode node)
        {
            return new HtmlTag("li", tag =>
            {
                var url = _urls.UrlFor(node.Path());
                var link = new LinkTag(node.Path().Parts.Last(), url);

                tag.Add("span").AddClass("file").Append(link);
            });
        }

        public HtmlTag BuildActiveTag(ISpecNode node)
        {
            return new HtmlTag("li", x =>
            {
                string text = node.Path().Parts.LastOrDefault() ?? "All Specs";
                x.Add("span").AddClass("active").Text(text).AddClass(node.TreeClass);
            });
        }

        public HtmlTag BuildFolderTag(SpecificationFolder folder)
        {
            var folderTag = new HtmlTag("li");
            var link = linkTagForFolder(folder);

            folderTag.Add("span").AddClass("folder").Append(link);

            var ul = folderTag.Add("ul");

            var builder = new ChildTagBuilder(this, ul);
            folder.ImmediateChildren.Each(x => x.AcceptVisitor(builder));


            return folderTag;
        }

        private LinkTag linkTagForFolder(SpecificationFolder folder)
        {
            var specPath = folder.Path();
            var url = _urls.UrlFor(specPath);
            return new LinkTag(specPath.Parts.Last(), url);
        }

        #region Nested type: ChildTagBuilder

        public class ChildTagBuilder : ISpecVisitor
        {
            private readonly SpecHierarchyBuilder _builder;
            private readonly HtmlTag _parent;

            public ChildTagBuilder(SpecHierarchyBuilder builder, HtmlTag parent)
            {
                if (parent.TagName() != "ul")
                {
                    throw new ArgumentOutOfRangeException("Only ul tags are valid here:  \n" + parent.ToString());
                }

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