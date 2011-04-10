using System.Collections.Generic;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security;
using System.Linq;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class RecordingAuthorizationPolicyExecutor : AuthorizationPolicyExecutor
    {
        private readonly IDebugReport _report;

        public RecordingAuthorizationPolicyExecutor(IDebugReport report)
        {
            _report = report;
        }

        public override AuthorizationRight IsAuthorized(IFubuRequest request, IEnumerable<IAuthorizationPolicy> policies)
        {
            var decision = base.IsAuthorized(request, policies);
            if (policies.Any())
            {
                var authorizationReport = new AuthorizationReport();
                policies.Each(p =>
                {
                    var rights = p.RightsFor(request);
                    authorizationReport.AddVote(p.ToString(), rights.Name);
                });
                authorizationReport.Decision = decision.Name;
                _report.AddDetails(authorizationReport);
            }
            return decision;
        }
    }

    public class AuthorizationReport : IBehaviorDetails
    {
        private readonly List<AuthorizationReportDetail> _details = new List<AuthorizationReportDetail>();

        public void AddVote(string policyDescription, string vote)
        {
            _details.Add(new AuthorizationReportDetail(policyDescription, vote));
        }

        public string Decision { get; set; }

        public IEnumerable<AuthorizationReportDetail> Details { get { return _details; } }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.Authorization(this);
        }
    }

    public class AuthorizationReportDetail
    {
        public string PolicyDescription { get; private set; }
        public string Vote { get; private set; }

        public AuthorizationReportDetail(string policyDescription, string vote)
        {
            PolicyDescription = policyDescription;
            Vote = vote;
        }
    }

}