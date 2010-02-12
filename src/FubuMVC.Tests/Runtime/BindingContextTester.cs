using System;
using FubuMVC.Core.Models;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Util;
using FubuMVC.StructureMap;
using FubuMVC.Tests.UI;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Runtime
{
    [TestFixture]
    public class BindingContextTester
    {
        private InMemoryRequestData request;
        private IServiceLocator locator;
        private BindingContext context;

        [SetUp]
        public void SetUp()
        {
            request = new InMemoryRequestData();
            locator = MockRepository.GenerateMock<IServiceLocator>();

            context = new BindingContext(request, locator);
        }

        [Test]
        public void get_regular_property()
        {
            request["Address1"] = "2035 Ozark";
            var property = ReflectionHelper.GetProperty<Address>(x => x.Address1);

            bool wasCalled = false;
            context.ForProperty(property, () =>
            {
                context.PropertyValue.ShouldEqual(request["Address1"]);

                wasCalled = true;
            });

            wasCalled.ShouldBeTrue();
        }

        [Test]
        public void get_property_that_falls_through_to_the_second_naming_strategy()
        {
            request["User-Agent"] = "hank";
            var property = ReflectionHelper.GetProperty<State>(x => x.User_Agent);

            bool wasCalled = false;

            context.ForProperty(property, () =>
            {
                context.PropertyValue.ShouldEqual("hank");
                wasCalled = true;
            });

            wasCalled.ShouldBeTrue();
        }

        [Test]
        public void prefix_with_returns_a_working_binding_context()
        {
            request["AddressAddress1"] = "479 SW 85th St";
            var property = ReflectionHelper.GetProperty<Address>(x => x.Address1);

            bool wasCalled = false;
            
            context.StartObject(new Address());
            IBindingContext prefixed = context.PrefixWith("Address");
            prefixed.ForProperty(property, () =>
            {
                prefixed.PropertyValue.ShouldEqual(request["AddressAddress1"]);


                wasCalled = true;
            });

            context.Problems.Any().ShouldBeFalse();

            wasCalled.ShouldBeTrue();                
        }

        public class State
        {
            public string User_Agent { get; set; }
        }
    }

    /// <summary>
    /// This is an integration test
    /// </summary>
    [TestFixture]
    public class when_binding_a_child_property_with_valid_data
    {
        private InMemoryRequestData data;
        private BindingContext context;
        private HolderClass holder;

        [SetUp]
        public void SetUp()
        {
            var container = StructureMapContainerFacility.GetBasicFubuContainer();

            data = new InMemoryRequestData();
            container.Inject<IRequestData>(data);

            context = container.GetInstance<BindingContext>();
            holder = new HolderClass();

            data["HeldClassName"] = "Jeremy";
            data["HeldClassAge"] = "36";
        
            context.StartObject(holder);

            var property = ReflectionHelper.GetProperty<HolderClass>(x => x.HeldClass);

            context.BindChild(property);
        }

        [Test]
        public void the_child_property_is_filled()
        {
            holder.HeldClass.ShouldNotBeNull();
        }

        [Test]
        public void the_properties_of_the_child_object_are_filled_in_from_the_request_data()
        {
            holder.HeldClass.Name.ShouldEqual("Jeremy");
            holder.HeldClass.Age.ShouldEqual(36);
        }

        [Test]
        public void should_be_no_problems_recorded()
        {
            context.Problems.Any().ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_binding_a_child_with_supplied_overrides_and_all_valid_data
    {
        private InMemoryRequestData data;
        private BindingContext context;
        private HolderClass holder;

        [SetUp]
        public void SetUp()
        {
            var container = StructureMapContainerFacility.GetBasicFubuContainer();

            data = new InMemoryRequestData();
            container.Inject<IRequestData>(data);

            context = container.GetInstance<BindingContext>();
            holder = new HolderClass();

            data["SpecialName"] = "Jeremy";
            data["SpecialAge"] = "36";
            data["SpecialColor"] = "red";

            context.StartObject(holder);

            var property = ReflectionHelper.GetProperty<HolderClass>(x => x.HeldClass);

            context.BindChild(property, typeof(SpecialClassThatIsHeld), "Special");
        }

        [Test]
        public void should_be_no_problems_recorded()
        {
            context.Problems.Any().ShouldBeFalse();
        }

        [Test]
        public void set_the_special_object_on_the_property_with_data()
        {
            var held = holder.HeldClass.ShouldBeOfType<SpecialClassThatIsHeld>();
            held.Name.ShouldEqual("Jeremy");
            held.Age.ShouldEqual(36);
            held.Color.ShouldEqual("red");
        }
    }

    [TestFixture]
    public class when_binding_a_child_object_that_is_rejected_by_the_parent_object
    {
        private InMemoryRequestData data;
        private BindingContext context;
        private HolderClass holder;

        [SetUp]
        public void SetUp()
        {
            var container = StructureMapContainerFacility.GetBasicFubuContainer();

            data = new InMemoryRequestData();
            container.Inject<IRequestData>(data);

            context = container.GetInstance<BindingContext>();
            holder = new HolderClass();

            data["SpecialName"] = "Jeremy";
            data["SpecialAge"] = "36";

            context.StartObject(holder);

            var property = ReflectionHelper.GetProperty<HolderClass>(x => x.HeldClass);

            context.BindChild(property, typeof(HeldClassThatGetsRejected), "Special");
        }

        [Test]
        public void should_be_a_single_conversion_problem()
        {
            var problem = context.Problems.Single();

            problem.PropertyName().ShouldEqual("HeldClass");
            problem.Item.ShouldEqual(holder);
            problem.ExceptionText.ShouldContain("the exception message");
        }
    }

    [TestFixture]
    public class when_binding_a_child_object_that_has_some_invalid_data
    {
        private InMemoryRequestData data;
        private BindingContext context;
        private HolderClass holder;

        [SetUp]
        public void SetUp()
        {
            var container = StructureMapContainerFacility.GetBasicFubuContainer();

            data = new InMemoryRequestData();
            container.Inject<IRequestData>(data);

            context = container.GetInstance<BindingContext>();
            holder = new HolderClass();

            data["HeldClassName"] = "Jeremy";
            data["HeldClassAge"] = "NOT A NUMBER";

            context.StartObject(holder);

            var property = ReflectionHelper.GetProperty<HolderClass>(x => x.HeldClass);

            context.BindChild(property);
        }

        [Test]
        public void the_child_property_is_filled()
        {
            holder.HeldClass.ShouldNotBeNull();
        }

        [Test]
        public void the_properties_of_the_child_object_are_filled_in_from_the_request_data()
        {
            holder.HeldClass.Name.ShouldEqual("Jeremy");

        }

        [Test]
        public void should_be_one_problems_recorded()
        {
            var problem = context.Problems.Single();
            problem.PropertyName().ShouldEqual("HeldClass.Age");
            problem.ExceptionText.ShouldContain("NOT A NUMBER is not a valid value for Int32");
        }
    }

    public class ClassThatIsHeld
    {
        public string Name { get; set;}
        public int Age { get; set;}
        public bool Active { get; set;}
    }

    public class SpecialClassThatIsHeld : ClassThatIsHeld
    {
        public string Color { get; set; }
    }

    public class HeldClassThatGetsRejected : ClassThatIsHeld
    {
        
    }

    public class HolderClass
    {
        private ClassThatIsHeld _heldClass;
        public ClassThatIsHeld HeldClass
        {
            get { return _heldClass; } 
            set
            {
                if (value is HeldClassThatGetsRejected)
                {
                    throw new InvalidCastException("the exception message");
                }
                _heldClass = value;
            }
        }
    }
}