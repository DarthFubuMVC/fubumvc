using System;
using System.Diagnostics;
using Fubu.Generation;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;
using System.Collections.Generic;

namespace fubu.Testing.Generation
{
    [TestFixture]
    public class SolutionFinderTester
    {
        private string theCurrentEnvironment;
        private IFileSystem fileSystem = new FileSystem();

        [SetUp]
        public void SetUp()
        {
            theCurrentEnvironment = Environment.CurrentDirectory;

        }

        [TearDown]
        public void TearDown()
        {
            Environment.CurrentDirectory = theCurrentEnvironment;
        }

        [Test]
        public void find_the_one_and_only_solution()
        {
            fileSystem.CreateDirectory("only-one");
            fileSystem.WriteStringToFile("only-one".AppendPath("anything.sln"), "whatever");
            
            Environment.CurrentDirectory = Environment.CurrentDirectory.AppendPath("only-one").ToFullPath();

            SolutionFinder.FindSolutionFile().ShouldEqual("anything.sln");
        }

        [Test]
        public void find_multiples_so_it_is_indeterminate()
        {
            fileSystem.CreateDirectory("multiples");
            fileSystem.CleanDirectory("multiples");
            fileSystem.WriteStringToFile("multiples".AppendPath("anything.sln"), "whatever");
            fileSystem.WriteStringToFile("multiples".AppendPath("else.sln"), "whatever");

            Environment.CurrentDirectory = Environment.CurrentDirectory.AppendPath("multiples").ToFullPath();


            SolutionFinder.FindSolutionFile().ShouldBeNull();
        
            // find the multiples too
            var solutions = SolutionFinder.FindSolutions();
            solutions.ShouldHaveTheSameElementsAs(
                "anything.sln", "else.sln");
        }
    }
}