using System.Collections.Generic;

namespace FubuMVC.Core.View.Model.Scanning
{
    public static class ScanRequestExtensions
    {
        public static void AddRoots(this ScanRequest request, IEnumerable<string> roots)
        {
            roots.Each(request.AddRoot);
        }
    }
}