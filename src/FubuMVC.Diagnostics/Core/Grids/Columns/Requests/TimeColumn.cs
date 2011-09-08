using FubuMVC.Diagnostics.Features.Requests;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Requests
{
	public class TimeColumn : GridColumnBase<RecordedRequestModel>
	{
		public TimeColumn()
			: base(r => r.Time)
		{
		}

		public override int Rank()
		{
			return 5;
		}
	}
}