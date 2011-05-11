using System.Collections.Generic;
using System.IO;
using Spark.Bindings;

namespace FubuMVC.Spark.SparkModel
{
    // TODO : UT
    public class FubuBindingProvider : BindingProvider
    {	
        private readonly ISparkTemplates _sparkTemplates;
        public FubuBindingProvider (ISparkTemplates sparkTemplates)
        {
            _sparkTemplates = sparkTemplates;
        }

        public override IEnumerable<Binding> GetBindings(BindingRequest request)
        {
            var viewFolder = request.ViewFolder;
            var viewPath = request.ViewPath;
            var bindings = new List<Binding>();
			
            foreach (var binding in _sparkTemplates.BindingsForView(viewPath)) 
            {
                var file = viewFolder.GetViewSource (binding.ViewPath);
                using (var stream = file.OpenViewStream())
                using (var reader = new StreamReader(stream)) {
                    bindings.AddRange (LoadStandardMarkup (reader));
                }					
            }		
			
            return bindings;
        }
    }
}