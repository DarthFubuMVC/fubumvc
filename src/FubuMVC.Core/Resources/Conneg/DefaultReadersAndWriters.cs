using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    public class DefaultReadersAndWriters : ConnegRule, DescribesItself
    {
        protected override DoNext applyInputs(IInputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            node.Add(typeof(ModelBindingReader<>));
            node.Add(settings.FormatterFor(MimeType.Json));
            node.Add(settings.FormatterFor(MimeType.Xml));

            return DoNext.Continue;
        }

        protected override DoNext applyOutputs(IOutputNode node, BehaviorChain chain, ConnegSettings settings)
        {
            node.Add(settings.FormatterFor(MimeType.Json));
            node.Add(settings.FormatterFor(MimeType.Xml));

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.ShortDescription =
                "Accept Json, Xml, and HTTP form posts, writes Json or Xml with Json being the default in both cases";
        }
    }
}