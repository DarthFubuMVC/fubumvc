using System;
using FubuCore.Dates;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Web.UI;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    [TestFixture]
    public class DateElementModifierTester : ValidationElementModifierContext<DateElementModifier>
    {
        [Test]
        public void adds_the_date_css_class_for_fubucore_date()
        {
            tagFor(ElementRequest.For<DateElementTarget>(x => x.FubuCoreDate)).HasClass("date").ShouldBeTrue();
        }

        [Test]
        public void adds_the_date_css_class_for_DateTime()
        {
            tagFor(ElementRequest.For<DateElementTarget>(x => x.Date)).HasClass("date").ShouldBeTrue();
        }

        [Test]
        public void no_date_for_other_types()
        {
            new DateElementModifier().Matches(ElementRequest.For<DateElementTarget>(x => x.NoDate)).ShouldBeFalse();
        }


        public class DateElementTarget
        {
            public string NoDate { get; set; }
            public DateTime Date { get; set; }
            public Date FubuCoreDate { get; set; }
        }
    }
}