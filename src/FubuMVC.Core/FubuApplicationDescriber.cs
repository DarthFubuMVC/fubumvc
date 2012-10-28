using System;
using System.Collections.Generic;
using Bottles;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Util.TextWriting;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Resources.Conneg;
using StringWriter = System.IO.StringWriter;

namespace FubuMVC.Core
{
    public static class FubuApplicationDescriber
    {
        public static string WriteDescription()
        {
            var writer = new System.IO.StringWriter();

            writeProperties(writer);

            writeErrors(writer);

            writeAssemblies(writer);

            writeBottles(writer);

            return writer.ToString();
        }

        private static void writeBottles(StringWriter writer)
        {
            writer.WriteLine("------------------------------------------------------------------------------------------------");
            writer.WriteLine("Logs");
            writer.WriteLine("------------------------------------------------------------------------------------------------");

            PackageRegistry.Diagnostics.EachLog((o, log) =>
            {
                if (log.Success)
                {
                    writer.WriteLine(o.ToString());
                    writer.WriteLine(log.FullTraceText());
                    writer.WriteLine("------------------------------------------------------------------------------------------------");
                }
            });

            writer.WriteLine();
        }

        private static void writeErrors(StringWriter writer)
        {
            writer.WriteLine("------------------------------------------------------------------------------------------------");
            writer.WriteLine("Errors");
            writer.WriteLine("------------------------------------------------------------------------------------------------");

            PackageRegistry.Diagnostics.EachLog((o, log) =>
            {
                if (!log.Success)
                {
                    writer.WriteLine(o.ToString());
                    writer.WriteLine(log.FullTraceText());
                    writer.WriteLine("------------------------------------------------------------------------------------------------");
                }
            });

            writer.WriteLine();

        }

        

        private static void writeAssemblies(StringWriter writer)
        {
            var report = new TextReport();
            report.StartColumns(2);
            report.AddDivider('-');
            report.AddText("Assemblies");
            report.AddDivider('-');

            AppDomain.CurrentDomain.GetAssemblies().Each(assem => {
                var assemblyName = assem.GetName();
                report.AddColumnData(assemblyName.Name, assemblyName.Version.ToString());
            });

            report.AddDivider('-');
            report.Write(writer);

            writer.WriteLine();
        }

        private static void writeProperties(StringWriter writer)
        {
            var report = new TextReport();
            report.StartColumns(2);

            if (FubuMvcPackageFacility.Restarted.HasValue)
                report.AddColumnData("Restarted", FubuMvcPackageFacility.Restarted.ToString());
            report.AddColumnData("Application Path", FubuMvcPackageFacility.GetApplicationPath());

            report.Write(writer);

            writer.WriteLine();
        }


    }
}