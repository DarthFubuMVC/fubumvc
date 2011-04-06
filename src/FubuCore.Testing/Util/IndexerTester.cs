using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Util
{
    [TestFixture]
    public class IndexerTester
    {
        private Guid _guid;
        private IDictionary<string, Guid> _dictionary;
        private Indexer<string, Guid> _indexer;
        private string _key;

        [SetUp]
        public void SetUp()
        {
            _dictionary = new Dictionary<string, Guid>();
            _guid = Guid.NewGuid();
            _key = "some key";
            _dictionary.Add(_key, _guid);
            _indexer = new Indexer<string, Guid>(s => _dictionary[s], (s, g) => _guid = g);
        }

        [Test]
        public void indexer_should_set()
        {
            Guid value = Guid.NewGuid();
            _indexer[_key] = value;
            _guid.ShouldEqual(value);
        }

        [Test]
        public void indexer_should_get()
        {
            _indexer[_key].ShouldEqual(_guid);
        }
    }
}