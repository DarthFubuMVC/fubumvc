using Shouldly;
using NUnit.Framework;
using Serenity.WebDriver;

namespace Serenity.Testing.WebDriver
{
    public class JQueryTester
    {
        [Test]
        public void BuildsFilterHasTextFunction()
        {
            string function = JQuery.HasTextFilterFunction("some text").Statement;
            function.ShouldBe("function() { return $(this).text().trim() === 'some text'; }");
        }

        [Test]
        public void BuildsFilterDoesNotHaveTextFunction()
        {
            string function = JQuery.DoesNotHaveTextFilterFunction("some text").Statement;
            function.ShouldBe("function() { return $(this).text().trim() !== 'some text'; }");
        }
    }
}