using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Infrastructure;

namespace FubuMVC.Diagnostics.Features.Chains.View
{
	public class get_Id_handler
	{
	    private readonly IChainVisualizerBuilder _visualizer;

		public get_Id_handler(IChainVisualizerBuilder visualizer)
		{
		    _visualizer = visualizer;
		}

	    public ChainModel Execute(ChainRequest request)
	    {
	        var visualizer = _visualizer.VisualizerFor(request.Id);
			if(visualizer == null)
			{
                throw new ArgumentException("{0} does not exist".ToFormat(request.Id));
			}

	        return visualizer;
	    }
	}
}