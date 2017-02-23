using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Validation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation
{
    
    public class ValidationContextTester
    {
        private ValidationContextTarget theTarget;
        private ValidationContext theContext;

        public ValidationContextTester()
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

        [Fact]
        public void get_value_when_the_type_is_correct()
        {
            get<string>(x => x.Name).ShouldBe("Jeremy");
        }

        [Fact]
        public void get_value_when_the_type_matches_2()
        {
            get<int>(x => x.Number).ShouldBe(25);
        }

        [Fact]
        public void services_defaults_to_in_memory()
        {
            theContext.ServiceLocator.ShouldBeOfType<InMemoryServiceLocator>();
        }

        [Fact]
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