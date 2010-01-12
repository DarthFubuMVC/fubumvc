using System.Collections.Generic;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public class BindResult
    {
        public List<ConvertProblem> Problems = new List<ConvertProblem>();
        public object Value;

        public override string ToString()
        {
            return string.Format("BindResult: {0}, Problems:  {1}", Value, Problems.Count);
        }
    }
}