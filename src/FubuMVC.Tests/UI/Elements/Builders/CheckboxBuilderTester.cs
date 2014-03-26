using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Elements.Builders;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Elements.Builders
{
    [TestFixture]
    public class CheckboxBuilderTester
    {
        private ElementRequest name = ElementRequest.For<BooleanTarget>(x => x.Name);
        private ElementRequest accredited = ElementRequest.For<BooleanTarget>(x => x.Accredited);
        private ElementRequest passed = ElementRequest.For<BooleanTarget>(x => x.Passed);
        private CheckboxBuilder builder = new CheckboxBuilder();

        [Test]
        public void matches_positive_against_bool()
        {
            builder.Matches(accredited).ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            builder.Matches(name).ShouldBeFalse();
            builder.Matches(passed).ShouldBeFalse(); // no bool?
        }

        [Test]
        public void build_with_true()
        {
            var request = ElementRequest.For<BooleanTarget>(x => x.Accredited);
            request.Model = new BooleanTarget{
                Accredited = true
            };

            builder.Build(request).ToString().ShouldEqual("<input type=\"checkbox\" checked=\"true\" />");
        }

        [Test]
        public void build_with_false()
        {
            accredited.Model = new BooleanTarget
            {
                Accredited = false
            };

            builder.Build(accredited).ToString().ShouldEqual("<input type=\"checkbox\" />"); 
        }
    }

    public class BooleanTarget
    {
        public string Name { get; set; }
        public bool Accredited { get; set; }
        public bool? Passed { get; set; }
    }
}