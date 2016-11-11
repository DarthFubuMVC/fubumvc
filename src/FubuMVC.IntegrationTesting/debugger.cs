using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore.Dates;
using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture, Explicit]
    public class debugger
    {
        [Test]
        public void try_to_load_asset_fixture()
        {
            var assembly = Assembly.Load("WebDriver");
            assembly.GetExportedTypes().Each(x => Console.WriteLine(x.FullName));
        }

        [Test]
        public void try_to_load_the_serenity_assembly()
        {
            var assembly = Assembly.Load("Serenity");
            assembly.GetExportedTypes().Each(x => Console.WriteLine(x.FullName));
        }

        [Test]
        public void try_to_create_ISystemTime()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                runtime.Get<ISystemTime>().ShouldBeOfType<SystemTime>();
            }
        }

        [Test]
        public void make_it_fail()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                runtime.Scenario(_ =>
                {
                    _.Get.Action<ErrorEndpoint>(x => x.get_error2());
                });

            }
        }
    }

    public class SomeService : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("I was disposed.");
        }
    }

    public class ErrorEndpoint
    {
        private readonly SomeService _service;

        public ErrorEndpoint(SomeService service)
        {
            _service = service;
        }

        public string get_error1()
        {
            throw new DivideByZeroException();
        }

        public Task<string> get_error2()
        {
            return Task.Factory.StartNew(() =>
            {
                throw new DivideByZeroException();

                return "hello";
            });
        }
    }
}