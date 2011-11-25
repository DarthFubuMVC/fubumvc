using System.Collections.Generic;

namespace Fubu.Templating
{
    public class TemplatePlanContext
    {
        private readonly IList<string> _errors = new List<string>();

        public string TempDir { get; set; }
        public string TargetPath { get; set; }
        public NewCommandInput Input { get; set; }

        public void RegisterError(string error)
        {
            _errors.Fill(error);
        }

        public IEnumerable<string> Errors { get { return _errors; } }
    }
}