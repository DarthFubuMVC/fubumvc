using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public class BindingsAttacher 
    {
 
        private const string FallbackBindingsName = "bindings";
        public string BindingsName { get; set; }

        public BindingsAttacher()
        {
            BindingsName = FallbackBindingsName;
        }

        public bool CanAttach()
        {
            throw new NotImplementedException();
//            var descriptor = request.Template;
//            
//            return descriptor != null 
//                && descriptor.Bindings.Count() == 0;
        }

        public void Attach()
        {
            throw new NotImplementedException();
//            var target = request.Template;
//            var logger = request.Logger;
//            var descriptor = target.Descriptor.As<SparkViewToken>();
//
//            _templateLocator.LocateBindings(BindingsName, target).Each(template =>
//            {
//                descriptor.AddBinding(template);
//                var msg = "Binding attached : {0}".ToFormat(template.FilePath);
//                logger.Log(target, msg);
//            });
        }
    }
}