using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Policies;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore.Reflection;

namespace FubuMVC.Tests.Runtime
{

    
    
    [TestFixture]
    public class must_be_descriptions_on_important_things
    {

        [Test]
        public void must_be_a_description_on_all_IChainModification_types()
        {
            IEnumerable<Type> types = typeof(FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IChainModification>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_a_description_on_all_IChainFilter_types()
        {
            IEnumerable<Type> types = typeof(FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IChainFilter>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }        
        
        [Test]
        public void must_be_a_description_on_all_IActionSource_types()
        {
            IEnumerable<Type> types = typeof(FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IActionSource>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Test]
        public void must_be_a_description_on_all_IConfigurationAction_types()
        {
            IEnumerable<Type> types = typeof(FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IConfigurationAction>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Test]
        public void must_be_a_description_on_all_IDependency_types()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IDependency>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_a_description_on_all_IUrlPolicy_types()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IUrlPolicy>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }



        [Test]
        public void must_be_a_description_on_all_ValueConverters()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<ValueConverter>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_a_description_on_all_conversion_families()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IConverterFamily>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Test]
        public void must_be_a_description_on_all_media_readers()
        {
            // IMediaWriter<T>
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(
                    x => x.IsConcrete() && x.IsOpenGeneric() && x.GetInterfaces().Any(t => t.Name.Contains("IReader")))
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_a_description_on_all_media_writers()
        {
            // IMediaWriter<T>
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(
                    x =>
                    x.IsConcrete() && x.IsOpenGeneric() && x.GetInterfaces().Any(t => t.Name.Contains("IMediaWriter")))
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_a_description_on_all_model_binders()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IModelBinder>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Test]
        public void must_be_a_description_on_all_property_binders()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IPropertyBinder>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Test]
        public void must_be_some_sort_of_description_on_every_BehaviorNode()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<BehaviorNode>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_some_sort_of_description_on_every_IFormatter()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IFormatter>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_some_sort_of_description_on_every_IRecordedHttpOutput()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IRecordedHttpOutput>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_some_sort_of_description_on_every_LogRecord()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<LogRecord>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }


        [Test]
        public void must_be_some_sort_of_description_on_every_iconverter_family()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IObjectConverterFamily>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_some_sort_of_description_on_every_iconverterstrategy()
        {
            IEnumerable<Type> types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IConverterStrategy>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }
    }
}