using System.IO;
using System.Linq;

namespace Fubu.Running
{
    public interface IFileMatch
    {
        bool Matches(string file);
        FileChangeCategory Category { get; }
    }

    public class BinFileMatch : IFileMatch
    {
        public bool Matches(string file)
        {
            return file.Replace('\\', '/').Split('/').Contains("bin");
        }

        public FileChangeCategory Category { get{return FileChangeCategory.AppDomain;} }
    }
}