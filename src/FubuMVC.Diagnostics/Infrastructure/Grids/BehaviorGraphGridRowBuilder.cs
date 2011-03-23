using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids
{
    public class BehaviorGraphGridRowBuilder : IGridRowBuilder<BehaviorGraph>
    {
        private readonly IEnumerable<IGridColumnBuilder<BehaviorChain>> _columnBuilders;
        private readonly IEnumerable<IGridFilter<BehaviorChain>> _filters;

        public BehaviorGraphGridRowBuilder(IEnumerable<IGridColumnBuilder<BehaviorChain>> columnBuilders, IEnumerable<IGridFilter<BehaviorChain>> filters)
        {
            _columnBuilders = columnBuilders;
            _filters = filters;
        }

        public IEnumerable<JsonGridRow> RowsFor(BehaviorGraph target, IEnumerable<JsonGridFilter> filters)
        {
            return target
                .Behaviors
                .Where(chain => _filters.Matches(filters, chain))
                .Select(chain =>
                            {
                                var columns = _columnBuilders
                                    .SelectMany(builder => builder.ColumnsFor(chain))
                                    .ToList();

                                var idCol = columns.FirstOrDefault(col => col.IsIdentifier);
                                if (idCol == null)
                                {
                                    throw new DiagnosticsException(1001,
                                                                   "No Identifier Column specified for BehaviorChain.");
                                }

                                return new JsonGridRow
                                           {
                                               Id = idCol.Value,
                                               Columns = columns
                                           };
                            });
        }
    }
}