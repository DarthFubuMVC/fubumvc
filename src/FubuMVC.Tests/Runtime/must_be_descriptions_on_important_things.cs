using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuCore.Descriptions;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Runtime
{
    
    public class must_be_descriptions_on_important_things
    {
        [Fact]
        public void must_be_a_description_on_all_IActionSource_types()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IActionSource>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Fact]
        public void must_be_a_description_on_all_IUrlPolicy_types()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IUrlPolicy>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Fact]
        public void must_be_a_description_on_all_ValueConverters()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<ValueConverter>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Fact]
        public void must_be_a_description_on_all_conversion_families()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IConverterFamily>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Fact]
        public void must_be_a_description_on_all_media_readers()
        {
            // IMediaWriter<T>
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(
                    x => x.IsConcrete() && x.IsOpenGeneric() && x.GetInterfaces().Any(t => t.Name.Contains("IReader")))
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Fact]
        public void must_be_a_description_on_all_media_writers()
        {
            // IMediaWriter<T>
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(
                    x =>
                        x.IsConcrete() && x.IsOpenGeneric() &&
                        x.GetInterfaces().Any(t => t.Name.Contains("IMediaWriter")))
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Fact]
        public void must_be_a_description_on_all_model_binders()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IModelBinder>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Fact]
        public void must_be_a_description_on_all_property_binders()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IPropertyBinder>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Fact]
        public void must_be_some_sort_of_description_on_every_BehaviorNode()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<BehaviorNode>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Fact]
        public void must_be_some_sort_of_description_on_every_IFormatter()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IFormatter>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Fact]
        public void must_be_some_sort_of_description_on_every_IRecordedHttpOutput()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IRecordedHttpOutput>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Fact]
        public void must_be_some_sort_of_description_on_every_iconverter_family()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IObjectConverterFamily>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Fact]
        public void must_be_some_sort_of_description_on_every_iconverterstrategy()
        {
            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IConverterStrategy>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }
    }
}