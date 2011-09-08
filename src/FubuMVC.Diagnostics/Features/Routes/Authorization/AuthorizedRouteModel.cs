using System;
using FubuCore;

namespace FubuMVC.Diagnostics.Features.Routes.Authorization
{
    public class AuthorizedRouteModel
    {
        private readonly string _urlPattern;

        public AuthorizedRouteModel(Guid id, string urlPattern)
        {
            Id = id;
            _urlPattern = urlPattern;
        }

        public Guid Id { get; private set; }

        public string Route()
        {
            return _urlPattern.IsEmpty() ? "(default)" : _urlPattern;
        }
    }
}