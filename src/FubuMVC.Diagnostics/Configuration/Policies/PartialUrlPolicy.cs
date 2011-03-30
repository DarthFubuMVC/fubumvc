using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Diagnostics.Partials;

namespace FubuMVC.Diagnostics.Configuration.Policies
{
	public class PartialUrlPolicy : IUrlPolicy
	{
		public bool Matches(ActionCall call, IConfigurationObserver log)
		{
			if(!call.HandlerType.Closes(typeof(PartialAction<>)))
			{
				return false;
			}

			log.RecordCallStatus(call, "Matched on {0}".ToFormat(GetType()));
			return true;
		}

		public IRouteDefinition Build(ActionCall call)
		{
			return new NulloRouteDefinition();
		}
	}
}