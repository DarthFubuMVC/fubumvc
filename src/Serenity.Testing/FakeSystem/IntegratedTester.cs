using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Conversion;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.StructureMap;
using FubuMVC.Core.View;
using Shouldly;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Rhino.Mocks;
using Serenity.Fixtures;
using StoryTeller;
using StructureMap;

namespace Serenity.Testing.FakeSystem
{
    [TestFixture, Explicit]
    public class IntegratedTester
    {
        [Test]
        public void register_a_custom_after_navigation()
        {
            var context = new FakeSerenitySystem().CreateContext();
            context.GetService<IApplicationUnderTest>()
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
            system.SubSystems.Each(x => { x.AssertWasCalled(o => o.Start()); });
        }

        [Test]
        public void calls_syssystem_stop_on_each_when_disposing()
        {
            var system = new FakeSerenitySystem();
            system.CreateContext();

            system.Application.ShouldNotBeNull();

            system.Dispose();

            system.SubSystems.Each(x => { x.AssertWasCalled(o => o.Stop()); });
        }

    }

    public class FakeSerenitySystem : FubuMvcSystem<FakeSerenitySource>
    {
        public IContainer TheContainer;

        public IList<ISubSystem> SubSystems = new List<ISubSystem>();

        public FakeSerenitySystem()
        {
            AfterNavigation = new FakeAfterNavigation();

            OnStartup<IContainer>(c => TheContainer = c);

            AddSubSystem(MockedSubSystem());
            AddSubSystem(MockedSubSystem());
            AddSubSystem(MockedSubSystem());
        }

        public ISubSystem MockedSubSystem()
        {
            var system = MockRepository.GenerateMock<ISubSystem>();
            system.Stub(x => x.Start()).Return(Task.Factory.StartNew(() => { }));
            system.Stub(x => x.Stop()).Return(Task.Factory.StartNew(() => { }));

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



    public class FakeSerenitySource : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory)
        {
            return FubuApplication.For<FakeSerenityRegistry>();
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
        public string Name { get; set; }
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
            var document = new FubuHtmlDocument<TextModel>(_services, new InMemoryFubuRequest())
            {
                Model = model
            };

            document.Title = "Persion:" + model.Name;

            document.Push("p");
            document.Add("span").Text(document.Model.Name);
            document.Add("input").Attr("type", "text").Id("Name").Name("Name").Value(model.Name);

            return document;
        }
    }
}