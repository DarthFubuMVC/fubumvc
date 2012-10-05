using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets.Diagnostics;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetDeclarationChecker
    {
      


        private readonly IAssetFileGraph _fileGraph;
        private readonly IPackageLog _log;
        private readonly AssetLogsCache _assetLogs;

        public AssetDeclarationChecker(IAssetFileGraph fileGraph, IPackageLog log, AssetLogsCache assetLogs)
        {
            _fileGraph = fileGraph;
            _log = log;
            _assetLogs = assetLogs;
        }

        // TODO -- would be nice if we could log the provenance of the file dependency.
        // i.e. -- which file had the wrong stuff
        public void VerifyFileDependency(string name)
        {
            var file = _fileGraph.Find(name);
            if (file == null)
            {
                // Guard clause to allow automated tests with faked up data work
                if (AssetDeclarationVerificationActivator.Latched) return;
                _log.MarkFailure(GetErrorMessage(name, _assetLogs));
            }
        }


        // For testing purposes only
        public static string GetErrorMessage(string name, AssetLogsCache logs)
        {
            var logsBuilder = new StringBuilder("Unable to locate asset named {0}".ToFormat(name));

            logsBuilder.AppendLine(Environment.NewLine + "Asset logs:");
            logs.FindByName(name).Logs.GroupBy(l => l.Provenance)
                .Each((grouped) =>
                            {
                                logsBuilder.AppendLine("    {0}".ToFormat(grouped.Key));
                                foreach (var assetLogEntry in grouped)
                                {
                                    logsBuilder.AppendLine("        {0}".ToFormat(assetLogEntry.Message));
                                }       
                            });              

            return logsBuilder.ToString();
        }
    }
}