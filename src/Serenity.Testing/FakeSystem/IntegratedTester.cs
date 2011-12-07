using System;
using FubuMVC.Core;
using FubuMVC.Core.UI;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using FubuMVC.StructureMap;
using Serenity.Fixtures;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StructureMap;
using FubuCore;
using Serenity.Testing.Fixtures;

namespace Serenity.Testing.FakeSystem
{
    [TestFixture]
    public class IntegratedTester
    {
        private ITestRunner theRunner;

        [TestFixtureSetUp]
        public void SetUp()
        {
            theRunner = TestRunnerBuilder.ForSystem<FakeSerenitySystem>();
        }

        [Test]
        public void run_simple_test()
        {
            var test = new Test("Try it");
            test.Add(Section.For<NameScreenFixture>().WithStep("GoTo", "name:Jeremy"));

            var testResult = theRunner.RunTest(test);
            testResult.Counts.ShouldEqual(0, 0, 0, 0);
        }

        [Test]
        public void run_more_complicated_test_positive()
        {
            var test = new Test("Try it");
            var section = Section.For<NameScreenFixture>()
                .WithStep("GoTo", "name:Jeremy")
                .WithStep("CheckName", "Name:Jeremy");
            test.Add(section);

            var testResult = theRunner.RunTest(test);
            testResult.Counts.ShouldEqual(1, 0, 0, 0); 
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            theRunner.SafeDispose();
        }
    }

    public class NameScreenFixture : ScreenFixture<TextModel>
    {
        public NameScreenFixture()
        {
            EditableElement(x => x.Name);
        }

        [FormatAs("Go to name {name}")]
        public void GoTo(string name)
        {
            // Add to ScreenFixture
            var driver = new ApplicationDriver(Application);
            driver.NavigateTo(new TextModel{Name = name});
        }
    }

    public class FakeSerenitySystem : InProcessSerenitySystem<FakeSerenitySource>
    {
        
        protected override ApplicationSettings findApplicationSettings()
        {
            var settings = ApplicationSettings.For<FakeSerenitySource>();
            settings.PhysicalPath = ".".ToFullPath();

            return settings;
        }

        public override void RegisterFixtures(FixtureRegistry registry)
        {
            registry.AddFixture<NameScreenFixture>();
        }
    }

    public class FakeSerenitySource : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<FakeSerenityRegistry>()
                .StructureMap(new Container());
        }
    }

    public class FakeSerenityRegistry : FubuRegistry
    {
        public FakeSerenityRegistry()
        {
            Actions.IncludeType<FakeSerenityActions>();
        }
    }

    public class TextModel
    {
        public string Name { get; set;}
    }

    public class FakeSerenityActions
    {
        private readonly IServiceLocator _services;

        public FakeSerenityActions(IServiceLocator services)
        {
            _services = services;
        }

        public HtmlDocument get_person_Name(TextModel model)
        {
            var document = new FubuHtmlDocument<TextModel>(_services){
                Model = model
            };

            document.Title = "Persion:" + model.Name;

            document.Push("p");
            document.Add(x => x.LabelFor(o => o.Name));
            document.Add(x => x.InputFor(o => o.Name));

            return document;
        }
    }
}