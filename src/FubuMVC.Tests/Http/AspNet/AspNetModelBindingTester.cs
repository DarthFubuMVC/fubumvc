using System.Web;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.Core;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Http.AspNet
{
    
    public class AspNetModelBindingTester
    {
        private IValueConverterRegistry registry;


        public AspNetModelBindingTester()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                registry = runtime.Get<BindingRegistry>();
            }
        }

        [Fact]
        public void can_find_a_converter_for_a_system_property()
        {
            var property = ReflectionHelper.GetProperty<ModelWithHttpPostedFileBase>(x => x.AcceptTypes);
            registry.FindConverter(property).ShouldNotBeNull();
        }

        [Fact]
        public void can_find_a_converter_for_HttpPostedFileBase()
        {
            var property = ReflectionHelper.GetProperty<ModelWithHttpPostedFileBase>(x => x.File);
            registry.FindConverter(property).ShouldNotBeNull();
        }

        [Fact]
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