using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FubuMVC.UI.Scripts.Registration.Conventions
{
    public class PreprocessorParsingConvention : IScriptConfigurationAction
    {
        private static readonly Regex ReferenceExpression = new Regex(@"///\s*<reference\s*path=""(?<path>.*?\.(\w{2,4})*)""\s*/>", RegexOptions.Compiled);

        public void Configure(FilePool files, ScriptGraph graph)
        {
            files
                .FilesMatching(file => file.Extension == "js")
                .Each(file =>
                          {
                              using(var reader = new StreamReader(file.FullName))
                              {
                                  string line;
                                  while((line = reader.ReadLine()) != null)
                                  {
                                      var matches = ReferenceExpression.Matches(line);
                                      if(matches.Count == 0)
                                      {
                                          continue;
                                      }

                                      var path = matches[0].Groups["path"].Value;
                                      var script = getScript(graph, file.Name);
                                      var dependency = getScript(graph, path);
                                      if(script == null || dependency == null)
                                      {
                                          continue;
                                      }

                                      script.AddDependency(dependency);
                                  }
                              }
                          });
        }

        private Script getScript(ScriptGraph graph, string name)
        {
            return graph.GetScript(name).FirstOrDefault(s => s.Name == name);
        }
    }
}