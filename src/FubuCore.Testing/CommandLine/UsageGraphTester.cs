using System;
using FubuCore.CommandLine;
using NUnit.Framework;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class UsageGraphTester
    {
        private UsageGraph theUsageGraph;

        [SetUp]
        public void SetUp()
        {
            theUsageGraph = new UsageGraph(typeof (FakeLinkCommand));
        }

        [Test]
        public void has_the_command_name()
        {
            theUsageGraph.CommandName.ShouldEqual("link");
        }

        [Test]
        public void has_the_description()
        {
            theUsageGraph.Description.ShouldEqual("Manage links");
        }

        [Test]
        public void has_both_usages()
        {
            theUsageGraph.Usages.Select(x => x.UsageKey).OrderBy(x => x).ShouldHaveTheSameElementsAs("link", "list");
        }

        [Test]
        public void has_the_arguments()
        {
            theUsageGraph.Arguments.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("AppFolder", "PackageFolder");
        }

        [Test]
        public void has_the_flags()
        {
            theUsageGraph.Flags.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("RemoveFlag", "CleanAllFlag", "NotepadFlag");
        }

        [Test]
        public void first_usage_has_all_the_right_mandatories()
        {
            theUsageGraph.FindUsage("list").Arguments.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("AppFolder");
        }

        [Test]
        public void first_usage_has_all_the_right_flags()
        {
            theUsageGraph.FindUsage("list").ValidFlags.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("CleanAllFlag", "NotepadFlag");
        }

        [Test]
        public void second_usage_has_all_the_right_mandatories()
        {
            theUsageGraph.FindUsage("link").Arguments.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("AppFolder", "PackageFolder");
        }

        [Test]
        public void second_usage_has_all_the_right_flags()
        {
            theUsageGraph.FindUsage("link").ValidFlags.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("RemoveFlag", "CleanAllFlag", "NotepadFlag");
        }

        [Test]
        public void get_the_description_of_both_usages()
        {
            theUsageGraph.FindUsage("list").Description.ShouldEqual("List the links");
            theUsageGraph.FindUsage("link").Description.ShouldEqual("Link an application folder to a package folder");
        }

        [Test]
        public void get_the_command_usage_of_the_list_usage()
        {
            theUsageGraph.FindUsage("list").Usage.ShouldEqual("fubu link <appfolder> [-cleanall] [-notepad]");
        }

        [Test]
        public void get_the_command_usage_of_the_link_usage()
        {
            theUsageGraph.FindUsage("link").Usage.ShouldEqual("fubu link <appfolder> <packagefolder> [-r] [-cleanall] [-notepad]");
        }

        [Test]
        public void smoke_test_writing_usage()
        {
            theUsageGraph.WriteUsages();
        }

        [Test]
        public void derive_a_single_usage_for_any_command_that_has_no_specific_usages()
        {
            var graph = new UsageGraph(typeof (SimpleCommand));
            var usage = graph.Usages.Single();
            usage.Description.ShouldEqual(typeof (SimpleCommand).GetAttribute<CommandDescriptionAttribute>().Description);
            usage.Arguments.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("Arg1", "Arg2");
        }
    }

    [TestFixture]
    public class valid_usage_tester
    {
        private UsageGraph theUsageGraph;

        [SetUp]
        public void SetUp()
        {
            theUsageGraph = new UsageGraph(typeof (FakeLinkCommand));
        }

        private bool isValidUsage(params string[] args)
        {
            var handlers = theUsageGraph.Handlers.Where(x => args.Contains(x.PropertyName));
            return theUsageGraph.IsValidUsage(handlers);
        }

        [Test]
        public void valid_with_all_required_arguments()
        {
            isValidUsage("AppFolder").ShouldBeTrue();
            isValidUsage("AppFolder", "PackageFolder").ShouldBeTrue();
        }

        [Test]
        public void invalid_with_missing_arguments()
        {
            isValidUsage().ShouldBeFalse();
        }

        [Test]
        public void invalid_flags()
        {
            isValidUsage("AppFolder", "RemoveFlag").ShouldBeFalse();
        }
    }
    
    public class FakeLinkInput
    {
        [RequiredUsage("list", "link"), Description("The root directory of the web folder")]
        public string AppFolder { get; set; }

        [RequiredUsage("link"), Description("The root directory of a package project")]
        public string PackageFolder { get; set; }

        [Description("Removes the link from the application to the package")]
        [ValidUsage("link")]
        [FlagAlias("r")]
        public bool RemoveFlag { get; set; }

        [Description("Removes all links from the application folder")]
        public bool CleanAllFlag { get; set; }

        [Description("Opens the application manifest in notepad")]
        public bool NotepadFlag { get; set; }
    }

    [Usage("list", "List the links")]
    [Usage("link", "Link an application folder to a package folder")]
    [CommandDescription("Manage links", Name = "link")]
    public class FakeLinkCommand : FubuCommand<FakeLinkInput>
    {
        public override void Execute(FakeLinkInput input)
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleInput
    {
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
    }

    [CommandDescription("does simple thing")]
    public class SimpleCommand : FubuCommand<SimpleInput>
    {
        public override void Execute(SimpleInput input)
        {
            throw new NotImplementedException();
        }
    }
}