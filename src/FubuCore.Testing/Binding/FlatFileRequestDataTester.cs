using System.Collections.Generic;
using System.IO;
using FubuCore.Binding;
using FubuMVC.StructureMap;
using NUnit.Framework;
using StructureMap;
using System.Linq;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class FlatFileRequestDataTester
    {
        private FlatFileRequestData request;

        [SetUp]
        public void SetUp()
        {
            request = new FlatFileRequestData("|", "a|b|c|d");
        }

        [Test]
        public void read_without_aliases()
        {
            request.ReadLine("1|2|3|4");
            request.Value("a").ShouldEqual("1");
            request.Value("b").ShouldEqual("2");
            request.Value("c").ShouldEqual("3");
            request.Value("d").ShouldEqual("4");
        }

        [Test]
        public void read_with_the_value_callback_method()
        {
            string value = null;
            request.ReadLine("1|2|3|4");
            request.Value("a", o => value = (string) o);

            value.ShouldEqual("1");
        }

        [Test]
        public void read_with_alias()
        {
            request.Alias("a", "aaa");
            string value = null;
            request.ReadLine("1|2|3|4");
            request.Value("aaa", o => value = (string)o);

            value.ShouldEqual("1");
        }
    }

    [TestFixture]
    public class FlatFileReaderIntegratedTester
    {
        private FlatFileReader<FlatFileReaderTarget> reader;

        [SetUp]
        public void SetUp()
        {
            using (var writer = new StreamWriter("flatfile.txt"))
            {
                writer.WriteLine("A|B|C|D");
                writer.WriteLine("a0|b0|c0|d0");
                writer.WriteLine("a1|b1|c1|d1");
                writer.WriteLine("a2|b2|c2|d2");
            }

            var container = new Container(new AppSettingProviderRegistry());
            reader = container.GetInstance<FlatFileReader<FlatFileReaderTarget>>();
        }

        [Test]
        public void read_from_flat_file_with_no_aliases()
        {
            var list = new List<FlatFileReaderTarget>();
            reader.ReadFile(new FlatFileRequest<FlatFileReaderTarget>()
                            {
                                Filename = "flatfile.txt",
                                Callback = list.Add,
                                Concatenator = "|",
                                Finder = data => new FlatFileReaderTarget(){Name = data.Value("A").ToString()}
                            });


            list.Count.ShouldEqual(3);


            list[0].A.ShouldEqual("a0");
            list[0].B.ShouldEqual("b0");
            list[0].C.ShouldEqual("c0");
            list[0].D.ShouldEqual("d0");
            list[0].Name.ShouldEqual("a0");
        }

        [Test]
        public void read_from_flat_file_with_aliases()
        {
            var list = new List<FlatFileReaderTarget>();
            reader.Alias("A", "AliasedA");
            reader.ReadFile(new FlatFileRequest<FlatFileReaderTarget>()
            {
                Filename = "flatfile.txt",
                Callback = list.Add,
                Concatenator = "|",
                Finder = data => new FlatFileReaderTarget() { Name = data.Value("A").ToString() }
            });

            var first = list.First();

            first.AliasedA.ShouldEqual("a0");
        }
    }

    public class FlatFileReaderTarget
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }

        public string AliasedA { get; set; }
        public string Name { get; set; }
    }
}