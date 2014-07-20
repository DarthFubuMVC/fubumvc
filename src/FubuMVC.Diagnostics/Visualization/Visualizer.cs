using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using FubuCore.Descriptions;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Bootstrap.Tags;
using HtmlTags;

namespace FubuMVC.Diagnostics.Visualization
{
    public class Visualizer : IVisualizer
    {
        private static readonly Cache<Type, string> _glyphs = new Cache<Type, string>(type => "icon-cog");

        private readonly FubuHtmlDocument _document;
        private readonly Cache<Type, bool> _hasVisualizer;

        public Visualizer(BehaviorGraph graph, FubuHtmlDocument document)
        {
            _document = document;
            _hasVisualizer = new Cache<Type, bool>(type => graph.Behaviors.Any(x => type == x.InputType()));
        }

        public BehaviorNodeViewModel ToVisualizationSubject(BehaviorNode node)
        {
            var description = Description.For(node);


            return new BehaviorNodeViewModel
            {
                Description = description,
                Node = node,
                VisualizationHtml = HasVisualizer(node.GetType())
                    ? _document.PartialFor(node).ToString()
                    : new DescriptionBodyTag(description).ToString()
            };
        }

        public IHtmlString Visualize(object @object)
        {
            if (@object == null) throw new ArgumentNullException("object");


            if (HasVisualizer(@object.GetType()))
            {
                return _document.PartialFor(@object);
            }

            var description = Description.For(@object);
            return VisualizeDescription(description);
        }


        public bool HasVisualizer(Type type)
        {
            return _hasVisualizer[type];
        }

        public static void Use<T>(string name)
        {
            _glyphs[typeof (T)] = name;
        }

        public static string GlyphFor(Type type)
        {
            return _glyphs[type];
        }


        public HtmlTag VisualizeDescription(Description description)
        {
            var titleTag = new HtmlTag("div", x => {
                x.PrependGlyph(GlyphFor(description.TargetType));
                x.Add("span").Text(description.Title);
            });

            if (!description.HasMoreThanTitle())
            {
                return titleTag;
            }

            var tag = new HtmlTag("div");
            tag.Append(titleTag);
            tag.Append(new DescriptionBodyTag(description));

            return tag;
        }
    }


    public static class HtmlTagExtensions
    {
        public static LiteralTag ToLiteral(this object target)
        {
            return new LiteralTag(target.ToString());
        }
    }
}