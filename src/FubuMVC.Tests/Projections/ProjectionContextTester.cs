using FubuCore;
using FubuCore.Formatting;
using FubuCore.Reflection;
using FubuMVC.Core.Projections;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;
using Rhino.Mocks;
using StructureMap.TypeRules;

namespace FubuMVC.Tests.Projections
{
    
    public class ProjectionContextTester : InteractionContext<ProjectionContext<ProjectionModel>>
    {
        [Fact]
        public void subject_delegates_to_the_inner_values()
        {
            var model = new ProjectionModel();
            MockFor<IValues<ProjectionModel>>().Stub(x => x.Subject).Return(model);


            ClassUnderTest.Subject.ShouldBeTheSameAs(model);
        }

        [Fact]
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

            ClassUnderTest.FormattedValueOf(accessor).ShouldBe(theFormattedValue);
            ClassUnderTest.FormattedValueOf(x => x.Name).ShouldBe(theFormattedValue);
        }



        [Fact]
        public void value_for_delegates_to_the_inner_values()
        {
            var accessor = ReflectionHelper.GetAccessor<ProjectionModel>(x => x.Name);
            MockFor<IValues<ProjectionModel>>().Stub(x => x.ValueFor(accessor))
                .Return("Jeremy");

            ClassUnderTest.Values.ValueFor(accessor).ShouldBe("Jeremy");
        }

        [Fact]
        public void getting_a_service_delegates_to_the_service_locator()
        {
            var stub = new StubUrlRegistry();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IUrlRegistry>()).Return(stub);

            ClassUnderTest.Service<IUrlRegistry>().ShouldBeTheSameAs(stub);
        }

        [Fact]
        public void urls_are_pulled_from_the_service_locator_but_only_once()
        {
            var stub = new StubUrlRegistry();
            MockFor<IServiceLocator>().Stub(x => x.GetInstance<IUrlRegistry>()).Return(stub);

            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);
            ClassUnderTest.Urls.ShouldBeTheSameAs(stub);

            MockFor<IServiceLocator>().AssertWasCalled(x => x.GetInstance<IUrlRegistry>(), x => x.Repeat.Once());
        }

        [Fact]
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

        [Fact]
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