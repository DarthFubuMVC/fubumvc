using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class SharedPathBuilderTester
    {
        private readonly SharedPathBuilder _classUnderTest;
        private readonly IEnumerable<string> _folderNames = new[] { "Shared1", "Shared2" };
        private readonly string _rootPath = AppDomain.CurrentDomain.BaseDirectory;

        public SharedPathBuilderTester()
        {
            _classUnderTest = new SharedPathBuilder(_folderNames);
        }

        [Test]
        public void path_zero_level_returns_empty()
        {
            _classUnderTest.BuildFrom(_rootPath, _rootPath).ShouldHaveCount(0);
        }

        [Test]
        public void path_one_level_returns_correct_shared_paths()
        {
            var expected = new List<string>();
            _folderNames.Each(s => expected.Add(FileSystem.Combine(_rootPath, s)));

            var path = FileSystem.Combine(_rootPath, "path1");
            
            _classUnderTest.BuildFrom(path, _rootPath)
                .ShouldHaveTheSameElementsAs(expected);
        }


        [Test]
        public void path_two_level_returns_correct_shared_paths()
        {
            var p1 = FileSystem.Combine(_rootPath, "path1");
            var p2 = FileSystem.Combine(p1, "path2");
            var paths = new List<string> { p1, _rootPath };
            
            var expected = new List<string>();
            paths.Each(p => _folderNames.Each(s => expected.Add(FileSystem.Combine(p, s))));

            _classUnderTest.BuildFrom(p2, _rootPath).ShouldHaveTheSameElementsAs(expected);
        }

        [Test]
        public void path_n_level_returns_correct_shared_paths()
        {

        }
		
        // TODO: More to come -> edge cases + expected behavior
    }
}