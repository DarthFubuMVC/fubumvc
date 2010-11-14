using FubuMVC.Core.UI.Configuration;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class LambdaElementModifierTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            tagModifier = (r, t) => { };
            modifier = new LambdaElementModifier(def => def.Accessor.PropertyType == typeof (string), def => tagModifier);
        }

        #endregion

        private TagModifier tagModifier;
        private LambdaElementModifier modifier;

        public class ConventionTarget
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Test]
        public void return_a_value_if_the_filter_matches_the_accessor()
        {
            AccessorDef def = AccessorDef.For<ConventionTarget>(x => x.Name);
            modifier.CreateModifier(def).ShouldBeTheSameAs(tagModifier);
        }

        [Test]
        public void return_null_if_the_filter_does_not_match_the_accessor()
        {
            AccessorDef def = AccessorDef.For<ConventionTarget>(x => x.Age);
            modifier.CreateModifier(def).ShouldBeNull();
        }
    }
}