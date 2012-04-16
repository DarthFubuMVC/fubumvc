using System.Collections;
using System.Collections.Generic;

namespace FubuMVC.Media.Testing.Atom
{
    [TestFixture]
    public class FeedWriterNodeTester
    {
        public class AddressEnumerable : IEnumerable<Address>
        {
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<Address> GetEnumerator()
            {
                yield return new Address{
                    City = "Austin"
                };
                yield return new Address{
                    City = "Dallas"
                };
                yield return new Address{
                    City = "Houston"
                };
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

        [Test]
        public void create_feed_source_type_for_direct_enumerable()
        {
            var node = new FeedWriterNode<Address>(new Feed<Address>(), FeedSourceType.direct,
                                                   typeof (AddressValuesEnumerable));

            node.FeedSourceType.ShouldEqual(typeof (DirectFeedSource<AddressValuesEnumerable, Address>));
        }

        [Test]
        public void create_feed_source_type_for_enumerable_source()
        {
            var node = new FeedWriterNode<Address>(new Feed<Address>(), FeedSourceType.enumerable,
                                                   typeof (AddressEnumerable));

            node.FeedSourceType.ShouldEqual(typeof (EnumerableFeedSource<AddressEnumerable, Address>));
        }

        [Test]
        public void build_object_def_has_correct_feed_writer_type()
        {
            var objectDef = new FeedWriterNode<Address>(new Feed<Address>(), FeedSourceType.enumerable,
                                                    typeof(AddressEnumerable)).ToObjectDef(DiagnosticLevel.None);


            objectDef.Type.ShouldEqual(typeof (FeedWriter<Address>));
        }

        [Test]
        public void has_a_dependency_for_the_ifeeddefinition()
        {
            var theFeed = new Feed<Address>();
            var objectDef = new FeedWriterNode<Address>(theFeed, FeedSourceType.enumerable,
                                                    typeof(AddressEnumerable)).ToObjectDef(DiagnosticLevel.None);
        
        
            objectDef.DependencyFor<IFeedDefinition<Address>>()
                .ShouldBeOfType<ValueDependency>().Value.ShouldBeTheSameAs(theFeed);
        }

        [Test]
        public void has_a_dependency_for_the_feed_source()
        {
            var theFeed = new Feed<Address>();
            var theNode = new FeedWriterNode<Address>(theFeed, FeedSourceType.enumerable,
                                                             typeof(AddressEnumerable));
            var objectDef = theNode.ToObjectDef(DiagnosticLevel.None);

            objectDef.DependencyFor<IFeedSource<Address>>()
                .ShouldBeOfType<ConfiguredDependency>()
                .Definition.Type.ShouldEqual(theNode.FeedSourceType);
        }
    }
}