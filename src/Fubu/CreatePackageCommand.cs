using System;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu
{
    public class CreatePackageInput
    {
        public string PackageFolder {get;set;}
        public string ZipFile { get; set; }
    }

    [CommandDescription("Create a package file from a package directory", Name = "create-pak")]
    public class CreatePackageCommand : FubuCommand<CreatePackageInput>
    {
        public override void Execute(CreatePackageInput input)
        {
            input.PackageFolder = AliasCommand.AliasFolder(input.PackageFolder);
            Execute(input, new FileSystem());
        }

        public void Execute(CreatePackageInput input, IFileSystem fileSystem)
        {
            /*
             * Include the package manifest
             * Include the content with aspx, ascx, spark
             * Include all of data
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             */


            throw new NotImplementedException();
        }
    }
}