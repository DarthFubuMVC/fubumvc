using System;
using System.ComponentModel;
using Bottles.Commands;
using FubuCore;
using FubuCore.CommandLine;
using Bottles;

namespace Fubu
{
    public class IncludeWebContentInput
    {
        [Description("The folder or alias to find the package manifest")]
        public string Folder { get; set; }

        [Description("Use to add a new file match to the WebContent in a package")]
        public string IncludeFlag { get; set; }

        [Description("Use ")]
        public bool ConfigFlag { get; set; }

        public void AppendIncludes(FileSet files)
        {
            IncludeFlag.IfNotNull(files.AppendInclude);

            if (WebFormsFlag)
            {
                files.AppendInclude("*.as*x;*.master");
            }

            if (ConfigFlag)
            {
                files.AppendInclude("*.config");
            }
        }

        public bool WebFormsFlag { get; set; }
    }

    [CommandDescription("Add file includes to a package manifest file", Name = "include-web-content")]
    public class IncludeWebContentCommand : FubuCommand<IncludeWebContentInput>
    {
        public override bool Execute(IncludeWebContentInput input)
        {
            var folder = new AliasService().GetFolderForAlias(input.Folder);

            var manifest = new FileSystem().LoadPackageManifestFrom(folder);

            if (manifest.ContentFileSet == null)
            {
                manifest.ContentFileSet = new FileSet();
            }

            input.AppendIncludes(manifest.ContentFileSet);

            new FileSystem().PersistToFile(manifest, folder, PackageManifest.FILE);

            return true;
        }
    }
}