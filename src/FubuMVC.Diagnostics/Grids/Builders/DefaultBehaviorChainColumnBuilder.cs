using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids.Builders
{
    public class DefaultBehaviorChainColumnBuilder : IGridColumnBuilder<BehaviorChain>
    {
    	private readonly IEnumerable<IBehaviorChainColumn> _columns;

    	public DefaultBehaviorChainColumnBuilder(IEnumerable<IBehaviorChainColumn> columns)
    	{
    		_columns = columns;
    	}

    	public IEnumerable<JsonGridColumn> ColumnsFor(BehaviorChain target)
    	{
    		return _columns.Select(col => new JsonGridColumn
    		                              	{
												Name = col.Name(),
												Value = col.ValueFor(target),
												IsHidden = col.IsHidden(target),
												HideFilter = col.HideFilter(target),
												IsIdentifier = col.IsIdentifier()
    		                              	});
        }
    }
}