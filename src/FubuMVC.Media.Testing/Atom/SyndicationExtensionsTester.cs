using FubuMVC.Media.Atom;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Media.Testing.Atom
{
    [TestFixture]
    public class SyndicationExtensionsTester
    {
        [Test]
        public void set_the_uri_of_a_syndication_link()
        {
            var link = new Link("http://somewhere.com/method");

            link.ToSyndicationLink().Uri.OriginalString.ShouldEqual(link.Url);
        }

        [Test]
        public void sets_the_relationship_type_if_it_exists()
        {
            var link = new Link("http://somewhere.com/method");
 
            link.ToSyndicationLink().RelationshipType.ShouldBeNull();

            link.Rel = "something";

            link.ToSyndicationLink().RelationshipType.ShouldEqual(link.Rel);
        }

        [Test]
        public void sets_the_title_if_it_exists()
        {
            var link = new Link("http://somewhere.com/method");

            link.ToSyndicationLink().Title.ShouldBeNull();

            link.Title = "something";

            link.ToSyndicationLink().Title.ShouldEqual(link.Title);
        }

        [Test]
        public void sets_the_mime_type_if_it_exists()
        {
            var link = new Link("http://somewhere.com/method");

            link.ToSyndicationLink().MediaType.ShouldBeNull();

            link.ContentType = "something";

            link.ToSyndicationLink().MediaType.ShouldEqual(link.ContentType); 
        }
    }
}