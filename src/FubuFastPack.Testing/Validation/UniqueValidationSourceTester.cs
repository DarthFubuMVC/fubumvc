using System.Reflection;
using FubuCore.Reflection;
using FubuFastPack.Domain;
using FubuFastPack.Validation;
using FubuTestingSupport;
using FubuValidation;
using NUnit.Framework;
using System.Linq;

namespace FubuFastPack.Testing.Validation
{
    [TestFixture]
    public class UniqueValidationSourceTester : InteractionContext<UniqueValidationSource>
    {
        protected override void beforeEach()
        {
            Services.Inject<ITypeDescriptorCache>(new TypeDescriptorCache());
            
        }

        [Test]
        public void should_find_no_rules_for_a_class_with_no_unique_rules()
        {
            // Doing repeated calls to get the caching into play
            ClassUnderTest.RulesFor(typeof(NoUniqueClass)).Any().ShouldBeFalse();
            ClassUnderTest.RulesFor(typeof(NoUniqueClass)).Any().ShouldBeFalse();
            ClassUnderTest.RulesFor(typeof(NoUniqueClass)).Any().ShouldBeFalse();
        }

        [Test]
        public void finds_one_rule_for_a_simple_unique()
        {
            ClassUnderTest.RulesFor(typeof(SimpleUniqueClass)).Single()
                .ShouldBeOfType<UniqueValidationRule>().Properties.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("Identifier");
        }

        [Test]
        public void find_multiple_rules_for_a_complex_unique_marked_class()
        {
            var rules = ClassUnderTest.DetermineRulesFor(typeof (ComplicatedUniqueClass)).OfType<UniqueValidationRule>();
            rules.Count().ShouldEqual(3);

            rules.First(x => x.Key == "A").Properties.Select(x => x.Name).ShouldHaveTheSameElementsAs("One", "Two");
            rules.First(x => x.Key == "B").Properties.Select(x => x.Name).ShouldHaveTheSameElementsAs("Three", "Four");
            rules.First(x => x.Key == null).Properties.Select(x => x.Name).ShouldHaveTheSameElementsAs("Five");
        }
    }

    public class UniqueValidationRuleTester
    {
        private PropertyInfo propertyOne;
        private PropertyInfo propertyTwo;
        private UniqueValidationRule theRule;
        private ValidationContext theContext;

        [SetUp]
        public void SetUp()
        {
            propertyOne = ReflectionHelper.GetProperty<ComplicatedUniqueClass>(x => x.One);
            propertyTwo = ReflectionHelper.GetProperty<ComplicatedUniqueClass>(x => x.Two);
            theRule = new UniqueValidationRule("A", typeof (ComplicatedUniqueClass), null,
                                               new[]{propertyOne, propertyTwo});

            theContext = new ValidationContext(null, new Notification(), typeof (ComplicatedUniqueClass));
        }

        [Test]
        public void validation_when_the_count_is_zero_should_do_nothing()
        {
            theRule.Validate(0, theContext);
            theContext.Notification.IsValid().ShouldBeTrue();
        }

        [Test]
        public void validation_when_the_count_is_more_than_zero_should_trigger_a_validation_failure()
        {
            theRule.Validate(1, theContext);

            var message = theContext.Notification.AllMessages.Single();
            message.StringToken.ShouldEqual(FastPackKeys.FIELD_MUST_BE_UNIQUE);
            message.Accessors.Select(x => x.Name).ShouldHaveTheSameElementsAs("One", "Two");
        }
    }
    
    public class NoUniqueClass
    {
        
    }

    public class SimpleUniqueClass : DomainEntity
    {
        [Unique]
        public string Identifier { get; set; }
    }

    public class ComplicatedUniqueClass : DomainEntity
    {
        [Unique(Key ="A")]
        public string One { get; set; }

        [Unique(Key = "A")]
        public string Two { get; set; }

        [Unique(Key = "B")]
        public string Three { get; set; }

        [Unique(Key = "B")]
        public string Four { get; set; }

        [Unique]
        public string Five { get; set; }
    }
}