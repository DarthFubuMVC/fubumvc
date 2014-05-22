using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.View
{
    [Description("Auto import view model namespaces into the views")]
    public class AutoImportModelNamespacesConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<CommonViewNamespaces>();
            graph.Actions().Each(a =>
            {
                if (a.HasInput)
                {
                    AddImport(settings, () => a.InputType().Namespace);
                }
                if (a.HasOutput)
                {
                    AddImport(settings, () => a.OutputType().Namespace);
                }
            });

            var viewEngines = graph.Settings.Get<ViewEngineSettings>();
            viewEngines.Facilities.Each(x => x.ReadSharedNamespaces(settings));
        }

        private void AddImport(CommonViewNamespaces commonViewNamespaces, Func<string> namespaceSelector)
        {
            var modelNamespace = namespaceSelector();
            if (!commonViewNamespaces.IgnoredNamespacesForAutoImport.Any(modelNamespace.StartsWith))
            {
                commonViewNamespaces.Add(modelNamespace);
            }
        }
    }
}