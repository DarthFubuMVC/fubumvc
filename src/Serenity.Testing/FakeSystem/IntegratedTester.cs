using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore.Conversion;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using HtmlTags;

using NUnit.Framework;
using FubuMVC.StructureMap;
using OpenQA.Selenium;
using Rhino.Mocks;
using Serenity.Fixtures;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Execution;
using StructureMap;
using FubuCore;
using Serenity.Testing.Fixtures;
using FubuTestingSupport;
using System.Linq;

namespace Serenity.Testing.FakeSystem
{
    [TestFixture, Explicit]
    public class IntegratedTester
    {
        private ITestRunner theRunner;

        [TestFixtureSetUp]
        public void SetUp()
        {
            theRunner = TestRunner.ForSystem<FakeSerenitySystem>();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            theRunner.Dispose();
        }
		
        [Test]
        public void run_simple_test()
        {
            var test = new Test("Try it");
            test.Add(Section.For<NameScreenFixture>().WithStep("GoTo", "name:Jeremy"));
			
			var testResult = theRunner.RunTest(new TestExecutionRequest(test, new TestStopConditions()));
			//var testResult = TestRunnerExtensions.RunTest(theRunner, test);
			testResult.Counts.ShouldEqual(0, 0, 0, 0);
			//testResult.Counts.ShouldEqual(1, 0, 0, 0); 
        }

        [Test]
        public void run_more_complicated_test_positive()
        {
            var test = new Test("Try it");
            var section = Section.For<NameScreenFixture>()
                .WithStep("GoTo", "name:Jeremy")
                .WithStep("CheckName", "Name:Jeremy");
            test.Add(section);
			
			var testResult = theRunner.RunTest(new TestExecutionRequest(test, new TestStopConditions()));
            //var testResult = theRunner.RunTest(test);
			testResult.Counts.ShouldEqual(1, 0, 0, 0);
            //testResult.Counts.ShouldEqual(1, 0, 0, 0); 
        }


        [Test,Explicit]
        public void run_more_complicated_test_negative()
        {
            var test = new Test("Try it");
            var section = Section.For<NameScreenFixture>()
                .WithStep("GoTo", "name:Jeremy")
                .WithStep("CheckName", "Name:Max");
            test.Add(section);

			var testResult = theRunner.RunTest(new TestExecutionRequest(test, new TestStopConditions()));
            //var testResult = theRunner.RunTest(test);
			testResult.Counts.ShouldEqual(0, 1, 0, 0);
            //testResult.Counts.ShouldEqual(0, 1, 0, 0);
        }

        [Test]
        public void convert_with_custom_converter()
        {
            var context = new FakeSerenitySystem().CreateContext();
            context.BindingRegistry.Converters.AllConverterFamilies.OfType<RandomTypeConverter>()
                .Any().ShouldBeTrue();
        }

        [Test]
        public void register_a_custom_after_navigation()
        {
            var context = new FakeSerenitySystem().CreateContext();
            context.Services.GetInstance<IApplicationUnderTest>()
                   .Navigation.AfterNavigation.ShouldBeOfType<FakeAfterNavigation>();
        }

        [Test]
        public void smoke_test_the_on_start_up()
        {
            var system = new FakeSerenitySystem();
            system.CreateContext().ShouldNotBeNull();
            system.TheContainer.ShouldNotBeNull();
            
        }

        [Test]
        public void calls_subsystem_start_on_each()
        {
            var system = new FakeSerenitySystem();
            system.SubSystems.Each(x => {
                x.AssertWasCalled(o => o.Start());
            });
        }

        [Test]
        public void calls_syssystem_stop_on_each_when_disposing()
        {
            var system = new FakeSerenitySystem();
            system.CreateContext();

            system.Application.ShouldNotBeNull();
        
            system.Dispose();

            system.SubSystems.Each(x =>
            {
                x.AssertWasCalled(o => o.Stop());
            });
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
			theRunner.SafeDispose();
            //theRunner.SafeDispose();
        }
    }

    public class FakeSerenitySystem : FubuMvcSystem<FakeSerenitySource>
    {
        public IContainer TheContainer;

        public IList<ISubSystem> SubSystems = new List<ISubSystem>();

        public FakeSerenitySystem()
        {
            AddConverter<RandomTypeConverter>();
            AfterNavigation = new FakeAfterNavigation();

            OnStartup<IContainer>(c => TheContainer = c);

            AddSubSystem(MockedSubSystem());
            AddSubSystem(MockedSubSystem());
            AddSubSystem(MockedSubSystem());
        }

        public ISubSystem MockedSubSystem()
        {
            var system = MockRepository.GenerateMock<ISubSystem>();
            system.Stub(x => x.Start()).Return(Task.Factory.StartNew(() => {}));
            system.Stub(x => x.Stop()).Return(Task.Factory.StartNew(() => {}));

            return system;
        }
    }

   

    public class FakeAfterNavigation : IAfterNavigation
    {
        public void AfterNavigation(IWebDriver driver, string desiredUrl)
        {
            throw new NotImplementedException();
        }
    }

    public class RandomTypeConverter : StatelessConverter<RandomType>
    {
        protected override RandomType convert(string text)
        {
            return new RandomType {Name = text};
        }
    }

    public class RandomType
    {
        public string Name { get; set; }
    }

    public class NameScreenFixture : ScreenFixture
    {

        [FormatAs("Go to name {name}")]
        public void GoTo(string name)
        {
            // Add to ScreenFixture
            var driver = new NavigationDriver(Application);
            driver.NavigateTo(new TextModel{Name = name});
        }
    }


    public class FakeSerenitySource : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
			return BootstrappingExtensions.StructureMap(
            	FubuApplication.For<FakeSerenityRegistry>()
                , new Container());
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
            var document = new FubuHtmlDocument<TextModel>(_services, new InMemoryFubuRequest()){
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