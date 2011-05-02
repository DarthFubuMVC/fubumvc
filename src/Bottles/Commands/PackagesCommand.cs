using System;
using System.ComponentModel;
using Bottles.Exploding;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Commands
{
    public class PackagesInput
    {
        [Description("Physical root folder (or valid alias) of the application")]
        public string AppFolder { get; set; }

        [Description("Removes all 'exploded' package folders out of the application folder")]
        public bool CleanAllFlag { get; set; }

        [Description("'Explodes' all the zip files underneath <appfolder>/bin/fubu-packages")]
        public bool ExplodeFlag { get; set; }

        [Description("Removes all package zip files and exploded directories from the application folder")]
        public bool RemoveAllFlag { get; set; }
    }

    [CommandDescription("Display and modify the state of package zip files in an application folder")]
    public class PackagesCommand : FubuCommand<PackagesInput>
    {
        private IFileSystem _system = new FileSystem();

        public override bool Execute(PackagesInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder).ToFullPath();

            Execute(input, _system);
            return true;
        }

        public void Execute(PackagesInput input, IFileSystem fileSystem)
        {
            var exploder = BuildExploder();

            if (input.CleanAllFlag)
                exploder.CleanAll(input.AppFolder);
            

            if (input.ExplodeFlag)
            {
                // TODO -- will need to do this for assembly packages as well
                exploder.ExplodeAllZipsAndReturnPackageDirectories(input.AppFolder);
            }

            if (input.RemoveAllFlag)
            {
                ConsoleWriter.Write("Removing all package files and directories from the application at " + input.AppFolder);
                _system.DeleteDirectory(input.AppFolder, "bin", BottleFiles.PackagesFolder);
            }

            exploder.LogPackageState(input.AppFolder);
        }


        //REVIEW: should this be here?
        public virtual IPackageExploder BuildExploder()
        {
            return PackageExploder.GetPackageExploder(new FileSystem());
        }
    }
}