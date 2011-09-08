using FubuMVC.Diagnostics.Features.Requests;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Requests
{
	public class UrlColumn : GridColumnBase<RecordedRequestModel>
	{
		public UrlColumn()
			: base(r => r.Url)
		{
		}

		public override int Rank()
		{
			return 4;
		}
	}
}