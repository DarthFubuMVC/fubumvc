using System.Linq;
using FubuMVC.Diagnostics.Features.Requests;
using FubuMVC.Diagnostics.Runtime;

namespace FubuMVC.Diagnostics.Models
{
    public interface IRequestCacheModelBuilder : IModelBuilder<RequestCacheModel>
    {
    }

	public class RequestCacheModelBuilder : IRequestCacheModelBuilder
	{
		private readonly IRequestHistoryCache _requestCache;

		public RequestCacheModelBuilder(IRequestHistoryCache requestCache)
		{
			_requestCache = requestCache;
		}

		public RequestCacheModel Build()
		{
			return new RequestCacheModel
			       	{
			       		Requests = _requestCache
									.RecentReports()
									.Select(r => new RecordedRequestModel
									             	{
														Id = r.Id,
									             		Url = r.Url,
														Time = r.Time,
														FormData = r.FormData,
														ExecutionTime = r.ExecutionTime,
														Steps = r.Steps
									             	})
			       	};
		}
	}
}