using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Headers
{
    public class WriteHeadersBehavior : BasicBehavior
    {
        private readonly IOutputWriter _writer;
        private readonly IFubuRequest _request;

        public WriteHeadersBehavior(IOutputWriter writer, IFubuRequest request)
            : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            _request.Find<IHaveHeaders>()
                .SelectMany(x => x.Headers)
                .Each(x => x.Write(_writer));

            return DoNext.Continue;
        }
    }
}