using Fubu.Running;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace fubu.Testing.Running
{
    [TestFixture]
    public class FileMatcherTester
    {
        private FileMatcher theMatcher;

        [SetUp]
        public void SetUp()
        {
            theMatcher = new FileMatcher();

            theMatcher.Add(new EndsWithPatternMatch(FileChangeCategory.Application, "*.asset.config"));
            theMatcher.Add(new ExtensionMatch(FileChangeCategory.Content, "*.css"));
            theMatcher.Add(new ExtensionMatch(FileChangeCategory.Content, "*.spark"));

        }

        [Test]
        public void read_matcher_from_file_if_the_file_does_not_exist()
        {
            var matcher = FileMatcher.ReadFromFile("non-existent-file.txt");
            matcher.ShouldNotBeNull();
        }

        [Test]
        public void read_matcher_from_file()
        {
            new FileSystem().WriteStringToFile(FileMatcher.File, @"
*.spark=Application
*.css=Content
");


            var matcher = FileMatcher.ReadFromFile(FileMatcher.File);

            matcher.MatchersFor(FileChangeCategory.Content).ShouldContain(new ExtensionMatch(FileChangeCategory.Content, "*.css"));
            matcher.MatchersFor(FileChangeCategory.Application).ShouldContain(new ExtensionMatch(FileChangeCategory.Application, "*.spark"));
        }

        [Test]
        public void always_return_app_domain_for_files_in_appdomain()
        {
            theMatcher.CategoryFor("bin\\foo").ShouldEqual(FileChangeCategory.AppDomain);
            theMatcher.CategoryFor("bin\\innocuous.txt").ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void can_return_application()
        {
            theMatcher.CategoryFor("diagnostics.asset.config").ShouldEqual(FileChangeCategory.Application);
        }

        [Test]
        public void web_config_is_app_domain()
        {
            theMatcher.CategoryFor("web.config").ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void dll_is_app_domain()
        {
            theMatcher.CategoryFor("something.dll").ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void exe_is_app_domain()
        {
            theMatcher.CategoryFor("something.exe").ShouldEqual(FileChangeCategory.AppDomain);
        }

        [Test]
        public void match_on_extension()
        {
            theMatcher.CategoryFor("something.spark").ShouldEqual(FileChangeCategory.Content);
            theMatcher.CategoryFor("something.css").ShouldEqual(FileChangeCategory.Content);
        }

        [Test]
        public void match_on_nothing_is_nothing()
        {
            theMatcher.CategoryFor("foo.txt").ShouldEqual(FileChangeCategory.Nothing);
        }

        [Test]
        public void build_exact_match()
        {
            FileMatcher.Build("web.config=AppDomain")
                       .ShouldEqual(new ExactFileMatch(FileChangeCategory.AppDomain, "web.config"));
        }

        [Test]
        public void build_extension_match()
        {
            FileMatcher.Build("*.spark=Content")
                       .ShouldEqual(new ExtensionMatch(FileChangeCategory.Content, "*.spark"));
        }

        [Test]
        public void build_ends_with_match()
        {
            FileMatcher.Build("*.asset.config=Application")
                       .ShouldEqual(new EndsWithPatternMatch(FileChangeCategory.Application, "*.asset.config"));
        }
    }
}