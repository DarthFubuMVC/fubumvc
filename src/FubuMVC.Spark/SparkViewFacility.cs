using System;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : ViewFacility<SparkTemplate>
    {
        private readonly SparkViewEngine _engine;

        public SparkViewFacility(SparkViewEngine engine)
        {
            _engine = engine;
        }

        public override Func<IFubuFile, SparkTemplate> CreateBuilder(SettingsCollection settings)
        {
            return file => new SparkTemplate(file, _engine);
        }

        public override FileSet FindMatching(SettingsCollection settings)
        {
            return settings.Get<SparkEngineSettings>().Search;
        }
    }
}