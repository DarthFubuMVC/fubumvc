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

        public BehaviorGraphGridRowBuilder(IEnumerable<IGridColumnBuilder<BehaviorChain>> columnBuilders)
        {
            _columnBuilders = columnBuilders;
        }

        public IEnumerable<JsonGridRow> RowsFor(BehaviorGraph target)
        {
            return target
                .Behaviors
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