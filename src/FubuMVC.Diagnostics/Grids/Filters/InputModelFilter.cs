using FubuMVC.Diagnostics.Grids.Columns;

namespace FubuMVC.Diagnostics.Grids.Filters
{
    public class InputModelFilter : BehaviorChainFilterBase<InputModelColumn>
    {
        public InputModelFilter(InputModelColumn column)
			: base(column)
        {
        }
    }
}