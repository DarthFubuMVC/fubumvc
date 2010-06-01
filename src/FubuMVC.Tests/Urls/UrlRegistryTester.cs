using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.View.FakeViews;
using NUnit.Framework;

namespace FubuMVC.Tests.Urls
{
    [TestFixture]
    public class UrlRegistryTester
    {
        [SetUp]
        public void SetUp()
        {
            UrlContext.Reset();
            registry = new UrlRegistry();
        }

        private UrlRegistry registry;

        public class UrlRegistryTesterController
        {
            public ViewModel1 New()
            {
                return null;
            }
        }

        public class SiteEntity
        {
        }

        [Test]
        public void action_url_ctor_function()
        {
            ActionCall call = ActionCall.For<UrlRegistryTesterController>(x => x.New());
            var route = new RouteDefinition("somepattern");

            var url = new ActionUrl(route, call);

            url.GetUrl(null).ShouldEqual(route.CreateUrl(null));
            url.HandlerType.ShouldEqual(call.HandlerType);
            url.Method.ShouldEqual(call.Method);
        }

        [Test]
        public void can_find_for_action()
        {
            ActionCall call = ActionCall.For<UrlRegistryTesterController>(x => x.New());
            var route = new RouteDefinition("somepattern");

            var url = new ActionUrl(route, call);

            registry.AddAction(url);

            registry.UrlFor<UrlRegistryTesterController>(x => x.New()).ShouldEqual("somepattern");
        }

        [Test]
        public void can_find_new()
        {
            ActionCall call = ActionCall.For<UrlRegistryTesterController>(x => x.New());
            var route = new RouteDefinition("somepattern");

            var url = new ActionUrl(route, call);

            registry.RegisterNew(url, typeof (SiteEntity));

            registry.UrlForNew<SiteEntity>().ShouldEqual(url.GetUrl(null));
        }

        [Test]
        public void has_new_positive_case()
        {
            ActionCall call = ActionCall.For<UrlRegistryTesterController>(x => x.New());
            var route = new RouteDefinition("somepattern");

            var url = new ActionUrl(route, call);

            registry.RegisterNew(url, typeof(SiteEntity));

            registry.HasNewUrl<SiteEntity>().ShouldBeTrue();
        }

        [Test]
        public void has_new_negative_case()
        {
            registry.HasNewUrl<SiteEntity>().ShouldBeFalse();
        }

        [Test]
        public void find_default_if_there_is_only_one()
        {
            var url = new StubModelUrl<UrlModel>(m => "/name/" + m.Name);
            registry.AddModel(url);

            registry.UrlFor(new UrlModel
            {
                Name = "Jeremy"
            }).ShouldEqual("/name/Jeremy");
        }

        [Test]
        public void find_for_action_throws_if_nothing_found()
        {
            Exception<FubuException>.ShouldBeThrownBy(
                () => { registry.UrlFor<UrlRegistryTesterController>(x => x.New()); });
        }

        [Test]
        public void find_url_by_category()
        {
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/name/" + m.Name)
            {
                Category = Categories.EDIT
            });
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/age/" + m.Age)
            {
                Category = Categories.DEFAULT
            });

            registry.UrlFor(new UrlModel
            {
                Name = "Jeremy",
                Age = 35
            }, Categories.EDIT).ShouldEqual("/name/Jeremy");
        }

        [Test]
        public void find_url_by_type_with_nothing_registered()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                registry.UrlFor(new UrlModel
                {
                    Name = "Jeremy",
                    Age = 35
                }, Categories.EDIT).ShouldEqual("/name/Jeremy");
            });
        }

        [Test]
        public void if_there_is_more_than_one_url_for_a_type_look_for_default_category()
        {
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/name/" + m.Name));
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/age/" + m.Age)
            {
                Category = Categories.DEFAULT
            });

            registry.UrlFor(new UrlModel
            {
                Name = "Jeremy",
                Age = 35
            }).ShouldEqual("/age/35");
        }

        [Test]
        public void throw_exception_if_there_are_no_urls_for_that_type()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                registry.UrlFor(new UrlModel
                {
                    Name = "Jeremy"
                });
            });
        }

        [Test]
        public void throw_exception_if_there_are_two_urls_and_none_are_default()
        {
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/name/" + m.Name));
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/name/" + m.Age));


            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                registry.UrlFor(new UrlModel
                {
                    Name = "Jeremy"
                });
            });
        }


        [Test]
        public void throw_exception_if_there_are_two_urls_for_a_given_category()
        {
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/name/" + m.Name)
            {
                Category = Categories.EDIT
            });
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/age/" + m.Age)
            {
                Category = Categories.EDIT
            });

            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                registry.UrlFor(new UrlModel
                {
                    Name = "Jeremy",
                    Age = 35
                }, Categories.EDIT);
            }).ErrorCode.ShouldEqual(2105);
        }
    }

    public class StubModelUrl<T> : IModelUrl
    {
        private readonly Func<T, string> _toUrl;

        public StubModelUrl(Func<T, string> toUrl)
        {
            _toUrl = toUrl;
        }

        public string CreateUrl(object input)
        {
            return _toUrl((T) input);
        }

        public void RootUrlAt(string baseUrl)
        {
            
        }

        public string Category { get; set; }

        public Type InputType { get { return typeof (T); } }
    }

    public class UrlModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class SubclassUrlModel : UrlModel
    {
    }

    [TestFixture]
    public class using_model_type_mapper
    {
        private UrlRegistry registry;

        [SetUp]
        public void SetUp()
        {
            UrlContext.Reset();
            registry = new UrlRegistry();

            registry.AddModel(new StubModelUrl<UrlModel>(m => "/name/" + m.Name)
            {
                Category = Categories.EDIT
            });
            registry.AddModel(new StubModelUrl<UrlModel>(m => "/age/" + m.Age)
            {
                Category = Categories.DEFAULT
            });

            registry.MapModelTypes(o =>
            {
                var type = o.GetType();

                return type == typeof(SubclassUrlModel)
                    ? type.BaseType
                    : type;
            });
        }

        [Test]
        public void regular_find_url_should_work_with_the_model_type_mapper_defined()
        {
            registry.UrlFor(new UrlModel
            {
                Name = "Jeremy",
                Age = 35
            }).ShouldEqual("/age/35");
        }

        [Test]
        public void find_url_using_model_type_mapper()
        {
            registry.UrlFor(new SubclassUrlModel
            {
                Name = "Jeremy",
                Age = 35
            }).ShouldEqual("/age/35");
        }

        [Test]
        public void regular_find_url_by_category_should_work_with_the_model_type_mapper_defined()
        {
            registry.UrlFor(new UrlModel()
            {
                Name = "Jeremy",
                Age = 35
            }, Categories.EDIT).ShouldEqual("/name/Jeremy");
        }

        [Test]
        public void find_url_by_category_using_model_type_mapper()
        {
            registry.UrlFor(new SubclassUrlModel()
            {
                Name = "Jeremy",
                Age = 35
            }, Categories.EDIT).ShouldEqual("/name/Jeremy");
        }
    }
}