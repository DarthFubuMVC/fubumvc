using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuTestingSupport;
using FubuValidation;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
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
            get<string>(x => x.Name).ShouldEqual("Jeremy");
        }

        [Test]
        public void get_value_when_the_type_matches_2()
        {
            get<int>(x => x.Number).ShouldEqual(25);
        }


    }

    public class ValidationContextTarget
    {
        public string Name { get; set; }
        public int Number { get; set; }
        
    }
}