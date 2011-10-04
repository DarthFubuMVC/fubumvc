using System;
using FubuCore;

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