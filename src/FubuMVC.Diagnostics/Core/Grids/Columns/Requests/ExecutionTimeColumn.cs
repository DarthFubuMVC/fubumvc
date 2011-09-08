using FubuMVC.Diagnostics.Features.Requests;

namespace FubuMVC.Diagnostics.Core.Grids.Columns.Requests
{
	public class ExecutionTimeColumn : GridColumnBase<RecordedRequestModel>
	{
		public ExecutionTimeColumn()
			: base(r => r.ExecutionTime)
		{
		}

		public override int Rank()
		{
			return 2;
		}
	}
}