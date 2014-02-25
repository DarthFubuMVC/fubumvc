using System;
using System.Collections;
using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class ReaderCollectionTester
    {
        private Lazy<ReaderCollection<Address>> _collection;
        private InputNode theInput;

        [SetUp]
        public void SetUp()
        {
            theInput = new InputNode(typeof (Address));

            _collection = new Lazy<ReaderCollection<Address>>(() => new ReaderCollection<Address>(theInput));
        }

        private ReaderCollection<Address> ClassUnderTest
        {
            get
            {
                return _collection.Value;
            }
        } 

        private IReader<Address> readerFor(params string[] mimeTypes)
        {
            var reader = MockRepository.GenerateMock<IReader<Address>>();
            reader.Stub(x => x.Mimetypes).Return(mimeTypes);

            theInput.Add(reader);

            return reader;
        }

        [Test]
        public void select_reader_simple()
        {
            var reader1 = readerFor("text/json", "application/json");
            var reader2 = readerFor("text/xml");
            var reader3 = readerFor("text/xml", "application/xml");
            var reader4 = readerFor("text/html");

            ClassUnderTest.ChooseReader(new CurrentMimeType("text/json", ""), null)
                .ShouldBeTheSameAs(reader1);

            ClassUnderTest.ChooseReader(new CurrentMimeType("text/html", ""), null)
                .ShouldBeTheSameAs(reader4);

            ClassUnderTest.ChooseReader(new CurrentMimeType("application/xml", ""), null)
                .ShouldBeTheSameAs(reader3);

            ClassUnderTest.ChooseReader(new CurrentMimeType("text/xml", ""), null)
                .ShouldBeTheSameAs(reader2);
        }

        [Test]
        public void select_reader_complex()
        {
            var reader1 = readerFor("text/json", "application/json");
            var reader2 = readerFor("text/xml");
            var reader3 = readerFor("text/xml", "application/xml");
            var reader4 = readerFor("text/html");

            ClassUnderTest.ChooseReader(new CurrentMimeType("text/json", ""), null)
                .ShouldBeTheSameAs(reader1);



            ClassUnderTest.ChooseReader(new CurrentMimeType("text/html", ""), null)
                .ShouldBeTheSameAs(reader4);
        }
    }
}