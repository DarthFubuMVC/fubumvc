using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Spark.Registration;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class SparkPrecompiler : IActivator
    {
        private readonly ISparkTemplateRegistry _templates;
        private readonly IViewEntryProviderCache _providerCache;
        private Action<IPackageLog> _activation;

        public SparkPrecompiler(ISparkTemplateRegistry templates, IViewEntryProviderCache providerCache)
        {
            _templates = templates;
            _providerCache = providerCache;
            UseActivation(p => Task.Factory.StartNew(() => Precompile(p)));
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            if(FubuMode.InDevelopment()) return;
            _activation(log);
        }

        public void Precompile(IPackageLog log)
        {
            _templates.ViewDescriptors().Each(vd => log.TrapErrors(() =>
            {
                var def = vd.ToViewDefinition();
                _providerCache.GetViewEntry(def.ViewDescriptor);
                _providerCache.GetViewEntry(def.PartialDescriptor);
            }));
        }

        // Just for testing
        public void UseActivation(Action<IPackageLog> activation)
        {
            _activation = activation;
        }
    }
}