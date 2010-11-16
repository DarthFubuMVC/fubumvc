using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class FileSystemTester
    {
        [Test]
        public void get_relative_path()
        {
            @"c:\a\b\1.bat".PathRelativeTo(@"c:\a\b").ShouldEqual("1.bat");
            @"c:\a\b\c\1.bat".PathRelativeTo(@"c:\a\b").ShouldEqual(@"c\1.bat");
            @"c:\a\b\c\d\1.bat".PathRelativeTo(@"c:\a\b").ShouldEqual(@"c\d\1.bat");
            @"c:\a\1.bat".PathRelativeTo(@"c:\a\b").ShouldEqual(@"..\1.bat");
            @"c:\a\1.bat".PathRelativeTo(@"c:\a\b\c").ShouldEqual(@"..\..\1.bat");
            @"c:\a\1.bat".PathRelativeTo(@"c:\a\b\c\d").ShouldEqual(@"..\..\..\1.bat");
            @"c:\A\b\1.bat".PathRelativeTo(@"c:\a\b").ShouldEqual("1.bat");
        }

        [Test]
        public void combine_when_it_is_only_one_value()
        {
            FileSystem.Combine("a").ShouldEqual("a");
        }

        [Test]
        public void combine_with_two_values()
        {
            FileSystem.Combine("a", "b").ShouldEqual("a\\b");
        }

        [Test]
        public void combine_with_three_values()
        {
            FileSystem.Combine("a", "b", "c").ShouldEqual("a\\b\\c");
        }

        [Test]
        public void combine_with_four_values()
        {
            FileSystem.Combine("a", "b", "c", "d").ShouldEqual("a\\b\\c\\d");
        }
    }
}