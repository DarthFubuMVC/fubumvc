using System.Collections.Generic;

namespace FubuMVC.UI.Scripts.Registration.Conventions
{
    public class DefaultScriptRegistrationConvention : IScriptConfigurationAction
    {
        public void Configure(FilePool files, ScriptGraph graph)
        {
            // TODO -- need to use the virtual path provider to register the relative path here
            files
                .FilesMatching(file => true)
                .Each(file => graph.RegisterScript(file.Name, file.FullName));
        }
    }
}