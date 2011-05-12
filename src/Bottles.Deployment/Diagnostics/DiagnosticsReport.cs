using System;
using System.Collections.Generic;
using System.IO;
using Bottles.Deployment.Parsing;
using HtmlTags;

namespace Bottles.Deployment.Diagnostics
{
    public class DiagnosticsReport
    {
        public void Report()
        {
            
            

            var doc = new HtmlDocument();
            doc.Title = "hi";
            doc.AddStyle(getCss());

            var mainDiv = new HtmlTag("div").AddClass("main");
            mainDiv.Add("h2").Text("FubuMVC Diagnostics").Append("V");
            var navBar = mainDiv.Add("div").AddClass("homelink").Text("menu");
            
            navBar.Add("span").Text(" > " + "1");
            navBar.Add("span").Text(" > " + "2");

            var dp = new DeploymentPlan();
            dp.AddRecipes(new []{new Recipe("hir"), new Recipe("hir2"), });
            var tags = getTags(dp);
            mainDiv.Append(tags);

            doc.Add(mainDiv);

            
            doc.WriteToFile("deploymentreport.html");


        }

        private IList<HtmlTag> getTags(DeploymentPlan dp)
        {
            var tags = new List<HtmlTag>();
            tags.Add(new HtmlTag("h3").Text("Bottles Deployment Report"));
            tags.Add(commandLine());
            tags.Add(new HtmlTag("p").Text("Ran on {DATE}"));
            tags.Add(addProfile(dp));
            tags.Add(addRecipes(dp));
            tags.Add(addHosts(dp));

            var ul = new HtmlTag("ul");
            tags.Add(ul);

            ul.Add("li").Text("Hosts");
            ul.Add("li").Text("Profile");
            return tags;
        }

        private HtmlTag addHosts(DeploymentPlan dp)
        {
            var tag = new HtmlTag("div")
                .Append("h4").Text("Hosts");

            var ol = new HtmlTag("ol");
            dp.Hosts.Each(h =>
            {
                ol.Add("li").Text(h.Name);
            });
            return tag;
        }

        private HtmlTag addRecipes(DeploymentPlan dp)
        {
            var tag = new HtmlTag("div")
                .Append("h4").Text("Recipes");
            var ul = new HtmlTag("ol");

            dp.Recipes.Each(r =>
            {
                ul.Add("li").Text(r.Name);
            });
            
            tag.Append(ul);
            return tag;
        }

        private HtmlTag addProfile(DeploymentPlan dp)
        {
            return new HtmlTag("h4").Text("Profile: NAME");
        }

        private HtmlTag commandLine()
        {
            return new HtmlTag("h4").Text("Command Line:")
                .Append(new HtmlTag("pre").Text("bottles.exe deploy PROFILE"));
        }

        private string getCss()
        {
            var type = typeof (DiagnosticsReport);
            var filename = "diagnostics.css";
            var stream = type.Assembly.GetManifestResourceStream(type, filename);
            if (stream == null) return String.Empty;
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}