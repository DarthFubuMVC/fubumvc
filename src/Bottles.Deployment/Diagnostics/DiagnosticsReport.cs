using System;
using System.Collections.Generic;
using System.IO;
using Bottles.Deployment.Parsing;
using HtmlTags;

namespace Bottles.Deployment.Diagnostics
{
    public class DiagnosticsReport
    {
        public void Report(DeploymentPlan plan)
        {
            Report(plan, "deploymentplan.html");
        }
        public void Report(DeploymentPlan plan, string locationToWriteReport)
        {
            var doc = new HtmlDocument();
            doc.Title = "Bottles Deployment Diagnostics";
            doc.AddStyle(getCss());

            HtmlTag mainDiv = new HtmlTag("div").AddClass("main");
            mainDiv.Add("h2").Text("Bottles Deployment Diagnostics");

            var left = buildLeftside(plan);
            mainDiv.Append(left);



            var right = buildRightside(plan);

            mainDiv.Append(right);



            doc.Add(mainDiv);

            doc.WriteToFile(locationToWriteReport);
            
        }

        private HtmlTag buildRightside(DeploymentPlan plan)
        {
            var right = new HtmlTag("div").AddClass("right");
            right.Append(new HtmlTag("h3").Text("Settings"));

            plan.Hosts.Each(h =>
            {
                right.Append(new HtmlTag("h4").Text(h.Name));
                var table = new TableTag();
                table.AddClass("details");

                table.AddHeaderRow(row =>
                {
                    row.Cell("Key");
                    row.Cell("Value");
                    row.Cell("Porvenance");
                });

                h.CreateDiagnosticReport().Each(s =>
                {
                    table.AddHeaderRow(row =>
                    {
                        row.Cell(s.Key);
                        row.Cell(s.Value);
                        row.Cell(s.Provenance);
                    });
                });

                right.Append(table);
            });

            return right;
        }

        private HtmlTag buildLeftside(DeploymentPlan dp)
        {
            HtmlTag left = new HtmlTag("div").AddClass("left");
            var tags = new List<HtmlTag>();

            tags.Add(commandLine());
            tags.Add(addRecipes(dp));
            tags.Add(addHosts(dp));
            tags.AddRange(addEnvironment(dp));
            left.Append(tags);

            return left;
        }

        private IList<HtmlTag> addEnvironment(DeploymentPlan dp)
        {
            HtmlTag tag = new HtmlTag("h3").Text("Environment");
            HtmlTag o = new HtmlTag("h4").Text("Overrides");


            var dl = new DLTag();

            dp.Environment.Overrides.Each((k, v) => { dl.AddDefinition(k, v); });

            var dl2 = new DLTag();
            HtmlTag x = new HtmlTag("h4").Text("Settings By Host");
            dp.Environment.EnvironmentSettingsData().AllKeys.Each(
                k => { dl2.AddDefinition(k, dp.Environment.EnvironmentSettingsData()[k]); });

            return new List<HtmlTag> {tag, o, dl, dl2};
        }

        private HtmlTag addHosts(DeploymentPlan dp)
        {
            HtmlTag tag = new HtmlTag("h3").Text("Hosts");

            var ol = new HtmlTag("ol");
            dp.Hosts.Each(h => { ol.Add("li").Text(h.Name); });
            tag.After(ol);

            return tag;
        }

        private HtmlTag addRecipes(DeploymentPlan dp)
        {
            HtmlTag tag = new HtmlTag("h3")
                .Text("Recipes");
            var ul = new HtmlTag("ol");

            dp.Recipes.Each(r => { ul.Add("li").Text(r.Name); });

            tag.After(ul);
            return tag;
        }

        private HtmlTag commandLine()
        {
            var dl = new DLTag();

            dl.AddDefinition("Command Line:", "bottle deploy {profile}");
            dl.AddDefinition("Ran On:", "2pm");
            dl.AddDefinition("Profile:", "{profile name}");

            return dl;
        }

        private string getCss()
        {
            Type type = typeof (DiagnosticsReport);
            string filename = "diagnostics.css";
            Stream stream = type.Assembly.GetManifestResourceStream(type, filename);
            if (stream == null) return String.Empty;
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}