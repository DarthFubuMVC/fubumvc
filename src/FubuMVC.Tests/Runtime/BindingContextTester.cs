using System;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.StructureMap;
using FubuMVC.Tests.UI;
using FubuTestingSupport;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using InMemoryRequestData=FubuMVC.Core.Runtime.InMemoryRequestData;

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

            var smartRequest = MockRepository.GenerateMock<ISmartRequest>();
            smartRequest.Stub(x => x.PrefixedWith(null)).Return(smartRequest).IgnoreArguments();
            locator.Stub(x => x.GetInstance<ISmartRequest>()).Return(smartRequest);

            context = new BindingContext(request, locator);
        }

        [Test]
        public void get_regular_property()
        {
            request["Address1"] = "2035 Ozark";
            var property = ReflectionHelper.GetProperty<Address>(x => x.Address1);

            bool wasCalled = false;
            context.ForProperty(property, x =>
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

            context.ForProperty(property, x =>
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
            prefixed.ForProperty(property, x =>
            {
                x.PropertyValue.ShouldEqual(request["AddressAddress1"]);


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
            //taken out ' is not a valid value for Int32' to support non english culture tests
            problem.ExceptionText.ShouldContain("NOT A NUMBER");
        }
    }

    [TestFixture]
    public class value_of_scenarios : InteractionContext<BindingContext>
    {
        private StubSmartRequest theSmartRequest;
        private InMemoryRequestData theRawRequest;

        protected override void beforeEach()
        {
            theRawRequest = new InMemoryRequestData();
            Services.Inject<IRequestData>(theRawRequest);

            theSmartRequest = new StubSmartRequest();
            Services.Inject<ISmartRequest>(theSmartRequest);
            Services.Inject<IServiceLocator>(new StructureMapServiceLocator(Services.Container));
        }

        [Test]
        public void value_as_t_by_name()
        {
            var theValue = Guid.NewGuid();
            var theKey = "some key";
            theSmartRequest[theKey] = theValue;

            ClassUnderTest.As<IBindingContext>().ValueAs<Guid>(theKey).ShouldEqual(theValue);
        }

        [Test]
        public void value_as_t_by_name_with_a_naming_strategy()
        {
            var theValue = Guid.NewGuid();
            var theKey = "some key";
            theSmartRequest["[" + theKey + "]"] = theValue;

            ClassUnderTest.As<IBindingContext>().ValueAs<Guid>(theKey).ShouldEqual(theValue);
        }


        [Test]
        public void value_as_t_by_name_with_continuation()
        {
            var action = MockRepository.GenerateMock<Action<Guid>>();
            var theKey = "some key";
            var theValue = Guid.NewGuid();

            theSmartRequest[theKey] = theValue;

            ClassUnderTest.As<IBindingContext>().ValueAs(theKey, action).ShouldBeTrue();
        
            action.AssertWasCalled(x => x.Invoke(theValue));
        }

        [Test]
        public void value_as_t_from_property_info()
        {
            var property = ReflectionHelper.GetProperty<ClassThatIsHeld>(x => x.Name);

            var theValue = Guid.NewGuid();
            theSmartRequest["Name"] = theValue;


            ClassUnderTest.ForProperty(property, context =>
            {
                context.ValueAs<Guid>().ShouldEqual(theValue);
            });
        }

        [Test]
        public void value_as_t_from_property_info_with_naming_strategy()
        {
            var property = ReflectionHelper.GetProperty<ClassThatIsHeld>(x => x.Name);

            var theValue = Guid.NewGuid();
            theSmartRequest["[Name]"] = theValue;


            ClassUnderTest.ForProperty(property, context =>
            {
                context.ValueAs<Guid>().ShouldEqual(theValue);
            });
        }

        [Test]
        public void value_as_t_from_property_by_continuation()
        {
            var property = ReflectionHelper.GetProperty<ClassThatIsHeld>(x => x.Name);

            var theValue = Guid.NewGuid().ToString();
            theSmartRequest["[Name]"] = theValue;
            theRawRequest["[Name]"] = theValue;

            var action = MockRepository.GenerateMock<Action<string>>();

            ClassUnderTest.ForProperty(property, context =>
            {
                context.ValueAs<string>(action).ShouldBeTrue();
            });

            action.AssertWasCalled(x => x.Invoke(theValue));
        }
    }

    public class StubSmartRequest : ISmartRequest
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public object this[string key]
        {
            get
            {
                return _values[key];
            }
            set
            {
                _values[key] = value;
            }
        }

        public object Value(Type type, string key)
        {
            throw new NotImplementedException();
        }

        public bool Value(Type type, string key, Action<object> continuation)
        {
            throw new NotImplementedException();
        }

        public T Value<T>(string key)
        {
            return (T) _values[key];
        }

        public bool Value<T>(string key, Action<T> callback)
        {
            if (!_values.Has(key))
            {
                return false;
            }

            callback((T) _values[key]);

            return true;
        }

        public ISmartRequest PrefixedWith(string prefix)
        {
            throw new NotImplementedException();
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