using System;

namespace FubuMVC.UI.Scripts.Registration
{
    public class LambdaConfigurationAction : IScriptConfigurationAction
    {
        private readonly Action<FilePool, ScriptGraph> _configure;

        public LambdaConfigurationAction(Action<FilePool, ScriptGraph> configure)
        {
            _configure = configure;
        }

        public void Configure(FilePool files, ScriptGraph graph)
        {
            _configure(files, graph);
        }
    }
}