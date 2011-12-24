using System.Collections.Generic;
using FubuCore;
using FubuMVC.Spark.SparkModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;

namespace FubuMVC.Spark.Tests.SparkModel.Scanning
{
    [TestFixture]
    public class ScanRequestTester : InteractionContext<ScanRequest>
    {
        private FileFound _ff;

        protected override void beforeEach()
        {
            _ff = new FileFound("a.spark", "a", "");
        }

        [Test]
        public void include_spark_views_adds_correct_filter()
        {
            ClassUnderTest.IncludeSparkViews();
            ClassUnderTest.Filters.ShouldContain("*{0}".ToFormat(Constants.DotSpark));
        }

        [Test]
        public void add_root_puts_the_root_into_the_roots_collection()
        {
            ClassUnderTest.Roots.ShouldEqual((new[] { "a", "b" }).Each(ClassUnderTest.AddRoot));
        }

        [Test]
        public void include_puts_the_filter_into_the_filters_expression()
        {
            new[] { "*.spark", "*.view", "*.template" }.Each(ClassUnderTest.Include);
            ClassUnderTest.Filters.ShouldEqual("*.spark;*.view;*.template");
        }

        [Test]
        public void exclude_directory_puts_the_directory_into_the_excluded_directories_collection()
        {
            ClassUnderTest.ExcludedDirectories.ShouldEqual((new[] { "bin", "obj" }.Each(ClassUnderTest.ExcludeDirectory)));
        }

        [Test]
        public void add_handler_puts_the_handler_into_the_onfound_list_and_is_called_from_on_found_method()
        {
            bool flag1 = false, flag2 = false;
            ClassUnderTest.AddHandler(x => flag1 = true);
            ClassUnderTest.AddHandler(x => flag2 = true);
            ClassUnderTest.OnFound(_ff);
            flag1.ShouldEqual(flag2).ShouldEqual(true);
        }
        [Test]
        public void on_found_handler_arg_is_the_file_found_instance_passed_to_the_on_found_method()
        {
            FileFound handlerArg = null;
            ClassUnderTest.AddHandler(x => handlerArg = x);
            ClassUnderTest.OnFound(_ff);
            handlerArg.ShouldEqual(_ff);
        }
    }
}
