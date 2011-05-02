using System.ComponentModel;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;

namespace Bottles.Commands
{
    public class ListInput
    {
        [Description("Where to scan")]
        public string PointFlag { get; set; }

        public string PointToScan()
        {
            var x = PointFlag ?? ".";
            return x.ToFullPath();
        }
    }

    [CommandDescription("Lists all discovered manifests", Name="list")]
    public class ListCommand : FubuCommand<ListInput>
    {
        public override bool Execute(ListInput input)
        {
            var point = input.PointToScan();
            ConsoleWriter.Write("Looking for manifests starting from: {0}", point);
            
            var fileSet = new FileSet()
                     {
                         DeepSearch = true,
                         Include = PackageManifest.FILE
                     };

            var system = new FileSystem();

            var manifests = system.FileNamesFor(fileSet, input.PointToScan())
                    .Select(filename => Path.GetDirectoryName(filename));

            foreach (var manifestDir in manifests)
            {
                ConsoleWriter.PrintHorizontalLine();
                var shorty = manifestDir.Remove(0, point.Length);
                ConsoleWriter.Write("Found manifest at: {0}", shorty);
                var m = system.LoadApplicationManifestFrom(manifestDir);
                ConsoleWriter.Write("Name: {0}", m.Name);
                ConsoleWriter.Write("Role: {0}", m.Role);
            }
            

            return true;
        }
    }
}