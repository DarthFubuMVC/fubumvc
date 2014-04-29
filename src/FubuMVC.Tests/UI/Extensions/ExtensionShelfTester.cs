using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.UI.Extensions;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Extensions
{
    [TestFixture]
    public class ExtensionShelfTester
    {
        [Test]
        public void filter_the_last_extension()
        {
            var shelf = new ExtensionShelf<ExtensionShelfModel>();
            shelf.Add("tag1", new StubExtension());
            shelf.Add("tag1", new StubExtension());
            shelf.Add("tag2", new StubExtension());
            var theExtensionThatShouldBeWrapped = new StubExtension();
            shelf.Add("tag2", theExtensionThatShouldBeWrapped);

            Func<IFubuPage<ExtensionShelfModel>, bool> filter = page => true;

            shelf.FilterLast(filter);

            // don't modify any of the previous ones
            shelf.ExtensionsFor("tag1").Each(x => x.ShouldBeOfType<StubExtension>());

            shelf.ExtensionsFor("tag2").First().ShouldBeOfType<StubExtension>();

            shelf.ExtensionsFor("tag2").Count().ShouldEqual(2);
            var filtered = shelf.ExtensionsFor("tag2").Last().ShouldBeOfType<FilteredContentExtension<ExtensionShelfModel>>();

            filtered.Inner.ShouldBeTheSameAs(theExtensionThatShouldBeWrapped);
            filtered.Filter.ShouldBeTheSameAs(filter);

        }
    }

    public class StubExtension : IContentExtension<ExtensionShelfModel>
    {
        public IEnumerable<object> GetExtensions(IFubuPage<ExtensionShelfModel> page)
        {
            throw new NotImplementedException();
        }
    }

    public class ExtensionShelfModel{}
}