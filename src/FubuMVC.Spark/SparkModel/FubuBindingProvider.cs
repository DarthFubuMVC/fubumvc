using System.Collections.Generic;
using System.IO;
using Spark.Bindings;

namespace FubuMVC.Spark.SparkModel
{
    // TODO : UT
    public class FubuBindingProvider : BindingProvider
    {
        private readonly ISparkTemplates _templates;
        public FubuBindingProvider(ISparkTemplates templates)
        {
            _templates = templates;
        }

        public override IEnumerable<Binding> GetBindings(BindingRequest request)
        {
            var viewFolder = request.ViewFolder;
            var viewPath = request.ViewPath;
            var bindings = new List<Binding>();

            foreach (var binding in _templates.BindingsForView(viewPath))
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