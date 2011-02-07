using System.Reflection;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using StructureMap.Configuration.DSL;
using System.Collections.Generic;

namespace FubuFastPack.StructureMap
{
    public class FastPackRegistry : Registry
    {
        public FastPackRegistry(params Assembly[] gridAssemblies)
        {
            For(typeof (IGridRunner<,>)).Use(typeof (GridRunner<,>));
            For<IQueryService>().Use<QueryService>();

            Scan(x =>
            {
                gridAssemblies.Each(x.Assembly);
                x.AddAllTypesOf<ISmartGrid>().NameBy(type => type.NameForGrid());
            });
        }
    }

}