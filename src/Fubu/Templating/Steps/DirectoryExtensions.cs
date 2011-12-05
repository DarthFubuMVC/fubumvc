using System.IO;

namespace Fubu.Templating.Steps
{
    public static class DirectoryExtensions
    {
        public static void SafeDelete(this FileSystemInfo info)
        {
            info.Attributes &= ~FileAttributes.ReadOnly;
            var d = info as DirectoryInfo;
            if (d != null)
            {
                var files = d.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
                foreach(var f in files)
                {
                    f.SafeDelete();
                }
            }

            info.Delete();
        }
    }
}