using System;
using FubuMVC.Core.UI.Bootstrap.Tags;

namespace FubuMVC.Core.Diagnostics.Visualization
{
    public static class DetailsTableBuilderExtensions
    {
        public static void AddDetail(this DetailTableBuilder builder, string label, Type type)
        {
            if (type == null) return;

            builder.AddDetailByPartial(label, new TypeInput{Type = type});
        }
    }
}