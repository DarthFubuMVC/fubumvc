using System.Diagnostics;
using FubuCore;
using FubuCore.Formatting;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.TypeRules;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class ProjectionContextTester : InteractionContext<ProjectionContext<ProjectionModel>> 
    {



        [Test]
        public void subject_delegates_to_the_inner_values()
        {
            var model = new ProjectionModel();
            MockFor<IValues<ProjectionModel>>().Stub(x => x.Subject).Return(model);

            

            ClassUnderTest.Subject.ShouldBeTheSameAs(model);
        }

        [Test]
        public void formatted_value_of_extension_method_1()
        {
            var formatter = MockFor<IDisplayFormatter>();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IDisplayFormatter>()).Return(formatter);

            var accessor = ReflectionHelper.GetAccessor<ProjectionModel>(x => x.Name);

            var theRawValue = "Jeremy";
            MockFor<IValues<ProjectionModel>>().Stub(x => x.ValueFor(accessor))
                .Return(theRawValue);

            var theFormattedValue = "*Jeremy*";
            formatter.Stub(x => x.GetDisplayForValue(accessor, theRawValue)).Return(theFormattedValue);

            ClassUnderTest.FormattedValueOf(accessor).ShouldEqual(theFormattedValue);
            ClassUnderTest.FormattedValueOf(x => x.Name).ShouldEqual(theFormattedValue);
        }

        [Test]
        public void try_the_casting_logic()
        {
            var contextType = typeof (ProjectionContext<ProjectionModel>);
            var valuesType = typeof (IValues<ProjectionModel>);

            var cInfo = contextType.GetTypeInfo();
            var vInfo = valuesType.GetTypeInfo();
            vInfo.IsAssignableFrom(cInfo).ShouldBeFalse();


//                    public PluginFamily Build(Type type)
//        {
//            if (!type.GetTypeInfo().IsGenericType) return null;
//
//            var basicType = type.GetGenericTypeDefinition();
//            if (!_graph.Families.Has(basicType))
//            {
//
//                return _graph.Families.ToArray().FirstOrDefault(x => type.GetTypeInfo().IsAssignableFrom(x.PluginType.GetTypeInfo()));
//            }
//
//            var basicFamily = _graph.Families[basicType];
//            var templatedParameterTypes = type.GetGenericArguments();
//
//            return basicFamily.CreateTemplatedClone(templatedParameterTypes.ToArray());
//        }
        }

        [Test]
        public void value_for_delegates_to_the_inner_values()
        {
            var accessor = ReflectionHelper.GetAccessor<ProjectionModel>(x => x.Name);
            MockFor<IValues<ProjectionModel>>().Stub(x => x.ValueFor(accessor))
                .Return("Jeremy");

            ClassUnderTest.Values.ValueFor(accessor).ShouldEqual("Jeremy");
        }

        [Test]
        public void getting_a_service_delegates_to_the_service_locator()
        {
            var stub = new StubUrlRegistry();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IUrlRegistry>()).Return(stub);

            ClassUnderTest.Service<IUrlRegistry>().ShouldBeTheSameAs(stub);
        }

        [Test]
        public void urls_are_pulled_from_the_service_locator_but_only_once()
        {
            var stub = new StubUrlRegistry();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IUrlRegistry>()).Return(stub);

            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
        
            MockFor<IServiceLocator>().AssertWasCalled(x => x.GetInstance<IUrlRegistry>(), x=> x.Repeat.Once());
        }

        [Test]
        public void display_formatter_is_pulled_from_the_service_locator_once()
        {
            var formatter = MockFor<IDisplayFormatter>();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IDisplayFormatter>()).Return(formatter);

            ClassUnderTest.Formatter.ShouldBeTheSameAs(formatter);
            ClassUnderTest.Formatter.ShouldBeTheSameAs(formatter);
            ClassUnderTest.Formatter.ShouldBeTheSameAs(formatter);
            ClassUnderTest.Formatter.ShouldBeTheSameAs(formatter);
            ClassUnderTest.Formatter.ShouldBeTheSameAs(formatter);

            MockFor<IServiceLocator>().AssertWasCalled(x => x.GetInstance<IDisplayFormatter>(), x => x.Repeat.Once()); 
        }

        [Test]
        public void create_a_context_for_a_different_type()
        {
            var formatter = MockFor<IDisplayFormatter>();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IDisplayFormatter>()).Return(formatter);

            var stub = new StubUrlRegistry();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IUrlRegistry>()).Return(stub);

            var different = new DifferentTarget();

            var context = ClassUnderTest.ContextFor(different);

            context.Urls.ShouldBeTheSameAs(stub);
            context.Formatter.ShouldBeTheSameAs(formatter);
            context.Subject.ShouldBeTheSameAs(different);
        }


    }


    public class ProjectionModel
    {
        public string Name { get; set; }
    }

    public class DifferentTarget
    {

    }
}