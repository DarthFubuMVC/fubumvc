using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Downloads
{
    public class DownloadFileBehavior : BasicBehavior
    {
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;

        public DownloadFileBehavior(IOutputWriter writer, IFubuRequest request)
            : base(PartialBehavior.Ignored)
        {
            _writer = writer;
            _request = request;
        }

        protected override Task<DoNext> performInvoke()
        {
            var output = _request.Get<DownloadFileModel>();

            // TODO -- definitely switch to async writing
            _writer.WriteFile(output.ContentType, output.LocalFileName, output.FileNameToDisplay);

            return Task.FromResult(DoNext.Continue);
        }
    }
}