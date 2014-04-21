using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Diagnostics.Visualization
{
	public class DescriptionVisualizationPolicy : IConfigurationAction, DescribesItself
	{
		public void Configure(BehaviorGraph graph)
		{
			graph
				.Behaviors
				.Where(x => x.ResourceType() == typeof (Description))
				.Each(x => x.Output.Add(typeof(DescriptionWriter))); // TODO -- this might be wrong
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Added the Diagnostics visualization for the Description";
		}
	}

	public class DescriptionWriter : IMediaWriter<Description>, DescribesItself
	{
		private readonly IVisualizer _visualizer;

		public DescriptionWriter(IVisualizer visualizer)
		{
			_visualizer = visualizer;
		}

	    public void Write(string mimeType, IFubuRequestContext context, Description resource)
	    {
            var tag = _visualizer.VisualizeDescription(resource, false);
            context.Writer.WriteHtml(tag);
	    }

	    public IEnumerable<string> Mimetypes
		{
			get { yield return MimeType.Html.Value; }
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Invokes the IVisualizer interface to visualize the Description";
		}
	}
}