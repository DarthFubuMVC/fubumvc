using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Core.Runtime.Files
{
    public interface IFubuApplicationFiles
    {
        string GetApplicationPath();

        IEnumerable<ContentFolder> AllFolders { get; }
        IEnumerable<ContentFolder> Folders { get; }
        IEnumerable<IFubuFile> FindFiles(FileSet fileSet);

        IFubuFile Find(string relativeName);
    }
}