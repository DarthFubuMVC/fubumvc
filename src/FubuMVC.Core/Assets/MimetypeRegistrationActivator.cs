using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Assets.Content;
using FubuCore;

namespace FubuMVC.Core.Assets
{
    public class MimetypeRegistrationActivator : IActivator
    {
        private readonly IEnumerable<ITransformerPolicy> _policies;

        public MimetypeRegistrationActivator(IEnumerable<ITransformerPolicy> policies)
        {
            _policies = policies;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _policies.Each(p =>
            {
                p.Extensions.Each(ext =>
                {
                    p.MimeType.AddExtension(ext);
                    var trace = "Registered extension {0} with MimeType {1}".ToFormat(ext, p.MimeType.Value);
                    log.Trace(trace);
                });
            });
        }

        public override string ToString()
        {
            return "Registering file extensions for asset transformations to the appropriate MimeType's";
        }
    }
}