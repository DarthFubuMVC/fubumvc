using System.Collections.Generic;
using FubuCore;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting
{
    public class DetailsTagWriter : IBehaviorDetailsVisitor
    {
        private readonly HtmlTag _holderTag;

        public DetailsTagWriter(HtmlTag holderTag)
        {
            _holderTag = holderTag;
        }

        public void ModelBinding(ModelBindingReport report)
        {
            var table = addDetail().Text("Bound Model " + report.BoundType.FullName).Child<TableTag>();

            table.AddClass("details");
            table.AddHeaderRow(row =>
            {
                row.Header("Key");
                row.Header("Value");
                row.Header("Source");
            });

            report.Each(binding =>
            {
                table.AddBodyRow(row =>
                {
                    row.Cell(binding.Key);
                    row.Cell(binding.Value.ToString());
                    row.Cell(binding.Source.ToString());
                });
            });
        }

        public void FileOutput(FileOutputReport report)
        {
            string text = "Wrote file:  " + report.ToString();
            addDetail().Text(text);
        }

        public void WriteOutput(OutputReport report)
        {
            addDetail().Text("Wrote Output ({0}):".ToFormat(report.ContentType));
            _holderTag.Add("pre").AddClass("content").Text(report.Contents);

        }

        public void Redirect(RedirectReport report)
        {
            addDetail().Text("Redirected to " + report.Url);
        }

        public void Exception(ExceptionReport report)
        {
            _holderTag.Add("pre").Text(report.Text);
        }

        public void SetValue(SetValueReport report)
        {
            var text = "Set value of {0} to {1}".ToFormat(report.Type.FullName, report.Value);
            addDetail().Text(text);
        }

        public void Authorization(AuthorizationReport report)
        {
            var table = addDetail().Text("Authorization").Child<TableTag>();

            table.AddClass("details");
            table.AddHeaderRow(row =>
            {
                row.Header("Policy");
                row.Header("Vote");
            });

            report.Details.Each(binding =>
            {
                table.AddBodyRow(row =>
                {
                    row.Cell(binding.PolicyDescription);
                    row.Cell(binding.Vote);
                });
            });
            table.AddFooterRow(footer =>
            {
                footer.AddClass("authz-decision");
                footer.Cell("Decision");
                footer.Cell(report.Decision);
            });
        }

        private HtmlTag addDetail()
        {
            return _holderTag.Add("div").AddClass("details");
        }

        public void Write(IBehaviorDetails details)
        {
            details.AcceptVisitor(this);
        }
    }
}