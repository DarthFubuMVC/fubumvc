using System;

namespace FubuMVC.Core.Projections
{
    public class UrlProjection : IValueProjection
    {
        private readonly Func<IProjectionTarget, string> _fetchUrl;
        private readonly string _name;

        public UrlProjection(Func<IProjectionTarget, string> fetchUrl, string name)
        {
            _fetchUrl = fetchUrl;
            _name = name;
        }

        public void WriteValue(IProjectionTarget target, IMediaNode node)
        {
            var value = _fetchUrl(target);
            node.SetAttribute(_name, value);
        }
    }
}