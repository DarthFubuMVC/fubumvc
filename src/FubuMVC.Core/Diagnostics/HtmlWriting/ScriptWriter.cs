using System.Collections.Generic;
using FubuMVC.Core.UI.Scripts;
using FubuMVC.Core.Urls;
using HtmlTags;
using System.Linq;
using HtmlTags.Extended.Attributes;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    public class ScriptQuery
    {
        public string Scripts { get; set; }
    }

    [FubuDiagnostics("Scripts")]
    public class ScriptWriter
    {
        private readonly ScriptGraph _scripts;
        private readonly IUrlRegistry _urls;

        public ScriptWriter(ScriptGraph scripts, IUrlRegistry urls)
        {
            _scripts = scripts;
            _urls = urls;
        }

        [FubuDiagnostics("Query script ordering and dependencies", ShownInIndex = true)]
        public HtmlDocument QueryScripts(ScriptQuery query)
        {
            var queryNames = query.Scripts == null
                                                 ? new string[0]
                                                 : query.Scripts.Split(',').Select(x => x.Trim()).Distinct();


            

            var document = DiagnosticHtml.BuildDocument(_urls, "Script Graph Query");
            document.Push("div").AddClass("script-query");
            
            document.Push("form").Attr("action", _urls.UrlFor<ScriptWriter>(x => x.QueryScripts(null)));
            
            document.Add("b").Text("For requested script names (comma-delimited):  ");
            document.Add(new TextboxTag("Scripts", queryNames.Join(", ")).Id("script-names-text"));
            document.Add("br");
            document.Add("input").Attr("type", "submit").Value("Query");
            document.Pop();
            


            if (queryNames.Any())
            {
                document.Add("hr");
                document.Add("b").Text("Results");
                document.Push("ul");
                
                var actuals = _scripts.GetScripts(queryNames);
                actuals.Each(script => document.Push("li").Text(script.Name));
            }

            document.Pop();

            return document;
        }
    }
}