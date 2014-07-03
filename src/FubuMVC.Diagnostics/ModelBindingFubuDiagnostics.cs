using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.Logging;
using FubuCore.Descriptions;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.UI.Bootstrap.Collapsibles;
using HtmlTags;

namespace FubuMVC.Diagnostics
{
    public class ModelBindingFubuDiagnostics
    {
        private readonly BindingRegistry _bindingRegistry;

        public ModelBindingFubuDiagnostics(BindingRegistry bindingRegistry)
        {
            _bindingRegistry = bindingRegistry;
        }

        public HtmlTag get_binding_all()
        {
            var description = Description.For(_bindingRegistry);

            return new DescriptionBodyTag(description);
        }

        public HtmlTag VisualizePartial(ModelBindingLog log)
        {
            var title = "Model Binding for " + log.Report.ModelType.FullName;



            var collapsible = new CollapsibleTag(log.Id.ToString(), title);

            var description = Description.For(log.Report.Binder);

            collapsible.AppendContent(new HtmlTag("div").AddClass("model-binder-title").Text("Used " + description.Title));


            if (log.Report.OrderedProperties().Any())
            {
                var report = new ModelBindingHtmlReport();
                log.Report.AcceptVisitor(report);
                collapsible.AppendContent(report.Table);
            }
            else
            {
                var tag = new DescriptionBodyTag(description);
                collapsible.AppendContent(tag);
            }


            return collapsible;
        }
    }

    public class ModelBindingHtmlReport : IBindingReportVisitor
    {
        private readonly TableTag _table;
        private readonly Stack<string> _descriptions = new Stack<string>();
        private readonly Stack<BindingReport> _bindingStack = new Stack<BindingReport>();

        public ModelBindingHtmlReport()
        {
            _table = buildTable();
        }

        private static TableTag buildTable()
        {
            var table = new TableTag();
            table.AddClass("table table-striped table-bordered table-condensed");

            table.AddHeaderRow(row =>
            {
                row.Header("Property");
                row.Header("Handler");
                row.Header("Value(s)");
            });

            return table;
        }

        public HtmlTag Table
        {
            get { return _table; }
        }

        void IBindingReportVisitor.Report(BindingReport report)
        {
            _bindingStack.Push(report);
        }

        private void write(object handler, IEnumerable<BindingValue> values = null)
        {
            var description = Description.For(handler).Title;

            var propertyName = _descriptions.Reverse().Join("").Replace(".[", "[").TrimStart('.');
            var valueString = values == null
                                    ? string.Empty
                                    : values.Select(x => "'{0}' from '{1}'/{2}".ToFormat(x.RawValue, x.Source, x.RawKey)).Join(", ");


            _table.AddBodyRow(row =>
            {
                row.Cell(propertyName);
                row.Cell(description);
                row.Cell(valueString);
            });
        }

        void IBindingReportVisitor.Property(PropertyBindingReport report)
        {
            _descriptions.Push("." + report.Property.Name);
            object handler = (object)report.Converter ?? report.Binder;
            write(handler, report.Values);
        }

        void IBindingReportVisitor.Element(ElementBinding binding)
        {
            _descriptions.Push("[{0}]".ToFormat(binding.Index));
            write(binding.Binder);
        }

        void IBindingReportVisitor.EndReport()
        {
            _bindingStack.Pop();
        }

        void IBindingReportVisitor.EndProperty()
        {
            _descriptions.Pop();
        }

        void IBindingReportVisitor.EndElement()
        {
            _descriptions.Pop();
        }
    }

}