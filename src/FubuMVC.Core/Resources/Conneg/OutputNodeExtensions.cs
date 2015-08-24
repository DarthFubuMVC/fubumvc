using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace FubuMVC.Core.Resources.Conneg
{
    public static class OutputNodeExtensions
    {
        public static bool Writes(this IMediaWriter media, MimeType mimeType)
        {
            return media.Mimetypes.Contains(mimeType.Value);
        }

        public static void AddView(this IOutputNode node, IViewToken token)
        {
            var writer = typeof (ViewWriter<>).CloseAndBuildAs<IMediaWriter>(token, node.ResourceType);

            node.Add(writer);
        }

        public static IViewToken ViewFor(this IOutputNode node)
        {
            var media = node.Media().OfType<IViewWriter>().FirstOrDefault();
            return media == null ? null : media.View;
        }
    }
}