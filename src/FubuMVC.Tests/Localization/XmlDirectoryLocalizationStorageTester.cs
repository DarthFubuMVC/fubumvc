using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;
using FubuCore.Util;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Localization.Basic;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class XmlDirectoryLocalizationStorageTester
    {
        [SetUp]
        public void SetUp()
        {
            var system = new FileSystem();
            system.DeleteDirectory("localization1");
            system.DeleteDirectory("localization2");
            system.DeleteDirectory("localization3");

            system.CreateDirectory("localization1");
            system.CreateDirectory("localization2");
            system.CreateDirectory("localization3");
        }

        // values = "value=display"
        private void write(string directory, CultureInfo culture, string values)
        {
            XmlDirectoryLocalizationStorage.Write(directory, culture, LocalString.ReadAllFrom(values));
        }

        [Test]
        public void GetFileName()
        {
            XmlDirectoryLocalizationStorage.GetFileName(new CultureInfo("en-US"))
                .ShouldBe("en-US.locale.config");

            XmlDirectoryLocalizationStorage.GetFileName(new CultureInfo("en-GB"))
                .ShouldBe("en-GB.locale.config");
        }

        [Test]
        public void save_and_load_local_strings()
        {
            var strings = new List<LocalString>{
                new LocalString("a", "a-display"),
                new LocalString("b", "b-display"),
                new LocalString("c", "c-display"),
                new LocalString("d", "d-display"),
            };

            XmlDirectoryLocalizationStorage.Write("locale.xml", strings);

            XmlDirectoryLocalizationStorage.LoadFrom("locale.xml").ShouldHaveTheSameElementsAs(strings);
        }

        [Test]
        public void save_and_load_local_strings_should_sort_on_key_for_easier_merges()
        {
            var strings = new List<LocalString>{
                new LocalString("c", "c-display"),
                new LocalString("d", "d-display"),
                new LocalString("a", "a-display"),
                new LocalString("b", "b-display"),
            };

            XmlDirectoryLocalizationStorage.Write("locale.xml", strings);

            XmlDirectoryLocalizationStorage.LoadFrom("locale.xml").ShouldHaveTheSameElementsAs(strings.OrderBy(x => x.value));
        }

        [Test]
        public void load_from_a_single_source()
        {
            write("localization1", new CultureInfo("en-US"), @"
                a=us-a
                b=us-b
                c=us-c
");

            write("localization1", new CultureInfo("en-GB"), @"
                a=gb-a
                b=gb-b
                c=gb-c
");

            var source = new XmlDirectoryLocalizationStorage(new string[]{"localization1"});

            source.Load(new CultureInfo("en-US")).ShouldHaveTheSameElementsAs(
                new LocalString("a", "us-a"),
                new LocalString("b", "us-b"),
                new LocalString("c", "us-c")
                );

            source.Load(new CultureInfo("en-GB")).ShouldHaveTheSameElementsAs(
                new LocalString("a", "gb-a"),
                new LocalString("b", "gb-b"),
                new LocalString("c", "gb-c")
                );

        }

        [Test]
        public void load_from_a_multiple_directory()
        {
            write("localization1", new CultureInfo("en-US"), @"
                a=us-a
                b=us-b
                c=us-c
");

            write("localization2", new CultureInfo("en-US"), @"
                d=us-d
                e=us-e
");

            var source = new XmlDirectoryLocalizationStorage(new string[] { "localization1", "localization2", "localization3" });

            source.Load(new CultureInfo("en-US")).ShouldHaveTheSameElementsAs(
                new LocalString("a", "us-a"),
                new LocalString("b", "us-b"),
                new LocalString("c", "us-c"),
                new LocalString("d", "us-d"),
                new LocalString("e", "us-e")
                );

        }

        [Test]
        public void write_missing_with_no_file_there()
        {
            var source = new XmlDirectoryLocalizationStorage(new string[] { "localization1", "localization2", "localization3" });

            source.WriteMissing("a", "us-a", new CultureInfo("en-US"));

            var document = new XmlDocument()
                .FromFile("localization1".AppendPath(XmlDirectoryLocalizationStorage.MissingLocaleConfigFile));

            var element = document.DocumentElement.FirstChild.As<XmlElement>();
            element.GetAttribute("culture").ShouldBe("en-US");
            element.GetAttribute("key").ShouldBe("a");
            element.InnerText.ShouldBe("us-a");


        }

        [Test]
        public void write_with_successive_missings()
        {
            var source = new XmlDirectoryLocalizationStorage(new string[] { "localization1", "localization2", "localization3" });

            source.WriteMissing("a", "us-a", new CultureInfo("en-US"));
            source.WriteMissing("b", "us-b", new CultureInfo("en-US"));

            var document = new XmlDocument()
                .FromFile("localization1".AppendPath(XmlDirectoryLocalizationStorage.MissingLocaleConfigFile));

            var element = document.DocumentElement.LastChild.As<XmlElement>();
            element.GetAttribute("culture").ShouldBe("en-US");
            element.GetAttribute("key").ShouldBe("b");
            element.InnerText.ShouldBe("us-b");
        }

        [Test]
        public void do_not_double_dip_in_missing_when_writing()
        {
            var source = new XmlDirectoryLocalizationStorage(new string[] { "localization1", "localization2", "localization3" });

            source.WriteMissing("a", "us-a", new CultureInfo("en-US"));
            source.WriteMissing("a", "us-a", new CultureInfo("en-US"));
            source.WriteMissing("a", "us-a", new CultureInfo("en-US"));
            source.WriteMissing("a", "us-a", new CultureInfo("en-US"));
            source.WriteMissing("a", "us-a", new CultureInfo("en-US"));

            var document = new XmlDocument()
                .FromFile("localization1".AppendPath(XmlDirectoryLocalizationStorage.MissingLocaleConfigFile));

            document.DocumentElement.ChildNodes.Count.ShouldBe(1);
        }

        [Test]
        public void CultureFor()
        {
            XmlDirectoryLocalizationStorage.CultureFor("en-US.locale.config").ShouldBe(new CultureInfo("en-US"));
            XmlDirectoryLocalizationStorage.CultureFor("en-CA.locale.config").ShouldBe(new CultureInfo("en-CA"));
            XmlDirectoryLocalizationStorage.CultureFor("en-GB.locale.config").ShouldBe(new CultureInfo("en-GB"));
        }

        [Test]
        public void load_all()
        {
            write("localization1", new CultureInfo("en-US"), @"
                a=us-a
                b=us-b
                f=us-f
            ");

            write("localization2", new CultureInfo("en-US"), @"
                c=us-c
                d=us-d
            ");

            write("localization3", new CultureInfo("en-US"), @"
                e=us-e
            ");

            write("localization1", new CultureInfo("en-GB"), @"
                a=gb-a
                b=gb-b
                f=gb-f
            ");

            write("localization2", new CultureInfo("en-GB"), @"
                c=gb-c
                d=gb-d
            ");

            var allKeys = new Cache<string, IList<LocalString>>(key => new List<LocalString>());

            var source = new XmlDirectoryLocalizationStorage(new string[] { "localization1", "localization2", "localization3" });

            source.LoadAll(text => Debug.WriteLine(text), (c, strings) => allKeys[c.Name].AddRange(strings));

            allKeys["en-US"].ShouldHaveTheSameElementsAs(LocalString.ReadAllFrom(@"
                a=us-a
                b=us-b
                f=us-f  
                c=us-c
                d=us-d
                e=us-e
"));

            allKeys["en-GB"].ShouldHaveTheSameElementsAs(LocalString.ReadAllFrom(@"
                a=gb-a
                b=gb-b
                f=gb-f
                c=gb-c
                d=gb-d
"));

        }



        [Test]
        public void merge_missing()
        {
            var source = new XmlDirectoryLocalizationStorage(new string[] { "localization1", "localization2", "localization3" });

            write("localization1", new CultureInfo("en-US"), @"
                a=us-a
                b=us-b
                f=us-f
");

            source.WriteMissing("c", "us-c", new CultureInfo("en-US"));
            source.WriteMissing("d", "us-d", new CultureInfo("en-US"));
            source.WriteMissing("a", "gb-A", new CultureInfo("en-GB"));

            source.MergeAllMissing();

            source.Load(new CultureInfo("en-US")).ShouldHaveTheSameElementsAs(
                LocalString.ReadAllFrom(@"
                    a=us-a
                    b=us-b
                    c=us-c
                    d=us-d
                    f=us-f
                    ")
                );

            source.Load(new CultureInfo("en-GB")).ShouldHaveTheSameElementsAs(new LocalString("a", "gb-A"));

            new XmlDocument().FromFile(
                "localization1".AppendPath(XmlDirectoryLocalizationStorage.MissingLocaleConfigFile))
                .DocumentElement.ChildNodes.Count.ShouldBe(0);

        
        }
    }
}