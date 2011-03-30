using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Grids.Columns.Requests
{
	public class ExecutionTimeColumn : GridColumnBase<RecordedRequestModel>
	{
		public ExecutionTimeColumn()
			: base(r => r.ExecutionTime)
		{
		}
	}
}