using System.Collections.Generic;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Combination
{
    [TestFixture]
    public class AssetFileCombinationTester
    {
        private readonly IEnumerable<AssetFile> _files = new[]{
            new AssetFile("jquery.js"),
            new AssetFile("script1.js"),
            new AssetFile("script2.js")
        };

        [Test]
        public void append_the_ultimate_extension_to_the_name()
        {
            // extension = ".js"
            var combination1 = new ScriptFileCombination(_files);
            combination1.Name.ShouldEndWith(".js");
        }

        [Test]
        public void name_is_consistent()
        {
            var combination1 = new ScriptFileCombination(_files);
            var combination2 = new ScriptFileCombination(_files);

            combination1.Name.ShouldEqual(combination2.Name);
        }

        [Test]
        public void use_the_folder_if_it_exists_for_the_name()
        {
            var combination1 = new StyleFileCombination("f1", _files);
            var combination2 = new StyleFileCombination("f1/f2", _files);


            combination1.Name.ShouldStartWith("f1/");
            combination2.Name.ShouldStartWith("f1/f2/");
        }
    }
}