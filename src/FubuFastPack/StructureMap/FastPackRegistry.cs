using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using StructureMap.Configuration.DSL;

namespace FubuFastPack.StructureMap
{
    public class FastPackRegistry : Registry
    {
        public FastPackRegistry()
        {
            For(typeof (IGridRunner<,>)).Use(typeof (GridRunner<,>));
            For<IQueryService>().Use<QueryService>();
        }
    }
}