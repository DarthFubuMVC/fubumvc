using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Grids.Columns.Requests
{
	public class ExceptionsColumn : GridColumnBase<RecordedRequestModel>
	{
		public ExceptionsColumn()
			: base("Exceptions")
		{
		}

		public override string ValueFor(RecordedRequestModel target)
		{
			return target.Exceptions();
		}
	}
}