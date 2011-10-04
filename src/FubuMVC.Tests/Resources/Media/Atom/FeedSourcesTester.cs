using System.Collections;
using System.Collections.Generic;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Resources.Media.Atom;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Resources.Projections;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Resources.Media.Atom
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

            var source = new EnumerableFeedSource<AddressEnumerable, Address>(request);

            source.GetValues().Select(x => x.ValueFor(o => o.City))
                .ShouldHaveTheSameElementsAs("Austin", "Dallas", "Houston");
        }

        [Test]
        public void return_values_for_direct_enumerable()
        {
            var enumerable = new AddressValuesEnumerable();

            var request = new InMemoryFubuRequest();
            request.Set(enumerable);

            var source = new DirectFeedSource<AddressValuesEnumerable, Address>(request);

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

    public class AddressValuesEnumerable : IEnumerable<IValues<Address>>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IValues<Address>> GetEnumerator()
        {
            foreach (var address in new AddressEnumerable())
            {
                yield return new SimpleValues<Address>(address);
            }
        }
    }
}