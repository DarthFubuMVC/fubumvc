using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Models;

namespace FubuMVC.Diagnostics.Features.Routes
{
    public class RoutesModelBuilder : IModelBuilder<RoutesModel>
    {
        private readonly IEnumerable<IGridColumnBuilder<BehaviorChain>> _columnBuilders;
        private readonly BehaviorGraph _graph;

        public RoutesModelBuilder(IEnumerable<IGridColumnBuilder<BehaviorChain>> columnBuilders, BehaviorGraph graph)
        {
            _columnBuilders = columnBuilders;
            _graph = graph;
        }

        public RoutesModel Build()
        {
            var columnModel = new JqGridColumnModel();
            var chain = _graph.Behaviors.First();
            _columnBuilders
                .SelectMany(builder => builder.ColumnsFor(chain))
                .Each(col => columnModel.AddColumn(new JqGridColumn
                                                       {
                                                           hidden = col.IsHidden,
                                                           hidedlg = col.IsHidden,
                                                           hideFilter = col.HideFilter,
                                                           name = col.Name,
                                                           index = col.Name
                                                       }));

            return new RoutesModel
                       {
                           ColumnModel = columnModel
                       };
        }
    }
}