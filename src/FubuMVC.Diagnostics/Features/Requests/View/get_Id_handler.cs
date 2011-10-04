using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Features.Chains.View;

namespace FubuMVC.Diagnostics.Features.Requests.View
{
	public class get_Id_handler
	{
		private readonly IRequestHistoryCache _requestCache;
	    private readonly IChainVisualizerBuilder _chainVisualizer;

		public get_Id_handler(IRequestHistoryCache requestCache, IChainVisualizerBuilder chainVisualizer)
		{
		    _requestCache = requestCache;
		    _chainVisualizer = chainVisualizer;
		}

	    public RequestDetailsModel Execute(RecordedRequestRequestModel request)
		{
			var report = _requestCache.RecentReports().SingleOrDefault(r => r.Id == request.Id);
			if(report == null)
			{
			    throw new ArgumentException("{0} does not exist".ToFormat(request.Id));
			}

		    var model = new RequestDetailsModel
		                    {
		                        Report = report,
		                        Root = Gather(report),
                                Chain = _chainVisualizer.VisualizerFor(report.BehaviorId),
		                        Logs = report
		                            .Steps
		                            .Where(s => s.Details is RequestLogEntry)
		                            .Select(s => s.Details)
		                            .Cast<RequestLogEntry>()
		                            .ToList()
		                    };
			return model;
		}

		public BehaviorDetailsModel Gather(IDebugReport report)
		{
			// TODO -- come back and clean this up. Just getting it up and running for a demo
			BehaviorDetailsModel root = null;
			var behaviors = new Cache<Type, BehaviorDetailsModel>(t =>
			                                                      	{
			                                                      		var model = new BehaviorDetailsModel {BehaviorType = t};
																		if(root == null)
																		{
																			root = model;
																		}

			                                                      		return model;
			                                                      	});
			Type lastBehaviorType = null;
			report
				.Steps
				.Each(s =>
				      	{
				      		var behaviorType = s.Behavior.BehaviorType;
				      	    var isSameBehavior = behaviorType == lastBehaviorType;
				      	    var isBehaviorFinish = s.Details.GetType().CanBeCastTo<BehaviorFinish>();

                            if (behaviors.Has(behaviorType) && (!isSameBehavior || isBehaviorFinish))
							{
								behaviors[behaviorType].AddAfter(s.Details);
							}
							else
							{
								behaviors[behaviorType].AddBefore(s.Details);
							}

                            var currentBehavior = behaviors[behaviorType];
				      	    currentBehavior.Id = s.Behavior.BehaviorId;

                            if (lastBehaviorType != null && !isSameBehavior && isBehaviorFinish)
							{   
								var lastBehavior = behaviors[lastBehaviorType];
								if(!lastBehavior.Equals(root))
								{
                                    currentBehavior.Inner = lastBehavior;
								}
							}

							lastBehaviorType = behaviorType;
				      	});

			return root;
		}
	}
}