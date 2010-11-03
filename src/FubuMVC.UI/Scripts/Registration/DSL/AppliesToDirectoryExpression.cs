using System.IO;

namespace FubuMVC.UI.Scripts.Registration.DSL
{
    public class AppliesToDirectoryExpression
    {
        private readonly FilePool _files;

        public AppliesToDirectoryExpression(FilePool files)
        {
            _files = files;
        }

        public AppliesToDirectoryExpression ToDirectory(string relativePath)
        {
            // TODO -- use virtual path provider to resolve this
            _files.AddDirectory(new DirectoryInfo(relativePath));
            return this;
        }
    }
}