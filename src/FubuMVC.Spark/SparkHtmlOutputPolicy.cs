using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using HtmlTags;

namespace FubuMVC.Spark
{
	public class SparkHtmlOutputPolicy : IConfigurationAction
	{
		public void Configure(BehaviorGraph graph)
		{
			graph.Behaviors
				.Where(x => x.ActionOutputType().CanBeCastTo<HtmlTag>())
				.Each(chain =>
				      {
				      	if (!chain.HasOutputBehavior()) return;

				      	chain.Outputs.First().ReplaceWith(new SparkHtmlTagOutput());

						graph.Observer.RecordCallStatus(chain.FirstCall(), "Replaced output node for HtmlTag with SparkHtmlTagOutput.");
				      });

			graph.Behaviors
				.Where(x => x.ActionOutputType().CanBeCastTo<HtmlDocument>())
				.Each(chain =>
				      {
				      	if (!chain.HasOutputBehavior()) return;

				      	chain.Outputs.First().ReplaceWith(new SparkHtmlDocumentOutput());

				      	graph.Observer.RecordCallStatus(chain.FirstCall(),
														"Replaced output node for HtmlTag with SparkHtmlDocumentOutput.");
				      });
		}
	}
}