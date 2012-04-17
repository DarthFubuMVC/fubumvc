using System.Collections;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Media.Atom;
using FubuMVC.Media.Projections;
using FubuMVC.Media.Testing.Xml;
using FubuTestingSupport;
using NUnit.Framework;

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
        public void build_object_def_has_correct_feed_writer_type()
        {
            var objectDef = new FeedWriterNode<Address>(new Feed<Address>(), typeof (AddressEnumerable))
                .As<IContainerModel>()
                .ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IMediaWriter<IEnumerable<Address>>>();


            objectDef.Type.ShouldEqual(typeof (FeedWriter<Address>));
        }

        [Test]
        public void has_a_dependency_for_the_ifeeddefinition()
        {
            var theFeed = new Feed<Address>();
            var objectDef = new FeedWriterNode<Address>(theFeed,typeof (AddressEnumerable))
                .As<IContainerModel>()
                .ToObjectDef(DiagnosticLevel.None)
                .FindDependencyDefinitionFor<IMediaWriter<IEnumerable<Address>>>();


            objectDef.DependencyFor<IFeedDefinition<Address>>()
                .ShouldBeOfType<ValueDependency>().Value.ShouldBeTheSameAs(theFeed);
        }
    }
}