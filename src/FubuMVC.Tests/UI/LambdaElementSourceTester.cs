using FubuMVC.Core.UI.Configuration;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class LambdaElementSourceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            builder = r => new HtmlTag("div").Id(r.ElementId);
            _builder = new LambdaElementBuilder(def => def.Accessor.PropertyType == typeof (string), x => builder);
        }

        #endregion

        private TagBuilder builder;
        private LambdaElementBuilder _builder;

        public class ConventionTarget
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Test]
        public void return_a_builder_if_the_filter_matches()
        {
            AccessorDef def = AccessorDef.For<ConventionTarget>(x => x.Name);
            _builder.CreateInitial(def).ShouldBeTheSameAs(builder);
        }

        [Test]
        public void return_nothing_if_the_filter_does_not_match_the_accessor()
        {
            AccessorDef def = AccessorDef.For<ConventionTarget>(x => x.Age);
            _builder.CreateInitial(def).ShouldBeNull();
        }
    }
}