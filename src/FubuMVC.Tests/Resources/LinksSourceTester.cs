using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources
{
    [TestFixture]
    public class LinksSourceTester
    {
        private Site theSubject;
        private SimpleValues<Site> theTarget;
        private ValidStubUrlRegistry theUrls;
        private LinksSource<Site> theLinks;

        [SetUp]
        public void SetUp()
        {
            theSubject = new Site(){Name = "my site", Id = Guid.NewGuid()};
            theUrls = new ValidStubUrlRegistry();
            theTarget = new SimpleValues<Site>(theSubject);

            theLinks = new LinksSource<Site>();
        }

        [Test]
        public void create_link_by_subject()
        {
            theLinks.ToSubject();

            theLinks.As<ILinkSource<Site>>().LinksFor(theTarget, theUrls)
                .Single().Url.ShouldEqual(theUrls.UrlFor(theSubject));
        }

        [Test]
        public void create_a_link_by_using_route_parameters()
        {
            theLinks.ToInput<SiteAction>(x => x.Name);

            var parameters = new RouteParameters<SiteAction>();
            parameters["Name"] = theSubject.Name;

            theLinks.As<ILinkSource<Site>>().LinksFor(theTarget, theUrls)
                .Single().Url.ShouldEqual(theUrls.UrlFor(parameters));
        }

        [Test]
        public void create_link_by_transforming_the_subject()
        {
            theLinks.To(site => new SiteAction(site.Name));

            theLinks.As<ILinkSource<Site>>().LinksFor(theTarget, theUrls)
                .Single().Url.ShouldEqual(theUrls.UrlFor(new SiteAction(theSubject.Name)));
        }

        [Test]
        public void create_link_by_identifier()
        {
            theLinks.ToSubject(x => x.Id);

            var parameters = new RouteParameters<Site>();
            parameters[x => x.Id] = theSubject.Id.ToString();

            theLinks.As<ILinkSource<Site>>().LinksFor(theTarget, theUrls)
                .Single().Url.ShouldEqual(theUrls.UrlFor(typeof(Site), parameters));
        }

        public class SiteAction
        {
            private readonly string _name;

            public SiteAction(string name)
            {
                _name = name;
            }

            public override string ToString()
            {
                return string.Format("site action for {0}", _name);
            }
        }
    }

    public class ValidStubUrlRegistry : IUrlRegistry
    {
        public string UrlFor(object model)
        {
            return "http://somewhere.com/" + model.ToString();
        }

        public string UrlFor(object model, string category)
        {
            throw new NotImplementedException();
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression)
        {
            return "http://somewhere.com/" + typeof (TController).Name + "/" +
                   ReflectionHelper.GetMethod(expression).Name;
        }

        public string UrlForNew<T>()
        {
            throw new NotImplementedException();
        }

        public string UrlForNew(Type entityType)
        {
            throw new NotImplementedException();
        }

        public bool HasNewUrl<T>()
        {
            throw new NotImplementedException();
        }

        public bool HasNewUrl(Type type)
        {
            throw new NotImplementedException();
        }

        public string UrlForPropertyUpdate(object model)
        {
            throw new NotImplementedException();
        }

        public string UrlForPropertyUpdate(Type type)
        {
            throw new NotImplementedException();
        }

        public string UrlFor(Type handlerType, MethodInfo method)
        {
            throw new NotImplementedException();
        }

        public string TemplateFor(object model)
        {
            throw new NotImplementedException();
        }

        public string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new()
        {
            throw new NotImplementedException();
        }

        public string UrlFor(Type modelType, RouteParameters parameters)
        {
            return "http://something.com/{0}/{1}".ToFormat(modelType.Name, parameters);
        }

        public string UrlFor(Type modelType, string category, RouteParameters parameters)
        {
            throw new NotImplementedException();
        }

        public string UrlForAsset(AssetFolder? folder, string name)
        {
            throw new NotImplementedException();
        }
    }
}