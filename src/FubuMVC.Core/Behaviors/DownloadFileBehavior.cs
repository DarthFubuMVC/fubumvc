using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class DownloadFileModel
    {
        public string ContentType { get; set; }
        public string LocalFileName { get; set; }
        public string FileNameToDisplay { get; set; }
    }



    public class DownloadFileNode : Process
    {
        public DownloadFileNode() : base(typeof(DownloadFileBehavior))
        {
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Output; }
        }
    }

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

        protected override DoNext performInvoke()
        {
            var output = _request.Get<DownloadFileModel>();
            _writer.WriteFile(output.ContentType, output.LocalFileName, output.FileNameToDisplay);

            return DoNext.Continue;
        }
    }
}