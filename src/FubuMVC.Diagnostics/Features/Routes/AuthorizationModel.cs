using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Features.Routes
{
    public class AuthorizationModel
    {
        public AuthorizationModel()
        {
            Rules = new List<AuthorizationRuleModel>();
        }

        public IEnumerable<AuthorizationRuleModel> Rules { get; set; }
    }
}