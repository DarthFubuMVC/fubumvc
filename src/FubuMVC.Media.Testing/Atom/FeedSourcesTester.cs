using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;
using FubuMVC.Media.Atom;
using FubuMVC.Media.Testing.Xml;
using FubuTestingSupport;
using NUnit.Framework;
using FubuMVC.Media.Projections;

namespace FubuMVC.Media.Testing.Atom
{
    [TestFixture]
    public class FeedSourcesTester
    {
        [Test]
        public void return_values_for_the_main_model()
        {
            var enumerable = new AddressEnumerable();

            var request = new InMemoryFubuRequest();
            request.Set(enumerable);

            var source = new EnumerableFeedSource<Address>(enumerable);

            source.GetValues().Select(x => x.ValueFor(o => o.City))
                .ShouldHaveTheSameElementsAs("Austin", "Dallas", "Houston");
        }


    }

    public class AddressEnumerable : IEnumerable<Address>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Address> GetEnumerator()
        {
            yield return new Address(){City = "Austin"};
            yield return new Address(){City = "Dallas"};
            yield return new Address(){City = "Houston"};
        }
    }

    public class AddressValuesEnumerable : IEnumerable<Media.Projections.IValues<Address>>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Media.Projections.IValues<Address>> GetEnumerator()
        {
            foreach (var address in new AddressEnumerable())
            {
                yield return new Media.Projections.SimpleValues<Address>(address);
            }
        }
    }
}