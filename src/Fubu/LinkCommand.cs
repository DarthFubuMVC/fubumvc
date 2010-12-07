using System;
using System.Diagnostics;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;
using System.Collections.Generic;
using System.Linq;

namespace Fubu
{
    public class LinkInput
    {
        public string AppFolder { get; set; }
        public string PackageFolder { get; set; }

        public bool RemoveFlag { get; set; }
        public bool CleanAllFlag { get; set; }
        public bool NotepadFlag { get; set; }

        public string RelativePathOfPackage()
        {
            var pkg = Path.GetFullPath(PackageFolder);
            var app = Path.GetFullPath(AppFolder);

            return pkg.PathRelativeTo(app);
        }
    }

    


    [CommandDescription("Links a package folder to an application folder in development mode")]
    public class LinkCommand : FubuCommand<LinkInput>
    {
        public override void Execute(LinkInput input)
        {
            input.AppFolder = AliasCommand.AliasFolder(input.AppFolder);
            input.PackageFolder = AliasCommand.AliasFolder(input.PackageFolder);

            

            Execute(input, new FileSystem());
        }

        private void Execute(LinkInput input, FileSystem fileSystem)
        {
            if (input.CleanAllFlag)
            {
                fileSystem.DeleteFile(input.AppFolder, PackageIncludeManifest.FILE);

                Console.WriteLine("Deleted the package include manifest file for " + input.AppFolder);
                return;
            }
            
            var manifest = fileSystem.LoadFromFile<PackageIncludeManifest>(input.AppFolder,
                                                                           PackageIncludeManifest.FILE);

            if (input.PackageFolder.IsNotEmpty())
            {
                updateManifest(input, fileSystem, manifest);
            }
            else
            {
                listCurrentLinks(input, manifest);
            }
            

            if (input.NotepadFlag)
            {
                fileSystem.OpenInNotepad(input.AppFolder, PackageIncludeManifest.FILE);
            }
        }

        private void listCurrentLinks(LinkInput input, PackageIncludeManifest manifest)
        {
            Console.WriteLine("Links for " + input.AppFolder);
            if (manifest.Folders.Any())
            {
                manifest.Folders.Each(x =>
                {
                    Console.WriteLine("  " + x);
                });
            }
            else
            {
                Console.WriteLine("No package links for " + input.AppFolder);
            }


        }

        private void updateManifest(LinkInput input, FileSystem fileSystem, PackageIncludeManifest manifest)
        {
            if (input.RemoveFlag)
            {
                remove(input, manifest);
            }
            else
            {
                add(fileSystem, input, manifest);
            }

            fileSystem.PersistToFile(manifest, input.AppFolder, PackageIncludeManifest.FILE);
        }

        private void remove(LinkInput input, PackageIncludeManifest manifest)
        {
            manifest.Exclude(input.RelativePathOfPackage());
            Console.WriteLine("Folder {0} was removed from the application at {1}", input.PackageFolder, input.AppFolder);
        }

        private void add(IFileSystem system, LinkInput input, PackageIncludeManifest manifest)
        {
            var exists = system.FileExists(input.PackageFolder, PackageManifest.FILE);
            if (!exists)
            {
                throw new ApplicationException("There is no package manifest file for the requested package folder at " + input.PackageFolder);
            }

            var wasAdded = manifest.Include(input.RelativePathOfPackage());
            Console.WriteLine(
                wasAdded
                    ? "Folder {0} was added to the application at {1}"
                    : "Folder {0} is already included in the application at {1}", input.PackageFolder, input.AppFolder);
        }
    }
}