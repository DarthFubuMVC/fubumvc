using System;
using System.IO;
using System.Reflection;
using Bottles.Exploding;
using Bottles.Zipping;
using FubuCore;

namespace Fubu.Templating.Steps
{
    public class UnzipTemplate : ITemplateStep
    {
        public static readonly string TemplateZip = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "defaultTemplate.zip");
        private readonly IZipFileService _zipFileService;

        public UnzipTemplate(IZipFileService zipFileService)
        {
            _zipFileService = zipFileService;
        }

        public string Describe(TemplatePlanContext context)
        {
            return "Unzip {0}".ToFormat(context.Input.ZipFlag);
        }

        public void Execute(TemplatePlanContext context)
        {
            var zipPath = context.Input.ZipFlag;
            var templateZip = string.IsNullOrEmpty(zipPath)
                                  ? TemplateZip
                                  : Path.Combine(Environment.CurrentDirectory, zipPath);

            _zipFileService.ExtractTo(templateZip, context.TempDir, ExplodeOptions.PreserveDestination);
        }
    }
}