using System;
using FubuCore;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    [MarkedForTermination("use descriptor as is")]
    public class SparkViewToken : IViewToken
    {
        private readonly SparkDescriptor _descriptor;

        public SparkViewToken(SparkDescriptor viewDescriptor)
        {
            _descriptor = viewDescriptor;
        }

        public ObjectDef ToViewFactoryObjectDef()
        {
            var def = ObjectDef.ForType<ViewFactory>();
            def
                .DependencyByType(typeof (IViewEntrySource), typeof (ViewEntrySource))
                .DependencyByValue(_descriptor);

            return def;
        }

        public Type ViewType
        {
            get { return typeof (ISparkView); }
        }

        public Type ViewModel
        {
            get { return _descriptor.ViewModel; }
        }

        public string Name()
        {
            return _descriptor.Name();
        }

        public string Namespace
        {
            get { return _descriptor.Namespace; }
        }

        public override string ToString()
        {
            return _descriptor.RelativePath();
        }
    }
}