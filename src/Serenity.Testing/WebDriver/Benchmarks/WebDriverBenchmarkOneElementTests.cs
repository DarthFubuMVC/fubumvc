using System.Collections.Generic;
using FubuCore;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Serenity.Testing.WebDriver.Benchmarks
{
    public class WebDriverBenchmarkOneElementTests<TBrowser> : WebDriverBenchmarkBase<TBrowser, By> where TBrowser : IBrowserLifecycle, new()
    {
        private const string InputId = "inputid";
        private const string InputClass = "some-input-class";
        private const string InputName = "somename";

        protected override void ConfigureDocument(HtmlDocument document)
        {
            document.Add(new HtmlTag("input", tag =>
            {
                tag.Id(InputId);
                tag.AddClass(InputClass);
                tag.Attr("name", InputName);
            }));

            document.ReferenceJavaScriptFile("file:///" + "jquery-2.0.3.min.js".ToFullPath());
        }

        protected override void ActionToBenchmark(By selector)
        {
            var element = Driver.FindElement(selector);
        }

        public override IEnumerable<TestCaseData> Cases()
        {
            yield return new TestCaseData(By.Id(InputId));
            yield return new TestCaseData(By.CssSelector("#{0}".ToFormat(InputId))); 
            yield return new TestCaseData((By) Serenity.WebDriver.By.jQuery("#{0}".ToFormat(InputId)));
            yield return new TestCaseData(By.ClassName(InputClass));
            yield return new TestCaseData(By.CssSelector(".{0}".ToFormat(InputClass)));
            yield return new TestCaseData((By) Serenity.WebDriver.By.jQuery(".{0}".ToFormat(InputClass)));
            yield return new TestCaseData(By.Name(InputName));
            yield return new TestCaseData(By.CssSelector("input[name=\"{0}\"]".ToFormat(InputName)));
            yield return new TestCaseData((By) Serenity.WebDriver.By.jQuery("input[name='{0}']".ToFormat(InputName)));
        }
    }
}