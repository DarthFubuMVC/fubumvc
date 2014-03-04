
using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.View.Model;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class SharedPathBuilderTester : InteractionContext<SharedPathBuilder>
    {
        private readonly IEnumerable<string> _folderNames;
        private readonly string _rootPath;

        public SharedPathBuilderTester()
        {
            _folderNames = new[] { "Shared1", "Shared2" };
            _rootPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        protected override void beforeEach()
        {
            Services.Inject(new SharedPathBuilder(_folderNames));
        }

        [Test]
        public void system_root_yields_empty()
        {
            var systemRoot = Path.GetPathRoot(_rootPath);

            ClassUnderTest.BuildBy(systemRoot, systemRoot, false)
                .ShouldHaveCount(0);
        }

        [Test]
        public void path_zero_level_returns_empty()
        {
            ClassUnderTest.BuildBy(_rootPath, _rootPath, false)
                .ShouldHaveCount(0);
        }

        [Test]
        public void path_one_level_returns_correct_shared_paths()
        {
            var path = FileSystem.Combine(_rootPath, "path1");

            var expected = new List<string>();
            _folderNames.Each(s => expected.Add(FileSystem.Combine(_rootPath, s)));

            ClassUnderTest.BuildBy(path, _rootPath, false)
                .ShouldHaveTheSameElementsAs(expected);
        }

        [Test]
        public void path_one_level_with_includeAncestor_bool_flag_returns_correct_paths()
        {
            var path = FileSystem.Combine(_rootPath, "path1");

            var expected = new List<string> {_rootPath};

            _folderNames.Each(s => expected.Add(FileSystem.Combine(_rootPath, s)));

            ClassUnderTest.BuildBy(path, _rootPath, true)
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

            ClassUnderTest.BuildBy(p2, _rootPath, false)
                .ShouldHaveTheSameElementsAs(expected);
        }



        [Test]
        public void path_two_level_with_includeAncestor_bool_flag_returns_correct_paths()
        {
            var p1 = FileSystem.Combine(_rootPath, "path1");
            var p2 = FileSystem.Combine(p1, "path2");
            var paths = new List<string> { p1, _rootPath };

            var expected = new List<string>();
            paths.Each(p =>
            {
                expected.Add(p);
                _folderNames.Each(s => expected.Add(FileSystem.Combine(p, s)));
            });

            ClassUnderTest.BuildBy(p2, _rootPath, true)
                .ShouldHaveTheSameElementsAs(expected);
        }

        [Test]
        public void path_n_level_returns_correct_shared_paths()
        {
            var tree = _rootPath;
            var paths = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                paths.Insert(0, tree);
                tree = FileSystem.Combine(tree, "p" + i);
            }

            var expected = new List<string>();
            paths.Each(p => _folderNames.Each(s => expected.Add(FileSystem.Combine(p, s))));

            ClassUnderTest.BuildBy(tree, _rootPath, false)
                .ShouldHaveTheSameElementsAs(expected);
        }

        [Test]
        public void path_n_level_with_includeAncestor_bool_flag_returns_correct_paths()
        {
            var tree = _rootPath;
            var paths = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                paths.Insert(0, tree);
                tree = FileSystem.Combine(tree, "p" + i);
            }

            var expected = new List<string>();
            paths.Each(p =>
            {
                expected.Add(p);
                _folderNames.Each(s => expected.Add(FileSystem.Combine(p, s)));
            });

            ClassUnderTest.BuildBy(tree, _rootPath, true)
                .ShouldHaveTheSameElementsAs(expected);
        }

    }
}