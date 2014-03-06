using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.View.Model
{
    public class Parsing
    {
        public Parsing()
        {
            Namespaces = Enumerable.Empty<string>();
        }

        public string ViewModelType { get; set; }
        public string Master { get; set; }
        public IEnumerable<string> Namespaces { get; set; }
    }
}