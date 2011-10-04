using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Resources.Media.Formatters;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Resources.Projections;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;
using System.Linq;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class ConnegOutputNode_building_ObjectDef_Tester
    {
        private ObjectDef _objectDef;
        private ConnegOutputNode theOutputNode;

        private ObjectDef theObjectDef
        {
            get
            {
                if (_objectDef == null)
                {
                    _objectDef = theOutputNode.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);
                }

                return _objectDef;
            }
        }

        private IEnumerable<ObjectDef> theReaderDependencies
        {
            get
            {
                return theObjectDef.EnumerableDependenciesOf<IMediaWriter<Address>>().Items;
            }
        }

        private ObjectDef theFormatterMediaReader
        {
            get
            {
                return theReaderDependencies.SingleOrDefault(x => x.Type == typeof(FormatterMediaWriter<Address>));
            }
        }

        [SetUp]
        public void SetUp()
        {
            _objectDef = null;
            theOutputNode = new ConnegOutputNode(typeof(Address));
        }


        public class StubAddressWriter : IMediaWriter<Address>
        {
            public IEnumerable<string> Mimetypes
            {
                get { throw new NotImplementedException(); }
            }

            public void Write(IValues<Address> source, IOutputWriter writer)
            {
                throw new NotImplementedException();
            }

            public void Write(Address source, IOutputWriter writer)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void with_writers_and_the_writers_are_added_at_the_beginning_of_the_writer_enumerable()
        {
            var WriterDef1 = new ObjectDef(typeof(StubAddressWriter));
            var WriterNode1 = MockRepository.GenerateMock<IMediaWriterNode>();
            WriterNode1.Stub(x => x.InputType).Return(typeof (Address));
            WriterNode1.Stub(x => x.ToObjectDef(DiagnosticLevel.None)).Return(WriterDef1);
            theOutputNode.AddWriter(WriterNode1);

            var WriterDef2 = new ObjectDef(typeof(StubAddressWriter));
            var WriterNode2 = MockRepository.GenerateMock<IMediaWriterNode>();
            WriterNode2.Stub(x => x.InputType).Return(typeof(Address));
            WriterNode2.Stub(x => x.ToObjectDef(DiagnosticLevel.None)).Return(WriterDef2);
            theOutputNode.AddWriter(WriterNode2);


            theReaderDependencies.Count().ShouldEqual(3);
            theReaderDependencies.ElementAt(0).ShouldBeTheSameAs(WriterDef1);
            theReaderDependencies.ElementAt(1).ShouldBeTheSameAs(WriterDef2);
        }

        [Test]
        public void the_object_def_is_of_the_correct_ConnegOutputBehavior_T()
        {
            theObjectDef.Type.ShouldEqual(typeof (ConnegOutputBehavior<Address>));
        }

        [Test]
        public void all_formatters_adds_the_formatter_media_reader_with_no_explicit_formatters()
        {
            theFormatterMediaReader.Dependencies.Any().ShouldBeFalse();
        }

        [Test]
        public void use_no_formatters_and_there_should_not_be_a_formatter_reader_at_all()
        {
            theOutputNode.UseNoFormatters();
            theFormatterMediaReader.ShouldBeNull();
        }

        [Test]
        public void use_specific_formatters()
        {
            theOutputNode.UseFormatter<JsonFormatter>();
            theOutputNode.UseFormatter<XmlFormatter>();

            theFormatterMediaReader.EnumerableDependenciesOf<IFormatter>()
                .Items
                .Select(x => x.Type)
                .ShouldHaveTheSameElementsAs(typeof(JsonFormatter), typeof(XmlFormatter));
        }
    }


    [TestFixture]
    public class ConnegOutputNodeTester
    {

        [Test]
        public void category_is_output()
        {
            var node = new ConnegOutputNode(typeof(Address));
            node.Category.ShouldEqual(BehaviorCategory.Output);
        }

        [Test]
        public void formatter_usage_is_all_by_default()
        {
            var node = new ConnegOutputNode(typeof (Address));
            node.FormatterUsage.ShouldEqual(FormatterUsage.all);
        }


        [Test]
        public void no_readers_by_default()
        {
            var node = new ConnegOutputNode(typeof(Address));
            node.Writers.Any().ShouldBeFalse();
        }

        [Test]
        public void no_selected_formatters_by_default()
        {
            var node = new ConnegOutputNode(typeof(Address));
            node.SelectedFormatterTypes.Any().ShouldBeFalse();
        }

        [Test]
        public void add_a_formatter_changes_the_formatter_usage_to_selected_and_adds_the_reader_to_its_collection()
        {
            var node = new ConnegOutputNode(typeof(Address));
            node.UseFormatter<JsonFormatter>();

            node.FormatterUsage.ShouldEqual(FormatterUsage.selected);

            node.SelectedFormatterTypes.Single().ShouldEqual(typeof (JsonFormatter));
        }

        [Test]
        public void throw_argument_exception_if_media_reader_node_type_is_wrong()
        {
            var Writer = MockRepository.GenerateMock<IMediaWriterNode>();
            Writer.Stub(x => x.InputType).Return(GetType());

            var node = new ConnegOutputNode(typeof(Address));

            Exception<ArgumentException>.ShouldBeThrownBy(() =>
            {
                node.AddWriter(Writer);
            });
        }

        [Test]
        public void calling_no_formatters_sets_the_usage_to_none()
        {
            var node = new ConnegOutputNode(typeof(Address));
            node.UseNoFormatters();

            node.FormatterUsage.ShouldEqual(FormatterUsage.none);
        }

        [Test]
        public void calling_no_formatters_clears_out_any_previously_selected_formatters()
        {

            var node = new ConnegOutputNode(typeof(Address));
            node.UseFormatter<JsonFormatter>();

            node.UseNoFormatters();

            node.SelectedFormatterTypes.Any().ShouldBeFalse();
        }

        [Test]
        public void use_all_formatters_sets_the_usage_to_all()
        {
            var node = new ConnegOutputNode(typeof(Address));
            node.UseNoFormatters();
            node.UseAllFormatters();

            node.FormatterUsage.ShouldEqual(FormatterUsage.all);
        }

        [Test]
        public void use_all_formatters_clears_out_any_previously_selected_formatters()
        {
            var node = new ConnegOutputNode(typeof(Address));
            node.UseFormatter<JsonFormatter>();

            node.UseAllFormatters();

            node.SelectedFormatterTypes.Any().ShouldBeFalse();
        }

    }
}