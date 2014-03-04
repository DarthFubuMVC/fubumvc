using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.View;

namespace FubuMVC.Core.Resources.Conneg
{
    public static class OutputNodeExtensions
    {

        public static bool Writes(this IMedia media, MimeType mimeType)
        {
            return media.Mimetypes.Contains(mimeType.Value);
        }

        public static void AddView(this IOutputNode node, IViewToken token, IConditional conditional)
        {
            var writer = typeof (ViewWriter<>).CloseAndBuildAs<object>(token, node.ResourceType);

            node.Add(writer, conditional);
        }

        public static IViewToken ViewFor(this IOutputNode node, IConditional conditional)
        {
            var media = node.Media().Where(x => x.Writer is IViewWriter && x.Condition == conditional).FirstOrDefault();
            return media == null ? null : media.Writer.As<IViewWriter>().View;
        }
    }
}