using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Grids.Columns.Requests
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