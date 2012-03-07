using System.Diagnostics;
using System.Web;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using System.Collections.Generic;
using StructureMap;

namespace FubuMVC.Tests.Http.AspNet
{
    [TestFixture]
    public class AspNetModelBindingTester
    {
        private IValueConverterRegistry registry;

        [SetUp]
        public void SetUp()
        {
            var container = new Container();
            FubuApplication.For(new FubuRegistry()).StructureMap(container).Bootstrap();
            registry = container.GetInstance<BindingRegistry>();
        }

        [Test]
        public void can_find_a_converter_for_a_system_property()
        {
            var property = ReflectionHelper.GetProperty<ModelWithHttpPostedFileBase>(x => x.AcceptTypes);
            registry.FindConverter(property).ShouldNotBeNull();
        }

        [Test]
        public void can_find_a_converter_for_HttpPostedFileBase()
        {
            var property = ReflectionHelper.GetProperty<ModelWithHttpPostedFileBase>(x => x.File);
            registry.FindConverter(property).ShouldNotBeNull();
        }

        [Test]
        public void can_find_a_converter_for_HttpCookie()
        {
            var property = ReflectionHelper.GetProperty<ModelWithHttpPostedFileBase>(x => x.MyCookie);
            registry.FindConverter(property).ShouldNotBeNull();
        }


        public class ModelWithHttpPostedFileBase
        {
            public HttpPostedFileBase File { get; set; }
            public string File2 { get; set; }
            public HttpCookie MyCookie { get; set; }

            // this is a "system" property defined in AggregateDictionary
            public string[] AcceptTypes { get; set; }
        }
    }
}