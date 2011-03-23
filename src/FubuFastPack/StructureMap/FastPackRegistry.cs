using FubuFastPack.JqGrid;
using FubuFastPack.NHibernate;
using FubuFastPack.Querying;
using FubuFastPack.Validation;
using FubuValidation;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuFastPack.StructureMap
{
    public class FastPackRegistry : Registry
    {
        public FastPackRegistry()
        {
            For(typeof (IGridRunner<,>)).Use(typeof (GridRunner<,>));
            For<IQueryService>().Use<QueryService>();
            For(typeof (Projection<>)).LifecycleIs(InstanceScope.Unique).Use(typeof (Projection<>));

            For<IValidationProvider>().Use<ValidationProvider>();
            For<IValidationSource>().Add<UniqueValidationSource>();
        }
    }
}