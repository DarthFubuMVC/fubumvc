using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Runtime.Files
{
    /// <summary>
    /// Provides an abstraction to find files across the main application and all
    /// Bottles loaded into this application
    /// </summary>
    [ApplicationLevel]
    public interface IFubuApplicationFiles
    {
        /// <summary>
        /// Tells you the directory of the main application
        /// </summary>
        /// <returns></returns>
        string GetApplicationPath();

        /// <summary>
        /// All the folders in the current server that contain content for the running
        /// FubuMVC application
        /// </summary>
        IEnumerable<ContentFolder> AllFolders { get; }

        /// <summary>
        /// Find files by FileSet across the application and all Bottles
        /// </summary>
        /// <param name="fileSet"></param>
        /// <returns></returns>
        IEnumerable<IFubuFile> FindFiles(FileSet fileSet);

        /// <summary>
        /// Finds a file by relative name across the application and all Bottles
        /// </summary>
        /// <param name="relativeName"></param>
        /// <returns></returns>
        IFubuFile Find(string relativeName);
    }
}