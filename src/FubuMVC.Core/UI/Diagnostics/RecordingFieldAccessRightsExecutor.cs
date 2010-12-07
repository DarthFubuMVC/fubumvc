using System.Collections.Generic;
using System.Data;
using System.Linq;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Security;

namespace FubuMVC.Core.UI.Diagnostics
{
    public class RecordingFieldAccessRightsExecutor : FieldAccessRightsExecutor
    {
        private readonly IDebugReport _debugReport;

        public RecordingFieldAccessRightsExecutor(IDebugReport debugReport)
        {
            _debugReport = debugReport;
        }

        public override AccessRight RightsFor(ElementRequest request, IEnumerable<IFieldAccessRule> securityRules, IEnumerable<IFieldAccessRule> logicRules)
        {
            var report = new FieldAccessReport(request.Accessor.Name);
            _debugReport.AddDetails(report);
            var securityRulesList = securityRules.ToArray();
            var logicRulesList = logicRules.ToArray();
            securityRulesList.Each(rule => report.AddVote("Authorization", rule.ToString(), rule.RightsFor(request).Name));
            if (securityRulesList.Length > 1)
            {
                report.AddVote("Authorization","Decision (most permissive wins)", getSecurityRights(request, securityRulesList).Name);
            }

            logicRulesList.Each(rule => report.AddVote("Logic", rule.ToString(), rule.RightsFor(request).Name));
            if (logicRulesList.Length > 1)
            {
                report.AddVote("Logic", "Decision (most restrictive wins)", getLogicRights(request, logicRulesList).Name);
            }

            var decision = base.RightsFor(request, securityRulesList, logicRulesList);
            report.AddVote("Effective", "(more restrictive of Logic and Authorization)", decision.Name);
            return decision;
        }
    }
    public class FieldAccessReport : IBehaviorDetails
    {
        private DataTable _data;

        public FieldAccessReport(string fieldName)
        {
            _data = new DataTable {TableName = "Field Access for " + fieldName};
            _data.Columns.Add("Category");
            _data.Columns.Add("Field Access Rule");
            _data.Columns.Add("Vote");
        }

        public void AddVote(string category, string policyDescription, string vote)
        {
            _data.Rows.Add(category, policyDescription, vote);
        }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            visitor.CustomTable(_data);
        }
    }

}