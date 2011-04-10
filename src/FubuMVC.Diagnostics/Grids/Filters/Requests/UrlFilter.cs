using FubuMVC.Diagnostics.Grids.Columns.Requests;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Grids.Filters.Requests
{
	public class UrlFilter : GridFilterBase<UrlColumn, RecordedRequestModel>
	{
		public UrlFilter(UrlColumn column) 
			: base(column)
		{
		}
	}

	public class StatusFilter : GridFilterBase<StatusColumn, RecordedRequestModel>
	{
		public StatusFilter(StatusColumn column)
			: base(column)
		{
		}
	}

	public class TimeFilter : GridFilterBase<TimeColumn, RecordedRequestModel>
	{
		public TimeFilter(TimeColumn column)
			: base(column)
		{
		}
	}
}