using System;
using System.Web;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using HtmlTags;

namespace FubuMVC.Diagnostics.Visualization
{
    public interface IVisualizer
    {
        BehaviorNodeViewModel ToVisualizationSubject(BehaviorNode node);

        bool HasVisualizer(Type type);
        HtmlTag VisualizeDescription(Description description);

        /// <summary>
        /// Creates and renders a visualization for the object.  Tries to find a partial
        /// for the type first, then falls back to rendering the Description of the object
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        IHtmlString Visualize(object @object);
    }
}