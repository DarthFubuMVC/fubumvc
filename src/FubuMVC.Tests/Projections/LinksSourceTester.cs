using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
{
    [TestFixture]
    public class LinksSourceTester
    {
        private Site theSubject;
        private SimpleProjectionTarget theTarget;
        private ValidStubUrlRegistry theUrls;
        private LinksSource<Site> theLinks;

        [SetUp]
        public void SetUp()
        {
            theSubject = new Site(){Name = "my site", Id = Guid.NewGuid()};
            theUrls = new ValidStubUrlRegistry();
            theTarget = new SimpleProjectionTarget(theSubject, theUrls);

            theLinks = new LinksSource<Site>();
        }

        [Test]
        public void create_link_by_subject()
        {
            theLinks.LinkToSubject();

            theLinks.As<ILinkSource>().LinksFor(theTarget)
                .Single().Uri.OriginalString.ShouldEqual(theUrls.UrlFor(theSubject));
        }

        [Test]
        public void create_link_by_transforming_the_subject()
        {
            theLinks.LinkTo(site => new SiteAction(site.Name));

            theLinks.As<ILinkSource>().LinksFor(theTarget)
                .Single().Uri.OriginalString.ShouldEqual(theUrls.UrlFor(new SiteAction(theSubject.Name)));
        }

        [Test]
        public void create_link_by_identifier()
        {
            theLinks.LinkToSubject(x => x.Id);

            var parameters = new RouteParameters<Site>();
            parameters[x => x.Id] = theSubject.Id.ToString();

            theLinks.As<ILinkSource>().LinksFor(theTarget)
                .Single().Uri.OriginalString.ShouldEqual(theUrls.UrlFor(typeof(Site), parameters));
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
            throw new NotImplementedException();
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
    }
}