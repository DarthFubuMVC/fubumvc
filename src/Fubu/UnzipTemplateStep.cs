using System;
using System.IO;
using System.Reflection;
using Bottles.Exploding;
using Bottles.Zipping;

namespace Fubu
{
    public class UnzipTemplateStep : ITemplateStep
    {
        public static readonly string TemplateZip = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "defaultTemplate.zip");
        private readonly IZipFileService _zipFileService;

        public UnzipTemplateStep(IZipFileService zipFileService)
        {
            _zipFileService = zipFileService;
        }

        public void Execute(TemplatePlanContext context)
        {
            var zipPath = context.Input.ZipFlag;
            var templateZip = string.IsNullOrEmpty(zipPath)
                                  ? TemplateZip
                                  : Path.Combine(Environment.CurrentDirectory, zipPath);

            _zipFileService.ExtractTo(templateZip, context.TargetPath, ExplodeOptions.PreserveDestination);
        }
    }
}