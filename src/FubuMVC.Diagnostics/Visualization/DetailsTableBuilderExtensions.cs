using System;
using FubuMVC.Core.UI.Bootstrap.Tags;
using FubuMVC.Diagnostics.Shared;

namespace FubuMVC.Diagnostics.Visualization
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