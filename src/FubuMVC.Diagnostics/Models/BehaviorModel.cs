using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models
{
    public class BehaviorModel
    {
        public BehaviorModel()
        {
            Logs = new List<string>();
        }

        public string BehaviorType { get; set; }
        public IEnumerable<string> Logs { get; set; }
    }
}