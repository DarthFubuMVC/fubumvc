using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View;

namespace FubuMVC.Core.Resources.Conneg
{
    public static class OutputNodeExtensions
    {
        public static bool HasView(this IOutputNode node, IConditional conditional)
        {
            return node.Media().Any(x => x.Condition == conditional && x.Writes(MimeType.Html));
        }

        public static bool Writes(this IMedia media, MimeType mimeType)
        {
            return media.Mimetypes.Contains(mimeType.Value);
        }

        public static void AddView(this IOutputNode node, IViewToken token, IConditional conditional)
        {
            var writer = typeof (ViewWriter<>).CloseAndBuildAs<object>(token, node.ResourceType);

            node.Add(writer, conditional);
        }
    }
}