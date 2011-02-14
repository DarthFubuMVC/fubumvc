using System.Reflection;
using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.Querying;
using StructureMap;
using StructureMap.Configuration.DSL;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace FubuFastPack.StructureMap
{
    public class FastPackRegistry : Registry
    {
        public FastPackRegistry()
        {
            For(typeof (IGridRunner<,>)).Use(typeof (GridRunner<,>));
            For<IQueryService>().Use<QueryService>();
            For(typeof (Projection<>)).LifecycleIs(InstanceScope.Unique).Use(typeof(Projection<>));
        }
    }

}