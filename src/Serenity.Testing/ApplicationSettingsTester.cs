using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace Serenity.Testing
{
    [TestFixture]
    public class ApplicationSettingsTester
    {
        [Test]
        public void read_from_a_file_by_name()
        {
            new FileSystem().AlterFlatFile("proj1.application", list =>
            {
                list.Add("ApplicationSettings.PhysicalPath=source/proj1");
                list.Add("ApplicationSettings.RootUrl=http://localhost/proj1");
            });

            var settings = ApplicationSettings.ReadByName("proj1");
            settings.PhysicalPath.ShouldEqual("source/proj1");
            settings.RootUrl.ShouldEqual("http://localhost/proj1");
        }
    }
}