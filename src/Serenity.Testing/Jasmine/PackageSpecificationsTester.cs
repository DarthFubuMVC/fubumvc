using NUnit.Framework;
using Serenity.Jasmine;
using FubuTestingSupport;

namespace Serenity.Testing.Jasmine
{


    [TestFixture]
    public class SpecificationFolderTester
    {
        [Test]
        public void find_child_folder()
        {
            var folder = new SpecificationFolder("pak1");
            var child = folder.ChildFolderFor("f1/f2/f3");

            child.FullName.ShouldEqual("pak1/f1/f2/f3");
            child.Parent.FullName.ShouldEqual("pak1/f1/f2");
            child.Parent.Parent.FullName.ShouldEqual("pak1/f1");
            child.Parent.Parent.Parent.FullName.ShouldEqual("pak1");
        }
    }
}