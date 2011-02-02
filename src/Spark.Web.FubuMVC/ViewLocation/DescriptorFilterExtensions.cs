using System;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public static class DescriptorFilterExtensions
    {
        public static void AddFilter(this ISparkServiceContainer target, IDescriptorFilter filter)
        {
            target.GetService<IDescriptorBuilder>().AddFilter(filter);
        }

        public static void AddFilter(this ISparkViewFactory target, IDescriptorFilter filter)
        {
            target.DescriptorBuilder.AddFilter(filter);
        }

        public static void AddFilter(this IDescriptorBuilder target, IDescriptorFilter filter)
        {
            if (!(target is FubuDescriptorBuilder))
                throw new InvalidCastException("IDescriptorFilters may only be added to FubuDescriptorBuilder");

            ((FubuDescriptorBuilder) target).AddFilter(filter);
        }

        public static void AddFilter(this FubuDescriptorBuilder target, IDescriptorFilter filter)
        {
            target.Filters.Add(filter);
        }
    }
}