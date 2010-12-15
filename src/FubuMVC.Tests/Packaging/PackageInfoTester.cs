using System.Collections.Generic;
using System.IO;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using NUnit.Framework;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class PackageInfoTester
    {
        private PackageInfo thePackage;

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists("data"))
            {
                Directory.Delete("data", true);
            }

            thePackage = new PackageInfo("something");
            thePackage.RegisterFolder(PackageInfo.DataFolder, Path.GetFullPath("data"));
        }


        private void writeText(string name, string text)
        {
            var directory = Path.GetDirectoryName(name);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(name))
            {
                File.Delete(name);
            }

            //if (name.Contains("/"))
            //{
                
            //}

            File.WriteAllText(name, text);
        }
        
        public IEnumerable<string> readFiles(string searchString)
        {
            var list = new List<string>();
            thePackage.ForData(searchString, (name, stream) => list.Add(stream.ReadAllText()));

            list.Sort();

            return list;
        }

        [Test]
        public void read_data_with_just_the_extension()
        {
            writeText("data/a.txt", "a");
            writeText("data/b.txt", "b");
            writeText("data/c.txt", "c");
            writeText("data/d.t2", "d");
            writeText("data/e.t2", "e");

            readFiles("*.txt").ShouldHaveTheSameElementsAs("a", "b", "c");
            readFiles("*.t2").ShouldHaveTheSameElementsAs("d", "e");
        }

        [Test]
        public void read_data_from_a_folder_and_extension()
        {
            writeText("data/st/a.txt", "a");
            writeText("data/st/b.txt", "b");
            writeText("data/c.txt", "c");
            writeText("data/st/d.t2", "d");
            writeText("data/e.t2", "e");

            readFiles("*.txt").ShouldHaveTheSameElementsAs("a", "b", "c");
            readFiles("*.t2").ShouldHaveTheSameElementsAs("d", "e");

            readFiles("st/*.txt").ShouldHaveTheSameElementsAs("a", "b");
            readFiles("st/*.t2").ShouldHaveTheSameElementsAs("d");
        }
    }
}