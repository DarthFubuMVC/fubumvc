using System;
using System.Collections.Generic;
using System.IO;
using Spark.Bindings;

namespace FubuMVC.Spark.SparkModel
{
    public class FubuBindingProvider : BindingProvider
    {
        public override IEnumerable<Binding> GetBindings(BindingRequest request)
        {
            var viewFolder = request.ViewFolder;
            var viewPath = request.ViewPath;
            var bindings = new List<Binding>();

            throw new NotImplementedException("Redo this");

//            foreach (var binding in _templateRegistry.BindingsForView(viewPath))
//            {
//                using (var stream = viewFolder.GetViewSource(binding.ViewPath).OpenViewStream())
//                using (var reader = new StreamReader(stream))
//                {
//                    bindings.AddRange(LoadStandardMarkup(reader));
//                }
//            }

            return bindings;
        }
    }
}