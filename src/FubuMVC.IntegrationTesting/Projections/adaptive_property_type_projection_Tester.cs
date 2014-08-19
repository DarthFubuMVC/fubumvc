using System;
using System.Net;
using FubuMVC.Core.Projections;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Projections
{
    [TestFixture]
    public class adaptive_property_type_projection_Tester
    {
        [Test]
        public void use_adaptive_projection_of_child_properties()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<MyAdaptiveEndpoint>(x => x.get_adaptive_projection_foo());
                _.ContentShouldBe("{\"subject\":{\"foo\":\"Shaver\"},\"data\":\"some data\"}");
            });

            TestHost.Scenario(_ =>
            {
                _.Get.Action<MyAdaptiveEndpoint>(x => x.get_adaptive_projection_bar());
                _.ContentShouldBe("{\"subject\":{\"bar\":\"Kentucky Headhunters\"},\"data\":\"some data\"}");
            });
        }

        [Test]
        public void projection_adaptively_when_value_is_null()
        {
            TestHost.Scenario(_ =>
            {
                _.Get.Action<MyAdaptiveEndpoint>(x => x.get_adaptive_projection_null());
                _.ContentShouldBe("{\"data\":\"some data\"}");
            });
        }

        [Test]
        public void projects_using_default_serialization_if_projection_cannot_be_found()
        {
            TestHost.Scenario(_ => {
                _.Get.Action<MyAdaptiveEndpoint>(x => x.get_adaptive_projection_baz());
                _.ContentShouldBe("{\"subject\":{\"BazName\":\"Screamin Cheetah Wheelies\"},\"data\":\"some data\"}");
            });
        }
    }

    

    public class MyBaz
    {
        public string BazName { get; set; }
    }

    public class MyAdaptiveEndpoint
    {
        public MyAdaptiveModel get_adaptive_projection_null()
        {
            return new MyAdaptiveModel
            {
                Data = "some data",
                Subject = null
            };
        }

        public MyAdaptiveModel get_adaptive_projection_foo()
        {
            return new MyAdaptiveModel
            {
                Data = "some data",
                Subject = new MyFoo{FooName = "Shaver"}
            };
        }

        public MyAdaptiveModel get_adaptive_projection_bar()
        {
            return new MyAdaptiveModel
            {
                Data = "some data",
                Subject = new MyBar
                {
                    BarName = "Kentucky Headhunters"
                }
            };
        }

        public MyAdaptiveModel get_adaptive_projection_baz()
        {
            return new MyAdaptiveModel
            {
                Data = "some data",
                Subject = new MyBaz
                {
                    BazName = "Screamin Cheetah Wheelies"
                }
            };
        }
    }

    public class MyAdaptiveProjection : Projection<MyAdaptiveModel>
    {
        public MyAdaptiveProjection()
        {
            AdaptiveValue(x => x.Subject).Name("subject");
            Value(x => x.Data).Name("data");
        }
    }

    public class MyAdaptiveModel
    {
        public object Subject { get; set; }
        public object Data { get; set; }
    }

    public class MyFooProjection : Projection<MyFoo>
    {
        public MyFooProjection()
        {
            Value(x => x.FooName).Name("foo");
        }
    }

    public class MyBarProjection : Projection<MyBar>
    {
        public MyBarProjection()
        {
            Value(x => x.BarName).Name("bar");
        }
    }

    public class MyFoo
    {
        public string FooName { get; set; }
    }

    public class MyBar
    {
        public string BarName { get; set; }
    }
}