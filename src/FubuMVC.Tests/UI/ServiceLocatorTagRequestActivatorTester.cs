using System;
using FubuCore;
using FubuMVC.Core.UI;
using FubuTestingSupport;
using HtmlTags.Conventions;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class ServiceLocatorTagRequestActivatorTester
    {
        [Test]
        public void matches_negative()
        {
            new ServiceLocatorTagRequestActivator(new InMemoryServiceLocator())
            .Matches(typeof(SubjectNotServiceAware)).ShouldBeFalse();
        }

        [Test]
        public void matches_positive()
        {
            new ServiceLocatorTagRequestActivator(new InMemoryServiceLocator())
                .Matches(typeof(SubjectThatIsServiceAware)).ShouldBeTrue();
        }

        [Test]
        public void activate_should_attach_the_service_locator_to_the_subject()
        {
            var services = new InMemoryServiceLocator();

            var activator = new ServiceLocatorTagRequestActivator(services);

            var subject = new SubjectThatIsServiceAware();

            activator.Activate(subject);

            subject.Services.ShouldBeTheSameAs(services);
        }
    }

    public class SubjectNotServiceAware{}

    public class SubjectThatIsServiceAware : TagRequest, IServiceLocatorAware
    {
        private IServiceLocator _services;

        public void Attach(IServiceLocator locator)
        {
            _services = locator;
        }

        public IServiceLocator Services
        {
            get { return _services; }
        }

        public override object ToToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}