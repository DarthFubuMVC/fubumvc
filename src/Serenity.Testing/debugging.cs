using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using FubuCore.Conversion;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using HtmlTags;
using KayakTestApplication;
using NUnit.Framework;
using StructureMap;
using TestContext = StoryTeller.Engine.TestContext;
using FubuTestingSupport;

namespace Serenity.Testing
{
    [TestFixture, Explicit]
    public class debugging
    {

        [Test]
        public void does_is_uneque_work()
        {
            typeof (SingleTypeActionSource).GetConstructors().Each(
                x => Debug.WriteLine(x));
        }

        [Test]
        public void start_an_inprocess_system_without_blowing_up()
        {
            var context = new TestContext();
            var system = new InProcessSerenitySystem<KayakApplication>();
            system.SetupEnvironment();
            system.RegisterServices(context);

            var driver = context.Retrieve<NavigationDriver>();

            driver.NavigateToHome();

            Thread.Sleep(5000);

            system.TeardownEnvironment();
        }

        [Test]
        public void what_does_by_to_string_look_like()
        {
            var dictionary = new Dictionary<string, object>();
            var list = new List<Dictionary<string, object>>();
            for (var i = 0; i < 5; i++)
            {
                var dict = new Dictionary<string, object>();
                dict.Add("key", i);
                dict.Add("name", "somebody");
                list.Add(dict);
            }

            dictionary.Add("nodes", list);

            Debug.WriteLine(JsonUtil.ToJson(dictionary));
        }

        [Test]
        public void write_source()
        {
            var settings = ApplicationSettings.For<KayakApplication>();
            settings.Write();
        }
    }
}