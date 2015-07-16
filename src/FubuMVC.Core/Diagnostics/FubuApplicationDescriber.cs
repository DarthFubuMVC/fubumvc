using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore.Util.TextWriting;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Diagnostics
{
    public static class FubuApplicationDescriber
    {
        public static string WriteDescription(IActivationDiagnostics diagnostics)
        {
            var writer = new System.IO.StringWriter();

            writeProperties(writer);

            writeErrors(writer, diagnostics);

            writeAssemblies(writer);

            writeBottles(writer, diagnostics);

            return writer.ToString();
        }

        private static void writeBottles(StringWriter writer, IActivationDiagnostics diagnostics)
        {
            writer.WriteLine("------------------------------------------------------------------------------------------------");
            writer.WriteLine("Logs");
            writer.WriteLine("------------------------------------------------------------------------------------------------");

            diagnostics.EachLog((o, log) =>
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

        private static void writeErrors(StringWriter writer, IActivationDiagnostics diagnostics)
        {
            writer.WriteLine("------------------------------------------------------------------------------------------------");
            writer.WriteLine("Errors");
            writer.WriteLine("------------------------------------------------------------------------------------------------");

            diagnostics.EachLog((o, log) =>
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
            report.StartColumns(3);
            report.AddDivider('-');
            report.AddText("Assemblies");
            report.AddDivider('-');

            AppDomain.CurrentDomain.GetAssemblies().Each(assem => {
                var assemblyName = assem.GetName();
                var file = findCodebase(assem);
                report.AddColumnData(assemblyName.Name, assemblyName.Version.ToString(), file);
            });

            report.AddDivider('-');
            report.Write(writer);

            writer.WriteLine();
        }

        private static string findCodebase(Assembly assem)
        {
            try
            {
                var file = assem.CodeBase;
                return file ?? "None";
            }
            catch (Exception)
            {
                return "None";
            }
        }

        private static void writeProperties(StringWriter writer)
        {
            var report = new TextReport();
            report.StartColumns(2);

            if (FubuApplication.Restarted.HasValue)
                report.AddColumnData("Restarted", FubuApplication.Restarted.ToString());
            report.AddColumnData("Application Path", FubuApplication.GetApplicationPath());

            report.Write(writer);

            writer.WriteLine();
        }


    }
}