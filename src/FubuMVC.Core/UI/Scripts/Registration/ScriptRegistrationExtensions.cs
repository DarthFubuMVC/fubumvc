using System.Collections.Generic;
using FubuMVC.UI.Scripts.Registration.Conventions;

namespace FubuMVC.UI.Scripts.Registration
{
    public static class ScriptRegistrationExtensions
    {
        public static void Configure(this IEnumerable<IScriptConfigurationAction> actions, FilePool files, ScriptGraph graph)
        {
            actions.Each(action => action.Configure(files, graph));
        }

        public static void RegisterPreprocessorDirectives(this ScriptRegistry registry)
        {
            registry.ApplyConvention<PreprocessorParsingConvention>();
        }
    }
}