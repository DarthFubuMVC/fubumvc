using System.IO;
using System.Web.Hosting;

namespace FubuMVC.Core.Packaging
{
    public class FileSystemVirtualFile : VirtualFile
    {
        private readonly string _physicalPath;

        public FileSystemVirtualFile(string virtualPath, string physicalPath)
            : base(virtualPath)
        {
            _physicalPath = physicalPath;
        }

        public override Stream Open()
        {
            return new FileStream(_physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}