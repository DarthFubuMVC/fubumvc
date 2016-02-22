using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Validation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    [TestFixture]
    public class ValidationContextTester
    {
        private ValidationContextTarget theTarget;
        private ValidationContext theContext;

        [SetUp]
        public void SetUp()
        {
            theTarget = new ValidationContextTarget(){
                Name = "Jeremy",
                Number = 25
            };

            theContext = new ValidationContext(null, new Notification(), theTarget);
        }

        private T get<T>(Expression<Func<ValidationContextTarget, object>> property)
        {
            var accessor = ReflectionHelper.GetAccessor(property);
            return theContext.GetFieldValue<T>(accessor);
        }

        [Test]
        public void get_value_when_the_type_is_correct()
        {
            get<string>(x => x.Name).ShouldBe("Jeremy");
        }

        [Test]
        public void get_value_when_the_type_matches_2()
        {
            get<int>(x => x.Number).ShouldBe(25);
        }

        [Test]
        public void services_defaults_to_in_memory()
        {
            theContext.ServiceLocator.ShouldBeOfType<InMemoryServiceLocator>();
        }

        [Test]
        public void gets_the_service()
        {
            var theServices = new InMemoryServiceLocator();
            var theService = new TestService();
            theServices.Add(theService);

            theContext.ServiceLocator = theServices;

            theContext.Service<TestService>().ShouldBeTheSameAs(theService);
        }

        public class TestService
        {
        }
    }

    public class ValidationContextTarget
    {
        public string Name { get; set; }
        public int Number { get; set; }
        
    }
}