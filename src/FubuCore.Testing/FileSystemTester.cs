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
    }
}