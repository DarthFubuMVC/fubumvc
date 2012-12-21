using System.Diagnostics;
using System.IO;
using FubuCore;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime.Files;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime.Files
{
    [TestFixture]
    public class ContentFolderTester
    {
        private ContentFolder theFolder;

        [SetUp]     
        public void SetUp()
        {
            theFolder = ContentFolder.ForApplication();
        }

        [Test]
        public void sets_the_folder_path()
        {
            theFolder.Path.ShouldEqual(FubuMvcPackageFacility.GetApplicationPath());
        }

        [Test]
        public void sets_the_provenance_of_the_application()
        {
            theFolder.Provenance.ShouldEqual(ContentFolder.Application);
        }

        [Test]
        public void can_find_files()
        {
            var files = theFolder.FindFiles(new FileSet{
                DeepSearch = true,
                Include = "*.txt"
            }).Select(x => x.Path.PathRelativeTo(FubuMvcPackageFacility.GetApplicationPath()));

            files.ShouldContain(@"Runtime{0}Files{0}Data{0}a.txt".ToFormat(Path.DirectorySeparatorChar));
            files.ShouldContain(@"Runtime{0}Files{0}Data{0}b.txt".ToFormat(Path.DirectorySeparatorChar));
            files.ShouldContain(@"Runtime{0}Files{0}Data{0}c.txt".ToFormat(Path.DirectorySeparatorChar));
            files.ShouldContain(@"Runtime{0}Files{0}Data{0}d.txt".ToFormat(Path.DirectorySeparatorChar));
        }

        [Test]
        public void provenance_path_is_set_on_files()
        {
            theFolder.FindFiles(new FileSet
            {
                DeepSearch = true,
                Include = "*.txt"
            }).Each(f => f.ProvenancePath.ShouldEqual(FubuMvcPackageFacility.GetApplicationPath()));
        }

    }
}