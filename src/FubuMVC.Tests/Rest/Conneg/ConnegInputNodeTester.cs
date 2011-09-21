using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Conneg;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Formatters;
using FubuMVC.Tests.Rest.Projections;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Rest.Conneg
{
    [TestFixture]
    public class ConnegInputNode_building_ObjectDef_Tester
    {
        private ObjectDef _objectDef;
        private ConnegInputNode theInputNode;

        private ObjectDef theObjectDef
        {
            get
            {
                if (_objectDef == null)
                {
                    _objectDef = theInputNode.As<IContainerModel>().ToObjectDef();
                }

                return _objectDef;
            }
        }

        private IEnumerable<ObjectDef> theReaderDependencies
        {
            get
            {
                return theObjectDef.EnumerableDependenciesOf<IMediaReader<Address>>().Items;
            }
        }

        private ObjectDef theFormatterMediaReader
        {
            get
            {
                return theReaderDependencies.SingleOrDefault(x => x.Type == typeof(FormatterMediaReader<Address>));
            }
        }

        [SetUp]
        public void SetUp()
        {
            _objectDef = null;
            theInputNode = new ConnegInputNode(typeof(Address));
        }

        public class StubAddressReader : IMediaReader<Address>
        {
            public IEnumerable<string> Mimetypes
            {
                get { throw new NotImplementedException(); }
            }

            public Address Read(string mimeType)
            {
                throw new NotImplementedException();
            }
        }


        [Test]
        public void with_readers_and_the_readers_are_added_at_the_beginning_of_the_reader_enumerable()
        {
            var readerDef1 = new ObjectDef(typeof(StubAddressReader));
            var readerNode1 = MockRepository.GenerateMock<IMediaReaderNode>();
            readerNode1.Stub(x => x.InputType).Return(typeof (Address));
            readerNode1.Stub(x => x.ToObjectDef()).Return(readerDef1);
            theInputNode.AddReader(readerNode1);

            var readerDef2 = new ObjectDef(typeof(StubAddressReader));
            var readerNode2 = MockRepository.GenerateMock<IMediaReaderNode>();
            readerNode2.Stub(x => x.InputType).Return(typeof(Address));
            readerNode2.Stub(x => x.ToObjectDef()).Return(readerDef2);
            theInputNode.AddReader(readerNode2);


            theReaderDependencies.Count().ShouldEqual(4);
            theReaderDependencies.ElementAt(0).ShouldBeTheSameAs(readerDef1);
            theReaderDependencies.ElementAt(1).ShouldBeTheSameAs(readerDef2);
        }

        [Test]
        public void the_object_def_is_of_the_correct_ConnegInputBehavior_T()
        {
            theObjectDef.Type.ShouldEqual(typeof (ConnegInputBehavior<Address>));
        }



        [Test]
        public void if_allow_http_form_post_should_be_the_ModelBindingMediaReader_in_the_media_readers()
        {
            theInputNode.AllowHttpFormPosts = true;
            theReaderDependencies.Single(x => x.Type == typeof (ModelBindingMediaReader<Address>))
                .ShouldNotBeNull();
        }

        [Test]
        public void if_allow_http_form_post_is_false_there_should_not_be_a_ModelBindingMediaReader()
        {
            theInputNode.AllowHttpFormPosts = false;
            theReaderDependencies.Any(x => x.Type == typeof (ModelBindingMediaReader<Address>))
                .ShouldBeFalse();
        }

        [Test]
        public void all_formatters_adds_the_formatter_media_reader_with_no_explicit_formatters()
        {
            theFormatterMediaReader.Dependencies.Any().ShouldBeFalse();
        }

        [Test]
        public void use_no_formatters_and_there_should_not_be_a_formatter_reader_at_all()
        {
            theInputNode.UseNoFormatters();
            theFormatterMediaReader.ShouldBeNull();
        }

        [Test]
        public void use_specific_formatters()
        {
            theInputNode.UseFormatter<JsonFormatter>();
            theInputNode.UseFormatter<XmlFormatter>();

            theFormatterMediaReader.EnumerableDependenciesOf<IFormatter>()
                .Items
                .Select(x => x.Type)
                .ShouldHaveTheSameElementsAs(typeof(JsonFormatter), typeof(XmlFormatter));
        }
    }


    [TestFixture]
    public class ConnegInputNodeTester
    {
        [Test]
        public void formatter_usage_is_all_by_default()
        {
            var node = new ConnegInputNode(typeof (Address));
            node.FormatterUsage.ShouldEqual(FormatterUsage.all);
        }

        [Test]
        public void http_posts_are_allowed_by_default()
        {
            var node = new ConnegInputNode(typeof(Address));
            node.AllowHttpFormPosts.ShouldBeTrue();
        }

        [Test]
        public void no_readers_by_default()
        {
            var node = new ConnegInputNode(typeof(Address));
            node.Readers.Any().ShouldBeFalse();
        }

        [Test]
        public void no_selected_formatters_by_default()
        {
            var node = new ConnegInputNode(typeof(Address));
            node.SelectedFormatterTypes.Any().ShouldBeFalse();
        }

        [Test]
        public void add_a_formatter_changes_the_formatter_usage_to_selected_and_adds_the_reader_to_its_collection()
        {
            var node = new ConnegInputNode(typeof(Address));
            node.UseFormatter<JsonFormatter>();

            node.FormatterUsage.ShouldEqual(FormatterUsage.selected);

            node.SelectedFormatterTypes.Single().ShouldEqual(typeof (JsonFormatter));
        }

        [Test]
        public void throw_argument_exception_if_media_reader_node_type_is_wrong()
        {
            var reader = MockRepository.GenerateMock<IMediaReaderNode>();
            reader.Stub(x => x.InputType).Return(GetType());

            var node = new ConnegInputNode(typeof(Address));

            Exception<ArgumentException>.ShouldBeThrownBy(() =>
            {
                node.AddReader(reader);
            });
        }

        [Test]
        public void calling_no_formatters_sets_the_usage_to_none()
        {
            var node = new ConnegInputNode(typeof(Address));
            node.UseNoFormatters();

            node.FormatterUsage.ShouldEqual(FormatterUsage.none);
        }

        [Test]
        public void calling_no_formatters_clears_out_any_previously_selected_formatters()
        {

            var node = new ConnegInputNode(typeof(Address));
            node.UseFormatter<JsonFormatter>();

            node.UseNoFormatters();

            node.SelectedFormatterTypes.Any().ShouldBeFalse();
        }

        [Test]
        public void use_all_formatters_sets_the_usage_to_all()
        {
            var node = new ConnegInputNode(typeof(Address));
            node.UseNoFormatters();
            node.UseAllFormatters();

            node.FormatterUsage.ShouldEqual(FormatterUsage.all);
        }

        [Test]
        public void use_all_formatters_clears_out_any_previously_selected_formatters()
        {
            var node = new ConnegInputNode(typeof(Address));
            node.UseFormatter<JsonFormatter>();

            node.UseAllFormatters();

            node.SelectedFormatterTypes.Any().ShouldBeFalse();
        }

    }
}