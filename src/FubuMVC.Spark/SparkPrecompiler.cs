using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Registration;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class SparkPrecompiler : IActivator
    {
        private readonly ISparkTemplateRegistry _templates;
        private readonly IViewEntryProviderCache _providerCache;
        private readonly SparkEngineSettings _settings;
        private Action<IPackageLog> _activation;

        public SparkPrecompiler(ISparkTemplateRegistry templates, IViewEntryProviderCache providerCache, SparkEngineSettings settings)
        {
            _templates = templates;
            _providerCache = providerCache;
            _settings = settings;

            UseActivation(p => Task.Factory.StartNew(() => Precompile(p)));
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            if (!_settings.PrecompileViews) return;
            _activation(log);
        }

        public void Precompile(IPackageLog log)
        {
            nonNativePartialDescriptors().Each( vd=> log.TrapErrors(() => {
                var def = vd.ToViewDefinition();
                _providerCache.GetViewEntry(def.ViewDescriptor);
                _providerCache.GetViewEntry(def.PartialDescriptor);
            }));
        }

        private IEnumerable<SparkDescriptor> nonNativePartialDescriptors()
        {
            return _templates.ViewDescriptors().Where(vd => !vd.Template.IsPartial());
        }

        // Just for testing
        public void UseActivation(Action<IPackageLog> activation)
        {
            _activation = activation;
        }
    }
}