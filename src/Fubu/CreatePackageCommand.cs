using System;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using Ionic.Zip;
using System.Collections.Generic;
using System.Linq;

namespace Fubu
{
    public enum TargetEnum
    {
        debug,
        release
    }

    public class CreatePackageInput
    {
        public CreatePackageInput()
        {
            TargetFlag = TargetEnum.release;
        }

        public string PackageFolder {get;set;}
        public string ZipFile { get; set; }
        public TargetEnum TargetFlag { get; set; }
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
            // Delete the file if it exists?
            if (fileSystem.PackageManifestExists(input.PackageFolder))
            {
                CreatePackage(input, fileSystem);
            }
            else
            {
                WritePackageManifestDoesNotExist(input.PackageFolder);
            }
        }

        public virtual void WritePackageManifestDoesNotExist(string packageFolder)
        {
            Console.WriteLine("The requested package folder at {0} does not have a package manifest.  Run 'fubu init-pak \"{0}\"' first.", packageFolder);
        }

        public virtual void CreatePackage(CreatePackageInput input, IFileSystem fileSystem)
        {
            var manifest = fileSystem.LoadPackageManifestFrom(input.PackageFolder);

            if (fileSystem.FileExists(input.ZipFile))
            {
                fileSystem.DeleteFile(input.ZipFile);
            }

            using (var zipFile = new ZipFile(input.ZipFile))
            {
                zipFile.AddFile(fileSystem.PackageManifestPathFor(input.PackageFolder), "");


                if (!TryFindAllAssemblies(manifest, input, zipFile))
                {
                    return;
                }

                AddDataFiles(input, zipFile, manifest);

                AddContentFiles(input, zipFile, manifest);

                zipFile.Save(input.ZipFile);
            }
        }

        private void AddContentFiles(CreatePackageInput input, ZipFile zipFile, PackageManifest manifest)
        {
            zipFile.AddFiles(new ZipFolderRequest()
                             {
                                 FileSet = manifest.ContentFileSet,
                                 ZipDirectory = FubuMvcPackages.WebContentFolder,
                                 RootDirectory = input.PackageFolder
                             });
        }

        private void AddDataFiles(CreatePackageInput input, ZipFile zipFile, PackageManifest manifest)
        {
            zipFile.AddFiles(new ZipFolderRequest(){
                FileSet = manifest.DataFileSet,
                ZipDirectory = PackageInfo.DataFolder,
                RootDirectory = Path.Combine(input.PackageFolder, PackageInfo.DataFolder)
            });
        }

        public bool TryFindAllAssemblies(PackageManifest manifest, CreatePackageInput input, ZipFile zipFile)
        {
            var binFolder = FileSystem.Combine(input.PackageFolder, "bin");

            var candidates = Directory.GetFiles(binFolder)
                .Where(x => PackageManifestReader.IsPotentiallyAnAssembly(x) && manifest.AssemblyNames.Contains(Path.GetFileNameWithoutExtension(x)));

            if (candidates.Count() == manifest.AssemblyNames.Count())
            {
                candidates.Each(file => zipFile.AddFile(file, "bin"));
                return true;
            }
            else
            {
                Console.WriteLine("Did not locate all designated assemblies at {0}", input.PackageFolder);
                Console.WriteLine("Looking for these assemblies in the package manifest file:");
                manifest.AssemblyNames.Each(name => Console.WriteLine("  " + name));
                Console.WriteLine("But only found");
                candidates.Each(file => Console.WriteLine("  " + file));
            }

            return false;
        }
    }

    public static class ZipFileExtensions
    {
        public static void AddFiles(this ZipFile file, ZipFolderRequest request)
        {
            request.WriteToZipFile(file);
        }
    }

    public class ZipFolderRequest
    {
        public FileSet FileSet { get; set; }
        public string RootDirectory { get; set; }
        public string ZipDirectory { get; set; }

        public void WriteToZipFile(ZipFile zipFile)
        {
            var cache = new Cache<string, string>(file => Path.Combine(ZipDirectory, file.PathRelativeTo(RootDirectory)));

            FileSet.IncludedFilesFor(RootDirectory).Each(cache.FillDefault);
            FileSet.ExcludedFilesFor(RootDirectory).Each(cache.Remove);

            cache.Each((file, name) => zipFile.AddFile(file, Path.GetDirectoryName(name)));
        }
    }
}