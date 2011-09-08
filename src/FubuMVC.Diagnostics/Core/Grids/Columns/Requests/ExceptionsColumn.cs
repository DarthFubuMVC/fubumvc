using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Requests
{
	public class ExceptionsColumn : GridColumnBase<RecordedRequestModel>
	{
		public ExceptionsColumn()
			: base("Exceptions")
		{
		}

		public override int Rank()
		{
			return 1;
		}

		public override string ValueFor(RecordedRequestModel target)
		{
			return target.Exceptions();
		}
	}
}