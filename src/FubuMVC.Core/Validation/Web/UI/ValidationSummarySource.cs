using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class ValidationSummarySource : IActionSource
    {
        public class ValidationSummaryController
        {
            public ValidationSummary Summary(ValidationSummary summary)
            {
                return new ValidationSummary();
            }
        }

        Task<ActionCall[]> IActionSource.FindActions(Assembly applicationAssembly)
        {
            var actionCall = ActionCall.For<ValidationSummaryController>(x => x.Summary(null));
            return Task.FromResult(new[] {actionCall});
        }

    }
}