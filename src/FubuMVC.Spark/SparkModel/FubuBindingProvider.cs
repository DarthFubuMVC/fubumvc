using System;
using System.Collections.Generic;
using System.IO;
using Spark.Bindings;

namespace FubuMVC.Spark.SparkModel
{
    public class FubuBindingProvider : BindingProvider
    {
        private readonly SparkTemplate[] _bindingTemplates;

        public FubuBindingProvider(SparkTemplate[] bindingTemplates)
        {
            _bindingTemplates = bindingTemplates;
        }

        public override IEnumerable<Binding> GetBindings(BindingRequest request)
        {
            var viewFolder = request.ViewFolder;
            var bindings = new List<Binding>();


            foreach (var binding in _bindingTemplates)
            {
                using (var stream = viewFolder.GetViewSource(binding.ViewPath).OpenViewStream())
                using (var reader = new StreamReader(stream))
                {
                    bindings.AddRange(LoadStandardMarkup(reader));
                }
            }

            return bindings;
        }
    }
}