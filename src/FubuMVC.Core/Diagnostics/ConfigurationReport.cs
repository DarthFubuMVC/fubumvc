using FubuCore.Configuration;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics
{
    [FubuDiagnostics("Configuration Diagnostics", ShownInIndex = true)]
    public class ConfigurationReport
    {
        private SettingsProvider _provider;
        private IUrlRegistry _urls;

        public ConfigurationReport(ISettingsProvider provider, IUrlRegistry urls)
        {
            _provider = (SettingsProvider)provider;
            _urls = urls;
        }

        [FubuDiagnostics("All entries resolved")]
        public HtmlDocument FullResolvedConfigReport()
        {
            var table = new TableTag();
            table.AddHeaderRow(row =>
            {
                row.Cell("Key");
                row.Cell("Value");
                row.Cell("Provenance");
            });
            var items = _provider.CreateDiagnosticReport();
            foreach (var settingDataSource in items)
            {
                table.AddBodyRow(row =>
                {
                    row.Cell(settingDataSource.Key);
                    row.Cell(settingDataSource.Value);
                    row.Cell(settingDataSource.Provenance);
                });
            }
            

            return DiagnosticHtml.BuildDocument(_urls, "Full Configuration Dump", table);
        }

        [FubuDiagnostics("All entries in template mode")]
        public HtmlDocument FullTemplatedConfigReport()
        {
            var table = new TableTag();
            table.AddHeaderRow(row =>
            {
                row.Cell("Key");
                row.Cell("Value");
                row.Cell("Provenance");
            });
            var items = _provider.CreateDiagnosticReport();
            foreach (var settingDataSource in items)
            {
                table.AddBodyRow(row =>
                {
                    row.Cell(settingDataSource.Key);
                    row.Cell(settingDataSource.Value);
                    row.Cell(settingDataSource.Provenance);
                });
            }


            return DiagnosticHtml.BuildDocument(_urls, "Full Configuration Dump", table);
        }
    }
}