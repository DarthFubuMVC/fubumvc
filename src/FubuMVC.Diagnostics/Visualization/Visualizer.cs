using System;
using System.Linq;
using System.Web;
using FubuCore.Descriptions;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Bootstrap.Collapsibles;
using FubuMVC.Core.UI.Bootstrap.Tags;
using FubuMVC.Diagnostics.Requests;
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
            _hasVisualizer = new Cache<Type, bool>(type => { return graph.Behaviors.Any(x => type == x.InputType()); });
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

        private object contentFor(object log)
        {
            if (_hasVisualizer[log.GetType()])
            {
                return _document.PartialFor(log);
            }

            var description = Description.For(log);
            return VisualizeDescription(description);
        }

        public HtmlTag VisualizeDescription(Description description)
        {
            if (!description.HasMoreThanTitle())
            {
                return new HtmlTag("div", x => {
                    x.PrependGlyph(GlyphFor(description.TargetType));
                    x.Add("span").Text(description.Title);
                });
            }

            return new DescriptionBodyTag(description);
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